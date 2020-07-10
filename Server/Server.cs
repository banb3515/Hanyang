#region API 참조
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using TcpData;
#endregion

namespace Server
{
    class Server
    {
        #region 변수
        private Socket listenerSocket; // 리스너 소켓
        private Dictionary<string, string> ids; // string1 = IP:Port, string2 = Client ID
        private Dictionary<string, Client> clients; // string = Packet.senderID
        #endregion

        #region 생성자
        public Server(int port)
        {
            #region 변수 초기화
            ids = new Dictionary<string, string>();
            clients = new Dictionary<string, Client>();
            #endregion

            #region 서버 실행
            try
            {
                Program.Log("서버 실행: v" + Program.VERSION + " - " + Packet.GetIP4Address() + ":" + port);
                listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Packet.GetIP4Address()), port);
                listenerSocket.Bind(ep);

                Thread listenThread = new Thread(ListenThread);
                listenThread.Start();
            }
            catch (Exception e)
            {
                Program.Log("Server 생성자: " + e.Message, state: "error");
            }

            Thread getDataThread = new Thread(GetData);
            getDataThread.Start();
            #endregion
        }
        #endregion

        #region ListenThread
        private void ListenThread()
        {
            try
            {
                for (; ; )
                {
                    listenerSocket.Listen(0);
                    Client client = new Client(listenerSocket.Accept());
                    IPEndPoint ep = (IPEndPoint)client.client.RemoteEndPoint;
                    ids.Add(ep.ToString(), client.id);
                    clients.Add(client.id, client);
                    Program.Log("연결: " + ep.ToString() + "/" + client.id);
                }
            }
            catch (Exception e)
            {
                Program.Log("ListenThread: " + e.Message, state: "error");
            }
        }
        #endregion

        #region 클라이언트로부터 데이터 받음
        public void DataIn(object cSocket)
        {
            Socket clientSocket = cSocket as Socket;

            byte[] Buffer;
            int readBytes;

            for (; ; )
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet, clientSocket);
                    }
                }
                catch 
                {
                    var ep = (clientSocket.RemoteEndPoint as IPEndPoint);

                    Program.Log("연결 해제: " + ep.ToString() + "/" + ids[ep.ToString()]);

                    #region 데이터 삭제
                    if (clients.ContainsKey(ids[ep.ToString()]))
                        clients.Remove(ids[ep.ToString()]);

                    if (ids.ContainsKey(ep.ToString()))
                        ids.Remove(ep.ToString());
                    #endregion

                    break;
                }
            }
        }
        #endregion

        #region 클라이언트로부터 받은 데이터 관리
        private void DataManager(Packet packet, Socket client)
        {
            try
            {
                IPEndPoint ep = (IPEndPoint)client.RemoteEndPoint;

                switch (packet.packetType)
                {
                    case PacketType.Test:
                        
                        break;
                }
            }
            catch (Exception e)
            {
                Program.Log("DataManager: " + e.Message, state: "error");
            }
        }
        #endregion

        #region 스레드 이벤트
        #region GetData
        private async void GetData()
        {
            string KEY = "762281280e4943e58669a6b02991a67c"; // NEIS API 키
            string Type = "json"; // 가져오는 데이터 타입
            string pIndex = "1"; // 시작 페이지
            string pSize = "1000"; // 페이지 사이즈
            string ATPT_OFCDC_SC_CODE = "B10"; // 교육청코드
            string SD_SCHUL_CODE = "7010377"; // 학교코드
            string AY = DateTime.Now.Year.ToString(); // 현재 년도

            List<string> deps = new List<string>
            {
                "건설정보과",
                "건축과",
                "자동화기계과",
                "디지털전자과",
                "자동차과",
                "컴퓨터네트워크과"
            };

            while (true)
            {
                int c = 1;

                // 데이터, 1 string = 반 이름, 2 string = 요일, 3 string = 교시, 4 string = 과목
                var datas = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

                for(int grade = 1; grade <= 3; grade ++)
                {
                    foreach (string dep in deps)
                    {
                        string url = "https://open.neis.go.kr/hub/hisTimetable?KEY=" + KEY +
                            "&Type=" + Type + "&pIndex=" + pIndex + "&pSize=" + pSize + "&ATPT_OFCDC_SC_CODE=" + ATPT_OFCDC_SC_CODE +
                            "&SD_SCHUL_CODE=" + SD_SCHUL_CODE + "&DDDEP_NM=" +
                            dep + "&GRADE=" + grade + "&AY=" + AY + "&TI_FROM_YMD=" + DateTime.Now.AddDays(-5).ToString("yyyyMMdd");

                        var jsonStr = new WebClient().DownloadString(url).ToString();
                        var jsonObj = JObject.Parse(jsonStr);
                        var jsonTimetable = jsonObj["hisTimetable"];

                        string resultCode = "";
                        string resultMsg = "";

                        var result = jsonObj["RESULT"];
                        if (result != null)
                        {
                            resultCode = result["CODE"].ToString();
                            resultMsg = result["MESSAGE"].ToString();
                        }
                        else
                        {
                            result = jsonTimetable.First["head"].Last["RESULT"];
                            resultCode = result["CODE"].ToString();
                            resultMsg = result["MESSAGE"].ToString();
                        }

                        // 정상 작동
                        if (resultCode == "INFO-000")
                        {
                            int length = Convert.ToInt32(jsonTimetable.First["head"].First["list_total_count"]); // 가져온 데이터 길이
                            var row = jsonTimetable[1]["row"]; // 데이터

                            #region dep 학과 데이터 가져오기
                            for (int i = 0; i < length; i++)
                            {
                                var date = row[i]["ALL_TI_YMD"].ToString();
                                date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);

                                if (!(DateTime.Compare(DateTime.Now, Convert.ToDateTime(date)) > 0))
                                    continue;

                                string dow = ""; // 요일
                                switch (Convert.ToDateTime(date).DayOfWeek)
                                {
                                    case DayOfWeek.Monday:
                                        dow = "Monday";
                                        break;
                                    case DayOfWeek.Tuesday:
                                        dow = "Tuesday";
                                        break;
                                    case DayOfWeek.Wednesday:
                                        dow = "Wednesday";
                                        break;
                                    case DayOfWeek.Thursday:
                                        dow = "Thursday";
                                        break;
                                    case DayOfWeek.Friday:
                                        dow = "Friday";
                                        break;
                                }
                                string className = row[i]["CLRM_NM"].ToString(); // 반 이름
                                if (!className.Contains("건설") && !className.Contains("건축") &&
                                    !className.Contains("기계") && !className.Contains("전자") &&
                                    !className.Contains("자동차") && !className.Contains("컴넷"))
                                    continue;

                                var perio = row[i]["PERIO"].ToString(); // 교시
                                var subject = row[i]["ITRT_CNTNT"].ToString(); // 과목
                                if (subject.Contains("*"))
                                    subject = subject.Replace("* ", ""); // 불필요한 문자 제거

                                if (!datas.ContainsKey(className))
                                    datas.Add(className, new Dictionary<string, Dictionary<string, string>>());
                                if (!datas[className].ContainsKey(dow))
                                    datas[className].Add(dow, new Dictionary<string, string>());
                                if (!datas[className][dow].ContainsKey(perio))
                                    datas[className][dow].Add(perio, subject);
                            }
                            #endregion
                        }
                        else
                            Program.Log("시간표 가져오기 오류: " + resultCode + " - " + resultMsg, "error");
                    }
                }

                var b = JsonConvert.SerializeObject(datas, Formatting.Indented);
                var controller = new JsonController("Timetable", dirPath: "GetData");
                var jObj = JObject.Parse(b);
                await controller.Write(jObj);

                await Task.Delay(600000);
            }
        }
        #endregion
        #endregion

        #region 함수
        #endregion
    }
}