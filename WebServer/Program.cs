#region API 참조
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace WebServer
{
    public class Program
    {
        #region 변수
        // Logger - 로그 기록
        public static ILogger Logger { get; set; }

        #region NEIS API
        private const string BASE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // API URL
        private const string API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API 키
        private const string TYPE = "Type=json&"; // 데이터 타입
        private const string P_INDEX = "pIndex=1&"; // 페이지 위치
        private const string P_SIZE = "pSize=1000&"; // 페이지 당 신청 수
        private const string ATPT_OFCDC_SC_CODE = "ATPT_OFCDC_SC_CODE=B10&"; // 시도교육청코드: 서울시 교육청
        private const string SD_SCHUL_CODE = "SD_SCHUL_CODE=7010377&"; // 표준학교코드: 한양공업고등학교

        private static readonly string[] departments = new string[]
        { 
            "건설정보과", 
            "건축과",
            "자동화기계과",
            "디지털전자과",
            "자동차과",
            "컴퓨터네트워크과"
        };
        #endregion
        #endregion

        #region Main
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Logger = host.Services.GetRequiredService<ILogger<Program>>();
            Logger.LogInformation("<Server> 웹 서버 실행");

            Thread getDataThread = new Thread(new ThreadStart(GetData));
            getDataThread.Start();

            host.Run();
        }
        #endregion

        #region HostBuilder
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        #endregion

        #region 함수
        #region 데이터 가져오기
        private static async void GetData()
        {
            Logger.LogInformation("<Server> 데이터 가져오기 Thread 실행");

            while(true)
            {
                await GetTimetable();

                await GetLunchMenu();

                await GetAcademicSchedule();

                await Task.Delay(1800000); // 1000 = 1초, 기본: 30분
            }
        }
        #endregion

        #region 시간표 가져오기
        private static async Task GetTimetable()
        {
            try
            {
                string AY = "AY=" + DateTime.Now.Year.ToString() + "&"; // 학년도
                // ↓ 오래된(너무 많은) 시간표를 가져오지 않기 위해 2달 전 날짜부터 시간표 가져오기
                string TI_FROM_YMD = "TI_FROM_YMD=" + DateTime.Now.AddMonths(-2).ToString("yyyyMMdd") + "&"; // 시간표시작일자
                string TI_TO_YMD = "TI_TO_YMD="; // 시간표종료일자

                // 이번 주 금요일 날짜 가져오기, 토/일요일일 때 다음 주 금요일 날짜 가져오기
                if (Convert.ToInt32(DateTime.Now.DayOfWeek) >= Convert.ToInt32(DayOfWeek.Saturday))
                    // 이번 주
                    TI_TO_YMD += DateTime.Today.AddDays(7 + Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                else
                    // 다음 주
                    TI_TO_YMD += DateTime.Today.AddDays(Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                TI_TO_YMD += "&";

                // 학년 수 만큼 반복: 1, 2, 3학년
                for(var grade = 1; grade <= 3; grade ++)
                {
                    // 학과 수 만큼 반복: 건설정보과, 건축과, 자동화기계과, 디지털전자과, 자동차과, 컴퓨터네트워크과
                    foreach (var dep in departments)
                    {
                        var json = new WebClient().DownloadString(
                            BASE_URL +
                            API_KEY + 
                            TYPE + 
                            P_INDEX + 
                            P_SIZE +
                            ATPT_OFCDC_SC_CODE + 
                            SD_SCHUL_CODE + 
                            AY + 
                            TI_FROM_YMD + 
                            TI_TO_YMD + 
                            "GRADE=" + grade.ToString() + "&" +
                            "DDDEP_NM=" + dep);
                        Logger.LogInformation(json);
                        Console.WriteLine(json);
                    }
                }

                Logger.LogInformation("<Server> 시간표 가져오기: 성공");
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> 시간표 가져오기: 오류 (" + e.Message + ")");
            }
        }
        #endregion

        #region 급식 메뉴 가져오기
        private static async Task GetLunchMenu()
        {

        }
        #endregion

        #region 학사 일정 가져오기
        private static async Task GetAcademicSchedule()
        {

        }
        #endregion
        #endregion
    }
}
