#region API 참조
using ByteSizeLib;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace WebServer
{
    public class Program
    {
        #region 변수
        public static ILogger Logger { get; set; } // Logger - 로그 기록

        private const string FCM_TOKEN = "AAAAypSgklo:APA91bHqNp_jlDP9BOUohMYfszAAygCrg9Kc0ONhNNJp_41wHb0WOpDMX8gDSgdUrq8wT8xou0whdflUdba4ma1-ZMTwfVTjWXUMLhnzsPFsR1ODsBSxXv7MhUwAaTAkGCzWNERhUoSZ";

        // API KEY 값
        public const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";

        #region API 요청 값
        public static Dictionary<string, string> AppInfo { get; set; } // 앱 정보

        public static Dictionary<string, Dictionary<string, string>> DataInfo { get; set; } // 데이터 정보

        public static Dictionary<string, Timetable> Timetable { get; set; } // 시간표

        public static LunchMenu LunchMenu { get; set; } // 급식 메뉴

        public static Dictionary<string, SchoolSchedule> SchoolSchedule { get; set; } // 학사 일정

        public static Dictionary<string, Dictionary<string, string>> SchoolNotice { get; set; } // 학교 공지사항

        public static Dictionary<string, Dictionary<string, string>> SchoolNewsletter { get; set; } // 가정통신문

        public static Dictionary<string, Dictionary<string, string>> AppNotice { get; set; } // 앱 공지사항
        #endregion

        #region NEIS API
        private const string NEIS_API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API 키
        private const string TIMETABLE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // 시간표 API URL
        private const string LUNCH_MENU_URL = "https://open.neis.go.kr/hub/mealServiceDietInfo?"; // 급식 일정 API URL
        private const string SCHOOL_SCHEDULE_URL = "https://open.neis.go.kr/hub/SchoolSchedule?"; // 학교 일정 API URL
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

            AppInfo = new Dictionary<string, string>
            {
                { "Version", "1.0" }, // 앱 버전
                { "UpdateContent", "☞ 한양이 앱이 출시되었습니다." } // 업데이트 내용
            };

            DataInfo = new Dictionary<string, Dictionary<string, string>>();

            AppNotice = new Dictionary<string, Dictionary<string, string>>();
            AppNotice.Add("1", new Dictionary<string, string> 
            {
                { "Name", "한양이" },
                { "Date", "2020-08-01" },
                { "Title", "한양이 앱 테스트 버전" },
                { "Content", "<p>한양이 앱 테스트 버전입니다.</p>" },
            });

            DataInfo.Add("AppNotice", new Dictionary<string, string> 
            {
                { "LastUpdate", DateTime.Now.ToString() },
                { "Size", ByteSize.FromBytes(GetJsonByteLength(AppNotice)).ToString() }
            });

            Thread getDataThread = new Thread(new ThreadStart(GetData));
            getDataThread.Start();
            Logger.LogInformation("<Server> 데이터 가져오기 Thread 실행");

            Thread crawlingThread = new Thread(new ThreadStart(GetCrawling));
            crawlingThread.Start();
            Logger.LogInformation("<Server> 학교 홈페이지 크롤링 Thread 실행");

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
            DataInfo.Add("LunchMenu", new Dictionary<string, string>());
            DataInfo.Add("SchoolSchedule", new Dictionary<string, string>());

            while (true)
            {
                try
                {
                    Logger.LogInformation("<Server> 데이터 가져오기: 시간표를 가져옵니다.");
                    var tmpTimetable = GetTimetable(); // 시간표 가져오기

                    Logger.LogInformation("<Server> 데이터 가져오기: 급식 메뉴를 가져옵니다.");
                    var tmpLunchMenu = GetLunchMenu(); // 급식 메뉴 가져오기

                    Logger.LogInformation("<Server> 데이터 가져오기: 학사 일정을 가져옵니다.");
                    var tmpSchoolSchedule = GetSchoolSchedule(); // 학사 일정 가져오기

                    if (tmpTimetable != null)
                    {
                        // 시간표 데이터 값의 변화가 있는지 확인
                        if (Timetable != null)
                        {
                            if (!JsonCompare(tmpTimetable, Timetable))
                            {
                                Timetable = tmpTimetable;

                                foreach (var className in tmpTimetable.Keys)
                                {
                                    DataInfo["Timetable-" + className].Remove("LastUpdate");
                                    DataInfo["Timetable-" + className].Add("LastUpdate", DateTime.Now.ToString());

                                    DataInfo["Timetable-" + className].Remove("Size");
                                    DataInfo["Timetable-" + className].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpTimetable[className])).ToString());
                                }
                            }
                        }
                        else
                        {
                            Timetable = tmpTimetable;

                            foreach (var className in tmpTimetable.Keys)
                            {
                                DataInfo.Add("Timetable-" + className, new Dictionary<string, string>());

                                DataInfo["Timetable-" + className].Add("LastUpdate", DateTime.Now.ToString());
                                DataInfo["Timetable-" + className].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpTimetable[className])).ToString());
                            }
                        }
                    }

                    if (tmpLunchMenu != null)
                    {
                        // 급식 메뉴 데이터 값의 변화가 있는지 확인
                        if (LunchMenu != null)
                        {
                            if (!JsonCompare(tmpLunchMenu, LunchMenu))
                            {
                                LunchMenu = tmpLunchMenu;

                                DataInfo["LunchMenu"].Remove("LastUpdate");
                                DataInfo["LunchMenu"].Add("LastUpdate", DateTime.Now.ToString());

                                DataInfo["LunchMenu"].Remove("Size");
                                DataInfo["LunchMenu"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpLunchMenu)).ToString());
                            }
                        }
                        else
                        {
                            LunchMenu = tmpLunchMenu;

                            DataInfo["LunchMenu"].Add("LastUpdate", DateTime.Now.ToString());
                            DataInfo["LunchMenu"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpLunchMenu)).ToString());
                        }
                    }

                    if (tmpSchoolSchedule != null)
                    {
                        // 학사 일정 데이터 값의 변화가 있는지 확인
                        if (SchoolSchedule != null)
                        {
                            if (!JsonCompare(tmpSchoolSchedule, SchoolSchedule))
                            {
                                SchoolSchedule = tmpSchoolSchedule;

                                DataInfo["SchoolSchedule"].Remove("LastUpdate");
                                DataInfo["SchoolSchedule"].Add("LastUpdate", DateTime.Now.ToString());

                                DataInfo["SchoolSchedule"].Remove("Size");
                                DataInfo["SchoolSchedule"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolSchedule)).ToString());
                            }
                        }
                        else
                        {
                            SchoolSchedule = tmpSchoolSchedule;

                            DataInfo["SchoolSchedule"].Add("LastUpdate", DateTime.Now.ToString());
                            DataInfo["SchoolSchedule"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolSchedule)).ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("<Server> 데이터 가져오기: 오류 (" + e.Message + ")");
                    continue;
                }

                await Task.Delay(1800000); // 1000 = 1초, 기본: 30분 (1800000)
            }
        }
        #endregion

        #region 시간표 가져오기
        private static Dictionary<string, Timetable> GetTimetable()
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
                            TIMETABLE_URL +
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
                return datas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> 시간표 가져오기: 오류 (" + e.Message + ")");
            }
            return Timetable;
        }
        #endregion

        #region 급식 메뉴 가져오기
        private static LunchMenu GetLunchMenu()
        {
            try
            {
                string MLSV_YMD = "MLSV_YMD=" + DateTime.Now.ToString("yyyyMM") + "&"; // 급식 일자

                var json = new WebClient().DownloadString(
                            LUNCH_MENU_URL +
                            NEIS_API_KEY +
                            TYPE +
                            P_INDEX +
                            P_SIZE +
                            ATPT_OFCDC_SC_CODE +
                            SD_SCHUL_CODE +
                            MLSV_YMD);

                var lunchMenu = JObject.Parse(json)["mealServiceDietInfo"];

                var head = lunchMenu.First["head"];
                var row = lunchMenu.Last["row"];

                var dataSize = Convert.ToInt32(head.First["list_total_count"]);
                var resultCode = head.Last["RESULT"]["CODE"].ToString();
                var resultMsg = head.Last["RESULT"]["MESSAGE"].ToString();

                if (resultCode == "INFO-000")
                {
                    var lunchMenuData = new LunchMenu
                    {
                        Data = new Dictionary<string, List<string>>()
                    };

                    for(int i = 0; i < dataSize; i ++)
                    {
                        var data = row[i];
                        var date = data["MLSV_YMD"].ToString(); // 날짜 문자열 형식
                        var tmpMenus = new List<string>(data["DDISH_NM"].ToString().Split("<br/>")); // <br/>을 기준으로 잘라 임시 리스트에 저장
                        var menus = new List<string>(); // 급식 메뉴 리스트 생성

                        // 메뉴에 있는 숫자(알레르기 표시) 제거
                        foreach(var tmp in tmpMenus)
                        {
                            var regex = new Regex(@"[0-9]"); // 정규식: 숫자 포함
                            var menu = tmp;

                            for (var j = 0; j < tmp.Length; j ++)
                            {
                                // 메뉴에 숫자가 포함되어 있을 때
                                if(regex.IsMatch(tmp[j].ToString()))
                                {
                                    // 그 뒤에 문자 모두 제거
                                    menu = tmp.Substring(0, tmp.IndexOf(tmp[j]));
                                    break;
                                }
                            }
                            menus.Add(menu); // 리스트에 추가
                        }

                        lunchMenuData.Data.Add(date, menus); // 급식 메뉴 데이터에 추가
                    }

                    Logger.LogInformation("<Server> 급식 메뉴 가져오기: 성공");
                    return lunchMenuData;
                }
                else
                    Logger.LogInformation("<Server> 급식 메뉴 가져오기: 실패 - " + resultCode + " (" + resultMsg + ")");
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> 급식 메뉴 가져오기: 오류 (" + e.Message + ")");
            }
            return LunchMenu;
        }
        #endregion

        #region 학사 일정 가져오기
        private static Dictionary<string, SchoolSchedule> GetSchoolSchedule()
        {
            try
            {
                int year = DateTime.Now.Year;

                var ssDatas = new Dictionary<string, SchoolSchedule>();

                // 1월부터 다음 년도 2월까지 반복
                for (int i = 1; i <= 14; i++)
                {
                    int month = i;

                    // 12월 이상(다음 년도 1월) 일 때
                    if (month > 12)
                    {
                        year++;
                        month -= 12;
                    }

                    var AA_YMD = "AA_YMD=" + year + month.ToString().PadLeft(2, '0') + "&";

                    var json = new WebClient().DownloadString(
                        SCHOOL_SCHEDULE_URL +
                        NEIS_API_KEY +
                        TYPE +
                        P_INDEX +
                        P_SIZE +
                        ATPT_OFCDC_SC_CODE +
                        SD_SCHUL_CODE +
                        AA_YMD);

                    var schoolSchedule = JObject.Parse(json)["SchoolSchedule"];
                    // {month}월 데이터가 없을 때
                    if (schoolSchedule == null)
                        continue;
                    var head = schoolSchedule.First["head"];
                    var row = schoolSchedule.Last["row"];

                    var dataSize = Convert.ToInt32(head.First["list_total_count"]);
                    var resultCode = head.Last["RESULT"]["CODE"].ToString();
                    var resultMsg = head.Last["RESULT"]["MESSAGE"].ToString();

                    if (resultCode == "INFO-000")
                    {
                        var ssData = new SchoolSchedule { Data = new Dictionary<string, List<string>>() };

                        for (int j = 0; j < dataSize; j++)
                        {
                            var data = row[j];
                            var day = data["AA_YMD"].ToString().Substring(6);
                            var list = new List<string>();

                            if (ssData.Data.ContainsKey(day))
                            {
                                list = ssData.Data[day];
                                ssData.Data.Remove(day);
                            }

                            list.Add(data["EVENT_NM"].ToString().Trim());
                            ssData.Data.Add(day, list);
                        }
                        ssDatas.Add(year + month.ToString().PadLeft(2, '0'), ssData);

                        Logger.LogInformation("<Server> " + year + "년 " + month + "월 학사 일정 가져오기: 성공");
                    }
                    else
                        Logger.LogInformation("<Server> " + year + "년 " + month + "월 학사 일정 가져오기: 실패 - " + resultCode + " (" + resultMsg + ")");
                }
                return ssDatas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> 학사 일정 가져오기: 오류 (" + e.Message + ")");
            }
            return SchoolSchedule;
        }
        #endregion

        #region 학교 홈페이지 크롤링
        private static async void GetCrawling()
        {
            DataInfo.Add("SchoolNotice", new Dictionary<string, string>());
            DataInfo.Add("SchoolNewsletter", new Dictionary<string, string>());

            while (true)
            {
                try
                {
                    Logger.LogInformation("<Server> 학교 홈페이지 크롤링: 학교 공지사항을 가져옵니다.");
                    var tmpSchoolNotice = GetSchoolNotice();

                    await Task.Delay(500);

                    Logger.LogInformation("<Server> 가정통신문 크롤링: 학교 공지사항을 가져옵니다.");
                    var tmpSchoolNewsletter = GetSchoolNewsletter();

                    if (tmpSchoolNotice != null)
                    {
                        // 학교 공지사항 데이터 값의 변화가 있는지 확인
                        if (SchoolNotice != null)
                        {
                            if (!JsonCompare(tmpSchoolNotice, SchoolNotice))
                            {
                                SchoolNotice = tmpSchoolNotice;

                                DataInfo["SchoolNotice"].Remove("LastUpdate");
                                DataInfo["SchoolNotice"].Add("LastUpdate", DateTime.Now.ToString());

                                DataInfo["SchoolNotice"].Remove("Size");
                                DataInfo["SchoolNotice"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolNotice)).ToString());
                            }
                        }
                        else
                        {
                            SchoolNotice = tmpSchoolNotice;

                            DataInfo["SchoolNotice"].Add("LastUpdate", DateTime.Now.ToString());
                            DataInfo["SchoolNotice"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolNotice)).ToString());
                        }
                    }

                    if (tmpSchoolNewsletter != null)
                    {
                        // 가정통신문 데이터 값의 변화가 있는지 확인
                        if (SchoolNewsletter != null)
                        {
                            if (!JsonCompare(tmpSchoolNewsletter, SchoolNewsletter))
                            {
                                SchoolNewsletter = tmpSchoolNewsletter;

                                DataInfo["SchoolNewsletter"].Remove("LastUpdate");
                                DataInfo["SchoolNewsletter"].Add("LastUpdate", DateTime.Now.ToString());

                                DataInfo["SchoolNewsletter"].Remove("Size");
                                DataInfo["SchoolNewsletter"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolNewsletter)).ToString());
                            }
                        }
                        else
                        {
                            SchoolNewsletter = tmpSchoolNewsletter;

                            DataInfo["SchoolNewsletter"].Add("LastUpdate", DateTime.Now.ToString());
                            DataInfo["SchoolNewsletter"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolNewsletter)).ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("<Server> 학교 홈페이지 크롤링: 오류 (" + e.Message + ")");
                    continue;
                }

                await Task.Delay(1800000); // 1000 = 1초, 기본: 30분 (1800000)
            }
        }
        #endregion

        #region 학교 공지사항 가져오기
        private static Dictionary<string, Dictionary<string, string>> GetSchoolNotice()
        {
            try
            {
                var options = new ChromeOptions
                {
                    PlatformName = PlatformType.Windows.ToString(),
                };
                options.AddArgument("headless");
                options.AddArgument("no-sandbox");
                options.AddArgument("window-size=1600,900");
                options.AddArgument("proxy-server='direct://'");
                options.AddArgument("proxy-bypass-list=*");

                var driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
                driver.Navigate().GoToUrl("http://hanyang.sen.hs.kr/index.do");

                var datas = new Dictionary<string, Dictionary<string, string>>();

                var flag = true;
                var page = 1;
                var index = 1;

                driver.FindElementByXPath("//*[@id=\"baseFrm_200483\"]/div/p/a").Click(); // 공지사항 보기 버튼 클릭

                while (flag)
                {
                    for (var line = 1; line <= 20; line++)
                    {
                        try
                        {
                            if (page != 1)
                                driver.FindElementByXPath("//*[@id=\"board_area\"]/div[4]/a[" + (page + 2) + "]").Click();

                            var checkNotice = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[1]");

                            if (checkNotice.Text == "공지")
                                continue;

                            var articleDate = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[4]");

                            if (DateTime.Now.Year > Convert.ToInt32(articleDate.Text.Substring(0, 4)))
                            {
                                flag = false;
                                break;
                            }

                            driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[2]/a").Click();
                            var name = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[1]/td[1]/div").Text;
                            var date = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[1]/td[2]/div").Text;
                            var title = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[2]/td/div").Text;
                            var content = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[3]/td/div").GetAttribute("innerHTML").Trim();

                            var data = new Dictionary<string, string>
                            {
                                { "Name", name },
                                { "Date", date },
                                { "Title", title },
                                { "Content", content }
                            };

                            Logger.LogInformation("<Server> 학교 홈페이지 크롤링: 학교 공지사항 (" + title + ")");

                            datas.Add(index++.ToString(), data);

                            driver.Navigate().Back();
                            driver.FindElementByXPath("//*[@id=\"baseFrm_200483\"]/div/p/a").Click(); // 공지사항 보기 버튼 클릭
                        }
                        catch (NoSuchElementException)
                        {
                            break;
                        }
                    }

                    if (flag)
                        page++;
                }

                driver.Close();
                driver.Quit();

                Logger.LogInformation("<Server> 학교 공지사항 가져오기: 성공");
                return datas;
            }
            catch (Exception e)
            {
                Logger.LogInformation("<Server> 학교 공지사항 가져오기: 오류 (" + e.Message + ")");
            }
            return SchoolNotice;
        }
        #endregion

        #region 가정통신문 가져오기
        private static Dictionary<string, Dictionary<string, string>> GetSchoolNewsletter()
        {
            try
            {
                var options = new ChromeOptions
                {
                    PlatformName = PlatformType.Windows.ToString(),
                };
                options.AddArgument("headless");
                options.AddArgument("no-sandbox");
                options.AddArgument("window-size=1600,900");
                options.AddArgument("proxy-server='direct://'");
                options.AddArgument("proxy-bypass-list=*");

                var driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
                driver.Navigate().GoToUrl("http://hanyang.sen.hs.kr/index.do");

                var datas = new Dictionary<string, Dictionary<string, string>>();

                var flag = true;
                var page = 1;
                var index = 1;

                driver.FindElementByXPath("//*[@id=\"baseFrm_200484\"]/div/p/a").Click(); // 가정통신문 보기 버튼 클릭

                while (flag)
                {
                    for (var line = 1; line <= 20; line++)
                    {
                        try
                        {
                            if (page != 1)
                                driver.FindElementByXPath("//*[@id=\"board_area\"]/div[4]/a[" + (page + 2) + "]").Click();

                            var checkNotice = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[1]");

                            if (checkNotice.Text == "공지")
                                continue;

                            var articleDate = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[4]");

                            if (DateTime.Now.Year > Convert.ToInt32(articleDate.Text.Substring(0, 4)))
                            {
                                flag = false;
                                break;
                            }

                            driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[" + line + "]/td[2]/a").Click();
                            var name = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[1]/td[1]/div").Text;
                            var date = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[1]/td[2]/div").Text;
                            var title = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[2]/td/div").Text;
                            var content = driver.FindElementByXPath("//*[@id=\"board_area\"]/table/tbody/tr[3]/td/div").GetAttribute("innerHTML").Trim();

                            var data = new Dictionary<string, string>
                            {
                                { "Name", name },
                                { "Date", date },
                                { "Title", title },
                                { "Content", content }
                            };

                            Logger.LogInformation("<Server> 학교 홈페이지 크롤링: 가정통신문 (" + title + ")");

                            datas.Add(index++.ToString(), data);

                            driver.Navigate().Back();
                            driver.FindElementByXPath("//*[@id=\"baseFrm_200484\"]/div/p/a").Click(); // 가정통신문 보기 버튼 클릭
                        }
                        catch (NoSuchElementException)
                        {
                            break;
                        }
                    }

                    if (flag)
                        page++;
                }

                driver.Close();
                driver.Quit();

                Logger.LogInformation("<Server> 가정통신문 가져오기: 성공");
                return datas;
            }
            catch (Exception e)
            {
                Logger.LogInformation("<Server> 가정통신문 가져오기: 오류 (" + e.Message + ")");
            }
            return SchoolNewsletter;
        }
        #endregion

        #region 급식 알림 보내기
        #endregion

        #region Json 비교
        private static bool JsonCompare(object obj1, object obj2)
        {
            try
            {
                if (ReferenceEquals(obj1, obj2)) return true;
                if ((obj1 == null) || (obj2 == null)) return false;
                if (obj1.GetType() != obj2.GetType()) return false;

                var objJson = JsonConvert.SerializeObject(obj1);
                var anotherJson = JsonConvert.SerializeObject(obj2);

                return objJson == anotherJson;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> Json 데이터 비교: 오류 (" + e.Message + ")");
            }
            return false;
        }
        #endregion

        #region Json 바이트 크기 가져오기
        private static int GetJsonByteLength(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);

                byte[] byteArr;
                byteArr = Encoding.UTF8.GetBytes(json);
                return byteArr.Length;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> Json 데이터 바이트 크기 가져오기: 오류 (" + e.Message + ")");
                return 0;
            }
        }
        #endregion
        #endregion
    }
}
