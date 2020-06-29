using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using TcpData;

namespace Hanyang.Server
{
    public class Client
    {
        #region 변수
        private Socket server;
        private string senderID;
        #endregion

        #region 생성자
        public Client(string serverIP, int port)
        {
            try
            {
                var clientIP = Packet.GetIP4Address();

                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverIP), port);

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
                try
                {
                    Buffer = new byte[server.SendBufferSize];
                    readBytes = server.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(Buffer));
                    }
                }
                catch { }
            }
        }
        #endregion

        #region 서버로부터 받은 데이터 관리
        private async void DataManager(Packet packet)
        {
            try
            {
                switch (packet.packetType)
                {
                    case PacketType.Registration:
                        senderID = packet.data["ID"].ToString();
                        App.NewestVersion = packet.data["Version"].ToString();
                        if (App.VERSION != App.NewestVersion)
                            await MainPage.GetInstance().DisplayAlert("업데이트", "* 한양이 앱을 업데이트할 수 있습니다.\n" + 
                                "- 현재 버전: v" + App.VERSION + "\n- 최신버전: v" + App.NewestVersion, "확인");
                        break;
                }
            }
            catch (Exception e)
            {
                await MainPage.GetInstance().DisplayAlert("오류", "* DataManager\n- 오류 내용\n" + e.Message, "확인");
            }
        }
        #endregion
    }
}