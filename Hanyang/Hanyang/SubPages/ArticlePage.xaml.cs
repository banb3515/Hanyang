#region API 참조
using Hanyang.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang.SubPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePage : ContentPage
    {
        #region 변수
        private bool task; // 다른 작업 중인지 확인
        #endregion

        #region 생성자
        public ArticlePage(string type, int id)
        {
            #region 변수 초기화
            task = false;
            #endregion

            InitializeComponent();

            #region 글 가져오기
            Dictionary<string, string> article = null;

            var title = "";

            switch (type)
            {
                case "SchoolNotice":
                    article = App.SchoolNotice[id.ToString()];
                    title = "공지사항";
                    break;
                case "가정통신문":
                    //article = App.GetSchoolNewsletters()[id];
                    title = "가정통신문";
                    break;
                case "앱 공지사항":
                    //article = App.GetAppNotices()[id];
                    title = "앱 공지사항";
                    break;
            }
            #endregion

            #region UI 설정
            Title = title;

            ArticleId.Text = "[" + id.ToString() + "]";
            ArticleTitle.Text = article["Title"];
            ArticleWriter.Text = article["Name"];
            ArticleDate.Text = article["Date"];
            ArticleContents.Text = article["Content"];
            #endregion

            //if (article.Attachments.Count < 1)
            //    Attachment.IsVisible = false;
            //else
            //{
            //    AttachmentList.ItemsSource = article.Attachments;
            //    if (article.Attachments.Count > 1)
            //        AttachmentList.HeightRequest = 75;
            //    Attachment.IsVisible = true;
            //}
        }
        #endregion

        #region 버튼 클릭
        #region 맨 위로 이동 버튼
        private async void TopButton_Clicked(object sender, System.EventArgs e)
        {
            if (!task)
            {
                task = true;
                await Scroll.ScrollToAsync(0, 0, true);
                task = false;
            }
        }
        #endregion
        #endregion
    }
}