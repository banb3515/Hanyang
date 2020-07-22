#region API 참조
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Models;

using Newtonsoft.Json.Linq;

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
        public static ILogger Logger { get; set; } // Logger - 로그 기록

        // API KEY 값
        public const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";

        #region API 요청 값
        public static Dictionary<string, Timetable> Timetable { get; set; } // 시간표
        #endregion

        #region NEIS API
        private const string BASE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // API URL
        private const string NEIS_API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API 키
        private const string TYPE = "Type=json&"; // 데이터 타입
        private const string P_INDEX = "pIndex=1&"; // 페이지 위치
        private const string P_SIZE = "pSize=1000&"; // 페이지 당 신청 수
        private const string ATPT_OFCDC_SC_CODE = "ATPT_OFCDC_SC_CODE=B10&"; // 시도교육청코드: 서울시 교육청
        private const string SD_SCHUL_CODE = "SD_SCHUL_CODE=7010377&"; // 표준학교코드: 한양공업고등학교

        // 학과 목록
        private static readonly string[] departments = new string[]
        { 
            "건설정보과", 
            "건축과",
            "자동화기계과",
            "디지털전자과",
            "자동차과",
            "컴퓨터네트워크과"
        };

        // 유효한 반 이름
        private static readonly string[] validClassNames = new string[]
        {
            "건설",
            "건축",
            "기계",
            "전자",
            "자동차",
            "컴넷"
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
                Logger.LogInformation("<Server> 데이터 가져오기: 시간표를 가져옵니다.");
                GetTimetable(); // 시간표 가져오기

                Logger.LogInformation("<Server> 데이터 가져오기: 급식 메뉴를 가져옵니다.");
                GetLunchMenu(); // 급식 메뉴 가져오기

                Logger.LogInformation("<Server> 데이터 가져오기: 학사 일정을 가져옵니다.");
                GetAcademicSchedule(); // 학사 일정 가져오기

                await Task.Delay(1800000); // 1000 = 1초, 기본: 30분
            }
        }
        #endregion

        #region 시간표 가져오기
        private static void GetTimetable()
        {
            try
            {
                string AY = "AY=" + DateTime.Now.Year.ToString() + "&"; // 학년도
                // ↓ 오래된(너무 많은) 시간표를 가져오지 않기 위해 2달 전 날짜부터 시간표 가져오기
                string TI_FROM_YMD = "TI_FROM_YMD=" + DateTime.Now.AddMonths(-2).ToString("yyyyMMdd") + "&"; // 시간표시작일자
                string TI_TO_YMD = "TI_TO_YMD="; // 시간표종료일자

                // 이번 주 금요일 날짜 가져오기, 토/일요일일 때 다음 주 금요일 날짜 가져오기
                if (Convert.ToInt32(DateTime.Now.DayOfWeek) < Convert.ToInt32(DayOfWeek.Saturday))
                    // 이번 주
                    TI_TO_YMD += DateTime.Today.AddDays(Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                else
                    // 다음 주
                    TI_TO_YMD += DateTime.Today.AddDays(7 + Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                TI_TO_YMD += "&";

                // 가져온 시간표 데이터: string = 반
                var datas = new Dictionary<string, Timetable>();
                
                // 학년 수 만큼 반복: 1, 2, 3학년
                for(int grade = 1; grade <= 3; grade ++)
                {
                    // 학과 수 만큼 반복: 건설정보과, 건축과, 자동화기계과, 디지털전자과, 자동차과, 컴퓨터네트워크과
                    foreach (var dep in departments)
                    {
                        var json = new WebClient().DownloadString(
                            BASE_URL +
                            NEIS_API_KEY +
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
                        var timetable = JObject.Parse(json)["hisTimetable"];
                        var head = timetable.First["head"];
                        var row = timetable.Last["row"];

                        var dataSize = Convert.ToInt32(head.First["list_total_count"]);
                        var resultCode = head.Last["RESULT"]["CODE"].ToString();
                        var resultMsg = head.Last["RESULT"]["MESSAGE"].ToString();

                        if (resultCode == "INFO-000")
                        {
                            DateTime firstDate = DateTime.ParseExact(row[dataSize - 1]["ALL_TI_YMD"].ToString(), "yyyyMMdd", null); // 마지막(최신) 시간표 처음 날짜
                            Logger.LogInformation(firstDate.ToString("yyyyMMdd"));

                            if (firstDate.DayOfWeek == DayOfWeek.Friday)
                                firstDate = firstDate.AddDays(-4);
                            else
                                firstDate = firstDate.AddDays(-6);

                            // 임시 딕셔너리: 1 string = 반, 2 string = 요일, 3 string = 교시, 4 string = 과목
                            var dict = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                            // 임시 날짜 딕셔너리: 1 string = 반, 2 string = 요일, 3 string = 날짜
                            var dateDict = new Dictionary<string, Dictionary<string, string>>();

                            for(int i = 0; i < dataSize; i ++)
                            {
                                var data = row[i];
                                var date = data["ALL_TI_YMD"].ToString(); // 날짜 문자열 형식
                                var datetime = DateTime.ParseExact(date, "yyyyMMdd", null); // 날짜 DateTime 객체 형식

                                // 가져온 날짜가 마지막 시간표 처음 날짜와 같거나 클 경우
                                if (DateTime.Compare(datetime, firstDate) >= 0)
                                {
                                    var className = data["CLRM_NM"].ToString(); // 반 이름: ex) 2컴넷B
                                    var dow = datetime.DayOfWeek.ToString(); // 요일

                                    // 수준별반 등 기타 반은 제외
                                    if (Array.FindIndex(validClassNames, x => className.Contains(x)) == -1)
                                        continue;

                                    // 임시 딕셔너리 초기화
                                    if (!dict.ContainsKey(className))
                                    {
                                        dict.Add(className, new Dictionary<string, Dictionary<string, string>>());
                                        dict[className].Add("Monday", new Dictionary<string, string>());
                                        dict[className].Add("Tuesday", new Dictionary<string, string>());
                                        dict[className].Add("Wednesday", new Dictionary<string, string>());
                                        dict[className].Add("Thursday", new Dictionary<string, string>());
                                        dict[className].Add("Friday", new Dictionary<string, string>());
                                    }

                                    // 임시 날짜 딕셔너리 초기화
                                    if (!dateDict.ContainsKey(className))
                                        dateDict.Add(className, new Dictionary<string, string>());

                                    // 날짜 추가
                                    if (!dateDict[className].ContainsKey(dow))
                                        dateDict[className].Add(dow, date);

                                    var perio = data["PERIO"].ToString(); // 교시
                                    var subject = data["ITRT_CNTNT"].ToString(); // 과목

                                    // 과목 문자열에서 불필요한 문자 제거
                                    if (subject.Contains("* "))
                                        subject = subject.Replace("* ", "");

                                    // 중복되지 않은 값을 임시 딕셔너리에 추가
                                    if (!dict[className][dow].ContainsKey(perio))
                                        dict[className][dow].Add(perio, subject);
                                }
                            }

                            foreach(var className in dict.Keys)
                            {
                                datas.Add(className, new Timetable
                                {
                                    ResultCode = "000",
                                    ResultMsg = "정상 처리되었습니다.",
                                    Date = dateDict[className],
                                    Data = dict[className]
                                });
                            }

                            Logger.LogInformation("<Server> " + grade + "학년 " + dep + " 시간표 가져오기: 성공");
                        }
                        else
                            Logger.LogInformation("<Server> " + grade + "학년 " + dep + " 시간표 가져오기: 실패 - " + resultCode + " (" + resultMsg + ")");
                    }
                }
                Timetable = datas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> 시간표 가져오기: 오류 (" + e.Message + ")");
            }
        }
        #endregion

        #region 급식 메뉴 가져오기
        private static void GetLunchMenu()
        {

        }
        #endregion

        #region 학사 일정 가져오기
        private static void GetAcademicSchedule()
        {

        }
        #endregion
        #endregion
    }
}
