#region API 참조
using System;
using System.IO;
using System.Threading;
#endregion

namespace Server
{
    class Program
    {
        #region 변수
        public const string VERSION = "1.1"; // 버전

        public static Server server; // 서버

        private static bool serverStatus; // 서버 상태
        private static Thread serverThread; // 서버 스레드
        private static Thread commandThread; // 명령어 스레드
        #endregion

        #region Main
        public static void Main(string[] args)
        {
            serverStatus = false;

            if (args.Length == 0)
            {
                StartServer(35155);
            }
            else if (args.Length == 1)
            {
                try
                {
                    StartServer(Convert.ToInt32(args[0]));
                }
                catch
                {
                    Console.WriteLine("* 포트 번호는 숫자만 입력 가능합니다.");
                }
            }
            else
                Console.WriteLine("잘못된 실행 방법입니다.\n실행 방법: '<프로그램 경로>' 또는 '<프로그램 경로> <포트 번호>'");
        }
        #endregion

        #region 서버 시작
        public static void StartServer(int port)
        {
            if (!serverStatus)
            {
                serverStatus = true;

                serverThread = new Thread(() =>
                {
                    server = new Server(port);
                });

                serverThread.Start();

                commandThread = new Thread(() =>
                {
                    string input;

                    while(true)
                    {
                        input = Console.ReadLine().ToLower();
                        if(!string.IsNullOrWhiteSpace(input))
                            new Command(input);
                    }
                });

                commandThread.Start();
            }
            else
                Log("서버가 이미 실행 중입니다.");
        }
        #endregion

        #region 서버 종료
        public static void StopServer()
        {
            if (serverStatus)
            {
                serverThread.Interrupt();
                commandThread.Interrupt();
            }
            else
                Log("서버가 시작되지 않아 종료할 수 없습니다.");
        }
        #endregion

        #region 로그 기록
        public static void Log(string msg, string state = "info")
        {
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(Environment.CurrentDirectory));
            string logPath = directory.ToString() + @"/Log/" +
                DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Hour + ".log";
            string dirPath = directory.ToString() + @"/Log/" +
                DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(logPath);

            string now = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
            state = state.ToUpper();
            string message = now + "<" + state + "> " + msg;
            Console.WriteLine(message);

            try
            {
                if (!di.Exists)
                    Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                    using (StreamWriter sw = new StreamWriter(logPath))
                        sw.WriteLine(message);
                else
                    using (StreamWriter sw = File.AppendText(logPath))
                        sw.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(now + "<ERROR> 로그 기록 오류: " + e.Message);
            }
        }
        #endregion
    }
}
