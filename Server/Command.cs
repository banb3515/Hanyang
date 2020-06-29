#region API 참조
using System;
#endregion

namespace Server
{
    public class Command
    {
        #region 생성자
        public Command(string cmd)
        {
            switch (cmd)
            {
                #region help
                case "help":
                    Program.Log("\n===== 명령어 확인 =====\n" +
                        " # clear : 출력된 로그를 지웁니다. (파일은 유지)");
                    break;
                #endregion

                #region clear
                case "clear":
                    Console.Clear();
                    Program.Log("Clear : 출력된 로그를 지웠습니다.");
                    break;
                #endregion

                #region 잘못된 명령어
                default:
                    Program.Log("잘못된 명령어입니다. (help 명령어 이용)");
                    break;
                    #endregion
            }
        }
        #endregion
    }
}