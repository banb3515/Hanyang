#region API 참조
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using TcpData;
#endregion

namespace Server
{
    class Server
    {
        #region 변수
        private Socket listenerSocket; // 리스너 소켓
        private Dictionary<string, Client> clients; // string = Packet.senderID
        #endregion

        #region 생성자
        public Server(int port)
        {
            #region 변수 초기화
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
                Program.Log("Server 생성자: " + e.Message, "error");
            }
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
                    clients.Add(ep.ToString(), client);
                    Program.Log("연결: " + ep.ToString());
                }
            }
            catch (Exception e)
            {
                Program.Log("ListenThread: " + e.Message, "error");
            }
        }
        #endregion

        #region 클라이언트로부터 데이터 받음
        public void DataIn(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;

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
                { break; }
            }
        }
        #endregion
    }
}