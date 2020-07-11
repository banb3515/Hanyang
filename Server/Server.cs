#region API 참조
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            #endregion

            #region 스레드 생성
            // 시간표 가져오기 스레드 생성
            Thread getTimetableThread = new Thread(GetData);
            getTimetableThread.Start();
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
                        Program.Log(Buffer + "");
                        Packet packet = new Packet(Buffer);
                        DataManager(packet, clientSocket);
                    }
                }
                catch (Exception e)
                {
                    var ep = (clientSocket.RemoteEndPoint as IPEndPoint);

                    Program.Log("연결 해제: " + ep.ToString() + "/" + ids[ep.ToString()]);
                    Program.Log(e.Message, state: "warning");
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
                Program.Log("TEST");
                IPEndPoint ep = (IPEndPoint)client.RemoteEndPoint;
                Packet serverPacket = new Packet(PacketType.GetData, "server");

                //switch (packet.packetType)
                //{
                //    #region 데이터 가져오기
                //    case PacketType.GetData:
                //        Program.Log(ep.ToString() + ": 데이터 요청");
                //        if(packet.data.ContainsKey("Timetable"))
                //        {
                //            var controller = new JsonController("Timetable", dirPath: "Data");
                //            var json = controller.Read();
                //            serverPacket.data.Add("Timetable", json);
                //        }
                //        client.Send(serverPacket.ToBytes());
                //        break;
                //    #endregion
                //}
            }
            catch (Exception e)
            {
                Program.Log("DataManager: " + e.Message, state: "error");
            }
        }
        #endregion

        #region 스레드 이벤트
        #region 데이터 가져오기
        private async void GetData()
        {
            while (true)
            {
                #region 시간표
                Program.Log("시간표 가져오기: 데이터를 받아옵니다.");

                string KEY = "762281280e4943e58669a6b02991a67c"; // NEIS API 키
                string Type = "json"; // 가져오는 데이터 타입
                string pIndex = "1"; // 시작 페이지
                string pSize = "1000"; // 페이지 사이즈
                string ATPT_OFCDC_SC_CODE = "B10"; // 교육청코드
                string SD_SCHUL_CODE = "7010377"; // 학교코드
                string AY = DateTime.Now.Year.ToString(); // 현재 년도

                List<string> deps = new List<string> // 학과
                {
                    "건설정보과",
                    "건축과",
                    "자동화기계과",
                    "디지털전자과",
                    "자동차과",
                    "컴퓨터네트워크과"
                };

                // 데이터, 1 string = 반 이름, 2 string = 요일, 3 string = 교시, 4 string = 과목
                var datas = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

                // 학년만큼 반복
                for(int grade = 1; grade <= 3; grade ++)
                {
                    // 학과만큼 반복
                    foreach (string dep in deps)
                    {
                        int day = 0;
                        switch(DateTime.Now.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                day = 4;
                                break;
                            case DayOfWeek.Tuesday:
                                day = 3;
                                break;
                            case DayOfWeek.Wednesday:
                                day = 2;
                                break;
                            case DayOfWeek.Thursday:
                                day = 1;
                                break;
                            case DayOfWeek.Friday:
                                day = 0;
                                break;
                            case DayOfWeek.Saturday:
                                day = -1;
                                break;
                            case DayOfWeek.Sunday:
                                day = -2;
                                break;
                        }
                        // API URL 지정
                        string url = "https://open.neis.go.kr/hub/hisTimetable?" + 
                            "KEY=" + KEY + // API 키
                            "&Type=" + Type + // 파일 타입
                            "&pIndex=" + pIndex + // 시작 페이지
                            "&pSize=" + pSize + // 페이지 사이즈
                            "&ATPT_OFCDC_SC_CODE=" + ATPT_OFCDC_SC_CODE + // 교육청 코드
                            "&SD_SCHUL_CODE=" + SD_SCHUL_CODE + // 학교 코드
                            "&DDDEP_NM=" + dep + // 학과
                            "&GRADE=" + grade + // 학년
                            "&AY=" + AY + // 년도
                            "&TI_TO_YMD=" + DateTime.Now.AddDays(day).ToString("yyyyMMdd") + // 시간표 마지막 날짜
                            "&TI_FROM_YMD=" + DateTime.Now.AddMonths(-1).ToString("yyyyMMdd"); // 시간표 시작 날짜

                        // json 데이터 가져오기
                        var jsonStr = new WebClient().DownloadString(url).ToString();
                        var jsonObj = JObject.Parse(jsonStr);
                        var jsonTimetable = jsonObj["hisTimetable"];

                        string resultCode = ""; // 결과 코드
                        string resultMsg = ""; // 결과 메세지

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

                            // dep 학과 데이터 가져오기
                            for (int i = 0; i < length; i++)
                            {
                                var date = row[i]["ALL_TI_YMD"].ToString(); // 시간표 날짜
                                date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2); // 20200620 -> 2020/06/20 포맷

                                // 시간표 날짜가 현재 날짜보다 클 경우 아래 구문 취소
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
                                // 정상적인 데이터를 가져오기 위해 데이터 정제
                                if (!className.Contains("건설") && !className.Contains("건축") &&
                                    !className.Contains("기계") && !className.Contains("전자") &&
                                    !className.Contains("자동차") && !className.Contains("컴넷"))
                                    continue;

                                var perio = row[i]["PERIO"].ToString(); // 교시
                                var subject = row[i]["ITRT_CNTNT"].ToString(); // 과목
                                // 불필요한 문자 제거
                                if (subject.Contains("*"))
                                    subject = subject.Replace("* ", "");

                                // 데이터 추가
                                if (!datas.ContainsKey(className))
                                    datas.Add(className, new Dictionary<string, Dictionary<string, string>>());
                                if (!datas[className].ContainsKey(dow))
                                    datas[className].Add(dow, new Dictionary<string, string>());
                                if (!datas[className][dow].ContainsKey(perio))
                                    datas[className][dow].Add(perio, subject);
                            }
                        }
                        else
                            Program.Log("시간표 가져오기 오류: " + resultCode + " - " + resultMsg, "error");
                    }
                }

                // json 파일 생성
                var jObjStr = JsonConvert.SerializeObject(datas);
                var controller = new JsonController("Timetable", dirPath: "Data");
                var jObj = JObject.Parse(jObjStr);
                await controller.Write(jObj);

                Program.Log("시간표 가져오기: 데이터를 받아왔습니다.");
                #endregion

                #region 급식
                #endregion

                #region 학사일정
                #endregion

                // 1시간 뒤 재실행
                await Task.Delay(3600000); // 1000 = 1초
            }
        }
        #endregion
        #endregion

        #region 함수
        #endregion
    }
}