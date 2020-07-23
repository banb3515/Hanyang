using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelfDiagnosisPage : ContentPage
    {
        private bool isLoading;

        public SelfDiagnosisPage()
        {
            isLoading = false;

            InitializeComponent();

            WebView.Source = "https://eduro.sen.go.kr/stv_cvd_co00_002.do";
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (!isLoading)
            {
                isLoading = true;

                string birthDate = (DateTime.Now.Year - (App.Grade + 15)).ToString().Substring(2) +
                    App.BirthMonth.ToString().PadLeft(2, '0') + 
                    App.BirthDay.ToString().PadLeft(2, '0');

                string script = "document.getElementById(\"schulNm\").value = \"한양공업고등학교\";" +
                    "document.getElementById(\"schulCode\").value = \"B100000601\";" +
                    $"document.getElementById(\"pName\").value = \"{App.Name}\";" +
                    $"document.getElementById(\"frnoRidno\").value = \"{birthDate}\";" +
                    "document.getElementById(\"btnConfirm\").click();";
                await WebView.EvaluateJavaScriptAsync(script);
            }
        }
    }
}