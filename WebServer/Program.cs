#region API ����
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
        #region ����
        public static ILogger Logger { get; set; } // Logger - �α� ���

        // API KEY ��
        public const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";

        #region API ��û ��
        public static Dictionary<string, Dictionary<string, string>> DataInfo { get; set; } // ������ ����

        public static Dictionary<string, Timetable> Timetable { get; set; } // �ð�ǥ

        public static LunchMenu LunchMenu { get; set; } // �޽� �޴�

        public static Dictionary<string, SchoolSchedule> SchoolSchedule { get; set; } // �л� ����

        public static Dictionary<string, Dictionary<string, string>> SchoolNotice { get; set; } // �б� ��������
        #endregion

        #region NEIS API
        private const string NEIS_API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API Ű
        private const string TIMETABLE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // �ð�ǥ API URL
        private const string LUNCH_MENU_URL = "https://open.neis.go.kr/hub/mealServiceDietInfo?"; // �޽� ���� API URL
        private const string SCHOOL_SCHEDULE_URL = "https://open.neis.go.kr/hub/SchoolSchedule?"; // �б� ���� API URL
        private const string TYPE = "Type=json&"; // ������ Ÿ��
        private const string P_INDEX = "pIndex=1&"; // ������ ��ġ
        private const string P_SIZE = "pSize=1000&"; // ������ �� ��û ��
        private const string ATPT_OFCDC_SC_CODE = "ATPT_OFCDC_SC_CODE=B10&"; // �õ�����û�ڵ�: ����� ����û
        private const string SD_SCHUL_CODE = "SD_SCHUL_CODE=7010377&"; // ǥ���б��ڵ�: �Ѿ��������б�

        // �а� ���
        private static readonly string[] departments = new string[]
        { 
            "�Ǽ�������", 
            "�����",
            "�ڵ�ȭ����",
            "���������ڰ�",
            "�ڵ�����",
            "��ǻ�ͳ�Ʈ��ũ��"
        };

        // ��ȿ�� �� �̸�
        private static readonly string[] validClassNames = new string[]
        {
            "�Ǽ�",
            "����",
            "���",
            "����",
            "�ڵ���",
            "�ĳ�"
        };
        #endregion
        #endregion

        #region Main
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Logger = host.Services.GetRequiredService<ILogger<Program>>();
            Logger.LogInformation("<Server> �� ���� ����");

            Thread getDataThread = new Thread(new ThreadStart(GetData));
            getDataThread.Start();

            Thread crawlingThread = new Thread(new ThreadStart(GetCrawling));
            crawlingThread.Start();

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

        #region �Լ�
        #region ������ ��������
        private static async void GetData()
        {
            Logger.LogInformation("<Server> ������ �������� Thread ����");

            DataInfo = new Dictionary<string, Dictionary<string, string>>();
            DataInfo.Add("LunchMenu", new Dictionary<string, string>());
            DataInfo.Add("SchoolSchedule", new Dictionary<string, string>());

            while (true)
            {
                Logger.LogInformation("<Server> ������ ��������: �ð�ǥ�� �����ɴϴ�.");
                var tmpTimetable = GetTimetable(); // �ð�ǥ ��������

                Logger.LogInformation("<Server> ������ ��������: �޽� �޴��� �����ɴϴ�.");
                var tmpLunchMenu = GetLunchMenu(); // �޽� �޴� ��������

                Logger.LogInformation("<Server> ������ ��������: �л� ������ �����ɴϴ�.");
                var tmpSchoolSchedule = GetSchoolSchedule(); // �л� ���� ��������

                // �ð�ǥ ������ ���� ��ȭ�� �ִ��� Ȯ��
                if (Timetable != null)
                {
                    if (!JsonCompare(tmpTimetable, Timetable))
                    {
                        foreach(var className in tmpTimetable.Keys)
                        {
                            DataInfo["Timetable-" + className].Remove("LastUpdate");
                            DataInfo["Timetable-" + className].Add("LastUpdate", DateTime.Now.ToString());

                            DataInfo["Timetable-" + className].Remove("Size");
                            DataInfo["Timetable-" + className].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpTimetable[className])).ToString());
                        }

                        Timetable = tmpTimetable;
                    }
                }
                else
                {
                    foreach (var className in tmpTimetable.Keys)
                    {
                        DataInfo.Add("Timetable-" + className, new Dictionary<string, string>());

                        DataInfo["Timetable-" + className].Add("LastUpdate", DateTime.Now.ToString());
                        DataInfo["Timetable-" + className].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpTimetable[className])).ToString());
                    }

                    Timetable = tmpTimetable;
                }

                // �޽� �޴� ������ ���� ��ȭ�� �ִ��� Ȯ��
                if (LunchMenu != null)
                {
                    if (!JsonCompare(tmpLunchMenu, LunchMenu))
                    {
                        DataInfo["LunchMenu"].Remove("LastUpdate");
                        DataInfo["LunchMenu"].Add("LastUpdate", DateTime.Now.ToString());

                        DataInfo["LunchMenu"].Remove("Size");
                        DataInfo["LunchMenu"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpLunchMenu)).ToString());

                        LunchMenu = tmpLunchMenu;
                    }
                }
                else
                {
                    DataInfo["LunchMenu"].Add("LastUpdate", DateTime.Now.ToString());
                    DataInfo["LunchMenu"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpLunchMenu)).ToString());

                    LunchMenu = tmpLunchMenu;
                }

                // �л� ���� ������ ���� ��ȭ�� �ִ��� Ȯ��
                if (SchoolSchedule != null)
                {
                    if (!JsonCompare(tmpSchoolSchedule, SchoolSchedule))
                    {
                        DataInfo["SchoolSchedule"].Remove("LastUpdate");
                        DataInfo["SchoolSchedule"].Add("LastUpdate", DateTime.Now.ToString());

                        DataInfo["SchoolSchedule"].Remove("Size");
                        DataInfo["SchoolSchedule"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolSchedule)).ToString());

                        SchoolSchedule = tmpSchoolSchedule;
                    }
                }
                else
                {
                    DataInfo["SchoolSchedule"].Add("LastUpdate", DateTime.Now.ToString());
                    DataInfo["SchoolSchedule"].Add("Size", ByteSize.FromBytes(GetJsonByteLength(tmpSchoolSchedule)).ToString());

                    SchoolSchedule = tmpSchoolSchedule;
                }

                await Task.Delay(1800000); // 1000 = 1��, �⺻: 30�� (1800000)
            }
        }
        #endregion

        #region �ð�ǥ ��������
        private static Dictionary<string, Timetable> GetTimetable()
        {
            try
            {
                string AY = "AY=" + DateTime.Now.Year.ToString() + "&"; // �г⵵
                // �� ������(�ʹ� ����) �ð�ǥ�� �������� �ʱ� ���� 2�� �� ��¥���� �ð�ǥ ��������
                string TI_FROM_YMD = "TI_FROM_YMD=" + DateTime.Now.AddMonths(-2).ToString("yyyyMMdd") + "&"; // �ð�ǥ��������
                string TI_TO_YMD = "TI_TO_YMD="; // �ð�ǥ��������

                // �̹� �� �ݿ��� ��¥ ��������, ��/�Ͽ����� �� ���� �� �ݿ��� ��¥ ��������
                if (Convert.ToInt32(DateTime.Now.DayOfWeek) < Convert.ToInt32(DayOfWeek.Saturday))
                    // �̹� ��
                    TI_TO_YMD += DateTime.Today.AddDays(Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                else
                    // ���� ��
                    TI_TO_YMD += DateTime.Today.AddDays(7 + Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                TI_TO_YMD += "&";

                // ������ �ð�ǥ ������: string = ��
                var datas = new Dictionary<string, Timetable>();
                
                // �г� �� ��ŭ �ݺ�: 1, 2, 3�г�
                for(int grade = 1; grade <= 3; grade ++)
                {
                    // �а� �� ��ŭ �ݺ�: �Ǽ�������, �����, �ڵ�ȭ����, ���������ڰ�, �ڵ�����, ��ǻ�ͳ�Ʈ��ũ��
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
                            DateTime firstDate = DateTime.ParseExact(row[dataSize - 1]["ALL_TI_YMD"].ToString(), "yyyyMMdd", null); // ������(�ֽ�) �ð�ǥ ó�� ��¥

                            if (firstDate.DayOfWeek == DayOfWeek.Friday)
                                firstDate = firstDate.AddDays(-4);
                            else
                                firstDate = firstDate.AddDays(-6);

                            // �ӽ� ��ųʸ�: 1 string = ��, 2 string = ����, 3 string = ����, 4 string = ����
                            var dict = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                            // �ӽ� ��¥ ��ųʸ�: 1 string = ��, 2 string = ����, 3 string = ��¥
                            var dateDict = new Dictionary<string, Dictionary<string, string>>();

                            for(int i = 0; i < dataSize; i ++)
                            {
                                var data = row[i];
                                var date = data["ALL_TI_YMD"].ToString(); // ��¥ ���ڿ� ����
                                var datetime = DateTime.ParseExact(date, "yyyyMMdd", null); // ��¥ DateTime ��ü ����

                                // ������ ��¥�� ������ �ð�ǥ ó�� ��¥�� ���ų� Ŭ ���
                                if (DateTime.Compare(datetime, firstDate) >= 0)
                                {
                                    var className = data["CLRM_NM"].ToString(); // �� �̸�: ex) 2�ĳ�B
                                    var dow = datetime.DayOfWeek.ToString(); // ����

                                    // ���غ��� �� ��Ÿ ���� ����
                                    if (Array.FindIndex(validClassNames, x => className.Contains(x)) == -1)
                                        continue;

                                    // �ӽ� ��ųʸ� �ʱ�ȭ
                                    if (!dict.ContainsKey(className))
                                    {
                                        dict.Add(className, new Dictionary<string, Dictionary<string, string>>());
                                        dict[className].Add("Monday", new Dictionary<string, string>());
                                        dict[className].Add("Tuesday", new Dictionary<string, string>());
                                        dict[className].Add("Wednesday", new Dictionary<string, string>());
                                        dict[className].Add("Thursday", new Dictionary<string, string>());
                                        dict[className].Add("Friday", new Dictionary<string, string>());
                                    }

                                    // �ӽ� ��¥ ��ųʸ� �ʱ�ȭ
                                    if (!dateDict.ContainsKey(className))
                                        dateDict.Add(className, new Dictionary<string, string>());

                                    // ��¥ �߰�
                                    if (!dateDict[className].ContainsKey(dow))
                                        dateDict[className].Add(dow, date);

                                    var perio = data["PERIO"].ToString(); // ����
                                    var subject = data["ITRT_CNTNT"].ToString(); // ����

                                    // ���� ���ڿ����� ���ʿ��� ���� ����
                                    if (subject.Contains("* "))
                                        subject = subject.Replace("* ", "");

                                    // �ߺ����� ���� ���� �ӽ� ��ųʸ��� �߰�
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

                            Logger.LogInformation("<Server> " + grade + "�г� " + dep + " �ð�ǥ ��������: ����");
                        }
                        else
                            Logger.LogInformation("<Server> " + grade + "�г� " + dep + " �ð�ǥ ��������: ���� - " + resultCode + " (" + resultMsg + ")");
                    }
                }
                return datas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> �ð�ǥ ��������: ���� (" + e.Message + ")");
            }
            return Timetable;
        }
        #endregion

        #region �޽� �޴� ��������
        private static LunchMenu GetLunchMenu()
        {
            try
            {
                string MLSV_YMD = "MLSV_YMD=" + DateTime.Now.ToString("yyyyMM") + "&"; // �޽� ����

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
                        var date = data["MLSV_YMD"].ToString(); // ��¥ ���ڿ� ����
                        var tmpMenus = new List<string>(data["DDISH_NM"].ToString().Split("<br/>")); // <br/>�� �������� �߶� �ӽ� ����Ʈ�� ����
                        var menus = new List<string>(); // �޽� �޴� ����Ʈ ����

                        // �޴��� �ִ� ����(�˷����� ǥ��) ����
                        foreach(var tmp in tmpMenus)
                        {
                            var regex = new Regex(@"[0-9]"); // ���Խ�: ���� ����
                            var menu = tmp;

                            for (var j = 0; j < tmp.Length; j ++)
                            {
                                // �޴��� ���ڰ� ���ԵǾ� ���� ��
                                if(regex.IsMatch(tmp[j].ToString()))
                                {
                                    // �� �ڿ� ���� ��� ����
                                    menu = tmp.Substring(0, tmp.IndexOf(tmp[j]));
                                    break;
                                }
                            }
                            menus.Add(menu); // ����Ʈ�� �߰�
                        }

                        lunchMenuData.Data.Add(date, menus); // �޽� �޴� �����Ϳ� �߰�
                    }

                    Logger.LogInformation("<Server> �޽� �޴� ��������: ����");
                    return lunchMenuData;
                }
                else
                    Logger.LogInformation("<Server> �޽� �޴� ��������: ���� - " + resultCode + " (" + resultMsg + ")");
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> �޽� �޴� ��������: ���� (" + e.Message + ")");
            }
            return LunchMenu;
        }
        #endregion

        #region �л� ���� ��������
        private static Dictionary<string, SchoolSchedule> GetSchoolSchedule()
        {
            try
            {
                int year = DateTime.Now.Year;

                var ssDatas = new Dictionary<string, SchoolSchedule>();

                // 1������ ���� �⵵ 2������ �ݺ�
                for (int i = 1; i <= 14; i++)
                {
                    int month = i;

                    // 12�� �̻�(���� �⵵ 1��) �� ��
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
                    // {month}�� �����Ͱ� ���� ��
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

                        Logger.LogInformation("<Server> " + year + "�� " + month + "�� �л� ���� ��������: ����");
                    }
                    else
                        Logger.LogInformation("<Server> " + year + "�� " + month + "�� �л� ���� ��������: ���� - " + resultCode + " (" + resultMsg + ")");
                }
                return ssDatas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> �л� ���� ��������: ���� (" + e.Message + ")");
            }
            return SchoolSchedule;
        }
        #endregion

        #region �б� Ȩ������ ũ�Ѹ�
        private static async void GetCrawling()
        {
            while (true)
            {
                try
                {
                    var driverService = ChromeDriverService.CreateDefaultService();
                    driverService.HideCommandPromptWindow = true;
                    
                    var options = new ChromeOptions();
                    options.AddArgument("--headless");
                    options.AddArgument("window-size=1600,900");

                    var driver = new ChromeDriver(driverService, options);
                    driver.Navigate().GoToUrl("http://hanyang.sen.hs.kr/index.do");
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    var datas = new Dictionary<string, Dictionary<string, string>>();

                    var flag = true;
                    var page = 1;
                    var index = 1;

                    driver.FindElementByXPath("//*[@id=\"baseFrm_200483\"]/div/p/a").Click(); // �������� ���� ��ư Ŭ��

                    while (flag)
                    {
                        if (page != 1)
                            driver.FindElementByXPath("//*[@id=\"board_area\"]/div[4]/a[" + (page + 2) + "]").Click();

                        for (var line = 2; line <= 11; line++)
                        {
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

                            Logger.LogInformation("[" + title + "] �������� ���� -\n" + content);

                            datas.Add(index++.ToString(), data);

                            driver.Navigate().Back();
                            driver.FindElementByXPath("//*[@id=\"baseFrm_200483\"]/div/p/a").Click(); // �������� ���� ��ư Ŭ��
                        }

                        if (flag)
                            page++;
                    }

                    driver.Close();
                    driver.Quit();

                    Logger.LogInformation("<Server> �б� �������� ��������: ����");
                    SchoolNotice = datas;
                }
                catch (Exception e)
                {
                    Logger.LogInformation("<Server> �б� �������� ��������: ���� (" + e.Message + ")");
                }

                await Task.Delay(1800000); // 1000 = 1��, �⺻: 30�� (1800000)
            }
        }
        #endregion

        #region Json ��
        private static bool JsonCompare(object obj1, object obj2)
        {
            if (ReferenceEquals(obj1, obj2)) return true;
            if ((obj1 == null) || (obj2 == null)) return false;
            if (obj1.GetType() != obj2.GetType()) return false;

            var objJson = JsonConvert.SerializeObject(obj1);
            var anotherJson = JsonConvert.SerializeObject(obj2);

            return objJson == anotherJson;
        }
        #endregion

        #region Json ����Ʈ ũ�� ��������
        private static int GetJsonByteLength(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            byte[] byteArr;
            byteArr = Encoding.UTF8.GetBytes(json);
            return byteArr.Length;
        }
        #endregion
        #endregion
    }
}
