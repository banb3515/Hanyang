#region API 참조
using Hanyang.Models;

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
        public ArticlePage(string title, int id)
        {
            #region 변수 초기화
            task = false;
            #endregion

            InitializeComponent();

            #region 글 가져오기
            Article article = null;
            
            switch (title)
            {
                case "공지사항":
                    article = App.GetNotices()[id];
                    break;
                case "가정통신문":
                    article = App.GetSchoolNewsletters()[id];
                    break;
                case "앱 공지사항":
                    article = App.GetAppNotices()[id];
                    break;
            }
            #endregion

            #region UI 설정
            Title = title;

            ArticleId.Text = "[" + id.ToString() + "]";
            ArticleTitle.Text = article.Title;
            ArticleWriter.Text = article.Info.Split('|')[0];
            ArticleDate.Text = article.Info.Split('|')[1];
            ArticleContents.Text = article.Contents;
            #endregion

            if (article.Attachments.Count < 1)
                Attachment.IsVisible = false;
            else
            {
                AttachmentList.ItemsSource = article.Attachments;
                if (article.Attachments.Count > 1)
                    AttachmentList.HeightRequest = 75;
                Attachment.IsVisible = true;
            }
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