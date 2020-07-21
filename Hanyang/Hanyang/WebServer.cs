#region API 참조
using System;
using System.Threading.Tasks;
#endregion

namespace Hanyang
{
    public class WebServer
    {
        public static string GetJson(string type)
        {
            try
            {
                type = type.ToLower();
                var json = new System.Net.WebClient().DownloadString(App.ServerUrl + type + "/" + App.API_KEY);
                return json;
            }
            catch (Exception e)
            {
                MainPage.GetInstance().DisplayAlert("test", e.Message, "확인");
                return null;
            }
        }
    }
}