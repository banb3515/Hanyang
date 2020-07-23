#region API 참조
using System;
using System.Threading.Tasks;
#endregion

namespace Hanyang
{
    public class WebServer
    {
        public static async Task<string> GetJson(string type)
        {
            try
            {
                type = type.ToLower();
                var json = new System.Net.WebClient().DownloadString(App.ServerUrl + type + "/" + App.API_KEY);
                return json;
            }
            catch (Exception e)
            {
                await MainPage.GetInstance().ErrorAlert("데이터 다운로드", "서버에서 데이터를 다운로드 하는 도중 오류가 발생했습니다.\n" + e.Message);
                return null;
            }
        }
    }
}