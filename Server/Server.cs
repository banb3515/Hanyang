#region API 참조
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

            Thread getDataThread = new Thread(async () =>
            {
                string KEY = "762281280e4943e58669a6b02991a67c";
                string Type = "json";
                string pIndex = "1";
                string pSize = "1000";
                string ATPT_OFCDC_SC_CODE = "B10";
                string SD_SCHUL_CODE = "7010377";
                string AY = DateTime.Now.Year.ToString();

                List<string> deps = new List<string>();
                deps.Add("건설정보과");
                deps.Add("건축과");
                deps.Add("자동화기계과");
                deps.Add("디지털전자과");
                deps.Add("자동차과");
                deps.Add("컴퓨터네트워크과");

                while (true)
                {
                    int c = 1;
                    foreach (string dep in deps)
                    {
                        string url = "https://open.neis.go.kr/hub/hisTimetable?KEY=" + KEY +
                            "&Type=" + Type + "&pIndex=" + pIndex + "&pSize=" + pSize + "&ATPT_OFCDC_SC_CODE=" + ATPT_OFCDC_SC_CODE +
                            "&SD_SCHUL_CODE=" + SD_SCHUL_CODE + "&DDDEP_NM=" +
                            "컴퓨터네트워크과" + "&GRADE=" + 2 + "&AY=" + AY;

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

                        if (resultCode == "INFO-000")
                        {
                            int length = Convert.ToInt32(jsonTimetable.First["head"].First["list_total_count"]);
                            var row = jsonTimetable[1]["row"];
                            for(int i = length - 68; i < length; i ++)
                            {
                                // 1번 ~ 14번: 월요일
                                // 15번 ~ 28번: 화요일
                                // 29번 ~ 40번: 수요일
                                // 41번 ~ 54번: 목요일
                                // 55번 ~ 68번: 금요일
                                var date = row[i]["ALL_TI_YMD"].ToString();
                                date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);

                                if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(date)) < 0)
                                    continue;
                                var grade = row[i]["GRADE"].ToString();
                                var className = row[i]["CLRM_NM"].ToString();
                                var perio = row[i]["PERIO"].ToString();
                                var subject = row[i]["ITRT_CNTNT"].ToString();
                                if (subject.Contains("*"))
                                    subject = subject.Replace("* ", "");
                                Console.WriteLine(c++ + " - " + date + ", 학년: " + row[i]["GRADE"] + ", 반: " + row[i]["CLRM_NM"] + ", 교시: " + row[i]["PERIO"] + ", 과목: " + subject);
                            }
                        }
                        else
                            Program.Log("시간표 가져오기 오류: " + resultCode + " - " + resultMsg, "error");

                        break;
                    }
                    Console.WriteLine("끝");
                    await Task.Delay(600000);
                }
            });
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
    }
}