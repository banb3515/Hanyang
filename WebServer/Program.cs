#region API ����
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
        #region ����
        public static ILogger Logger { get; set; } // Logger - �α� ���

        // API KEY ��
        public const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";

        #region API ��û ��
        public static Dictionary<string, Timetable> Timetable { get; set; } // �ð�ǥ
        #endregion

        #region NEIS API
        private const string BASE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // API URL
        private const string NEIS_API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API Ű
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

            while(true)
            {
                Logger.LogInformation("<Server> ������ ��������: �ð�ǥ�� �����ɴϴ�.");
                GetTimetable(); // �ð�ǥ ��������

                Logger.LogInformation("<Server> ������ ��������: �޽� �޴��� �����ɴϴ�.");
                GetLunchMenu(); // �޽� �޴� ��������

                Logger.LogInformation("<Server> ������ ��������: �л� ������ �����ɴϴ�.");
                GetAcademicSchedule(); // �л� ���� ��������

                await Task.Delay(1800000); // 1000 = 1��, �⺻: 30��
            }
        }
        #endregion

        #region �ð�ǥ ��������
        private static void GetTimetable()
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
                            DateTime firstDate = DateTime.ParseExact(row[dataSize - 1]["ALL_TI_YMD"].ToString(), "yyyyMMdd", null); // ������(�ֽ�) �ð�ǥ ó�� ��¥
                            Logger.LogInformation(firstDate.ToString("yyyyMMdd"));

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
                                    ResultCode = "000",
                                    ResultMsg = "���� ó���Ǿ����ϴ�.",
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
                Timetable = datas;
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> �ð�ǥ ��������: ���� (" + e.Message + ")");
            }
        }
        #endregion

        #region �޽� �޴� ��������
        private static void GetLunchMenu()
        {

        }
        #endregion

        #region �л� ���� ��������
        private static void GetAcademicSchedule()
        {

        }
        #endregion
        #endregion
    }
}
