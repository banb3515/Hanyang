#region API ����
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
        #region ����
        // Logger - �α� ���
        public static ILogger Logger { get; set; }

        #region NEIS API
        private const string BASE_URL = "https://open.neis.go.kr/hub/hisTimetable?"; // API URL
        private const string API_KEY = "KEY=762281280e4943e58669a6b02991a67c&"; // API Ű
        private const string TYPE = "Type=json&"; // ������ Ÿ��
        private const string P_INDEX = "pIndex=1&"; // ������ ��ġ
        private const string P_SIZE = "pSize=1000&"; // ������ �� ��û ��
        private const string ATPT_OFCDC_SC_CODE = "ATPT_OFCDC_SC_CODE=B10&"; // �õ�����û�ڵ�: ����� ����û
        private const string SD_SCHUL_CODE = "SD_SCHUL_CODE=7010377&"; // ǥ���б��ڵ�: �Ѿ��������б�

        private static readonly string[] departments = new string[]
        { 
            "�Ǽ�������", 
            "�����",
            "�ڵ�ȭ����",
            "���������ڰ�",
            "�ڵ�����",
            "��ǻ�ͳ�Ʈ��ũ��"
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
                await GetTimetable();

                await GetLunchMenu();

                await GetAcademicSchedule();

                await Task.Delay(1800000); // 1000 = 1��, �⺻: 30��
            }
        }
        #endregion

        #region �ð�ǥ ��������
        private static async Task GetTimetable()
        {
            try
            {
                string AY = "AY=" + DateTime.Now.Year.ToString() + "&"; // �г⵵
                // �� ������(�ʹ� ����) �ð�ǥ�� �������� �ʱ� ���� 2�� �� ��¥���� �ð�ǥ ��������
                string TI_FROM_YMD = "TI_FROM_YMD=" + DateTime.Now.AddMonths(-2).ToString("yyyyMMdd") + "&"; // �ð�ǥ��������
                string TI_TO_YMD = "TI_TO_YMD="; // �ð�ǥ��������

                // �̹� �� �ݿ��� ��¥ ��������, ��/�Ͽ����� �� ���� �� �ݿ��� ��¥ ��������
                if (Convert.ToInt32(DateTime.Now.DayOfWeek) >= Convert.ToInt32(DayOfWeek.Saturday))
                    // �̹� ��
                    TI_TO_YMD += DateTime.Today.AddDays(7 + Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                else
                    // ���� ��
                    TI_TO_YMD += DateTime.Today.AddDays(Convert.ToInt32(DayOfWeek.Friday) - Convert.ToInt32(DateTime.Today.DayOfWeek)).ToString("yyyyMMdd");
                TI_TO_YMD += "&";

                // �г� �� ��ŭ �ݺ�: 1, 2, 3�г�
                for(var grade = 1; grade <= 3; grade ++)
                {
                    // �а� �� ��ŭ �ݺ�: �Ǽ�������, �����, �ڵ�ȭ����, ���������ڰ�, �ڵ�����, ��ǻ�ͳ�Ʈ��ũ��
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

                Logger.LogInformation("<Server> �ð�ǥ ��������: ����");
            }
            catch (Exception e)
            {
                Logger.LogError("<Server> �ð�ǥ ��������: ���� (" + e.Message + ")");
            }
        }
        #endregion

        #region �޽� �޴� ��������
        private static async Task GetLunchMenu()
        {

        }
        #endregion

        #region �л� ���� ��������
        private static async Task GetAcademicSchedule()
        {

        }
        #endregion
        #endregion
    }
}
