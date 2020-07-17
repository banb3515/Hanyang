#region API 참조
using Newtonsoft.Json.Linq;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using TcpData;

using Xamarin.Forms;
#endregion

namespace Hanyang
{
    public class Server
    {
        #region 변수
        private Socket server; // 서버 소켓
        private string senderID; // 발신자 ID
        private string ipAddress; // 서버 IP 주소
        private int port; // 서버 포트 번호
        #endregion

        #region 생성자
        public Server(string ipAddress, int port)
        {
            try
            {
                this.ipAddress = ipAddress;
                this.port = port;
                var clientIP = Packet.GetIP4Address();

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                server.Connect(ep);
            }
            catch
            {
                throw new Exception("서버에 연결할 수 없습니다.\n프로그램을 종료합니다.");
            }

            try
            {
                Thread thread = new Thread(DataIn);
                thread.Start();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region 서버로부터 데이터 받음
        private void DataIn()
        {
            byte[] Buffer;
            int readBytes;

            for (; ; )
            {
                //try
                //{
                    Buffer = new byte[server.SendBufferSize];
                    readBytes = server.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                //}
                //catch (Exception e)
                //{
                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        await MainPage.GetInstance().DisplayAlert("DataIn Error", e.Message, "asd");
                //    });
                //}
            }
        }
        #endregion

        #region 서버로부터 받은 데이터 관리
        private void DataManager(Packet packet)
        {
            Debug.WriteLine("@Type@" + packet.Data["Type"]);
            var type = (PacketType)Enum.Parse(typeof(PacketType), packet.Data["Type"].ToString());

            //try
            //{
                #region ID 할당
                if (type == PacketType.Registration)
                {
                    senderID = packet.Data["ID"].ToString();
                    App.NewestVersion = packet.Data["Version"].ToString();
                    if (App.VERSION != App.NewestVersion)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await MainPage.GetInstance().DisplayAlert("업데이트", "* 한양이 앱을 업데이트할 수 있습니다.\n" +
                                "- 현재 버전: v" + App.VERSION + "\n- 최신버전: v" + App.NewestVersion, "확인");
                        });
                    }
                }
                #endregion

                #region 데이터 가져오기
                else if (type == PacketType.GetData)
                {
                    Debug.WriteLine("@Timetable@" + packet.Data["Timetable"].ToString());
                }
                #endregion
            //}
            //catch (Exception e)
            //{
            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        await MainPage.GetInstance().DisplayAlert("오류", "* DataManager\n- 오류 내용\n" + e.Message, "확인");
            //    });
            //}
        }
        #endregion

        #region 서버로부터 요청
        public async void GetTimetable()
        {
            try
            {
                Packet packet = new Packet(PacketType.GetData, senderID);
                packet.Data.Add("Timetable", "1");
                server.Send(packet.ToBytes());
            }
            catch (Exception e)
            {
                await MainPage.GetInstance().DisplayAlert("오류", "* Request\n- 오류 내용\n" + e.Message, "확인");
            }
        }
        #endregion
    }
}