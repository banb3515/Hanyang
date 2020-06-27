#region API 참조
using Hanyang.Animations;
using Hanyang.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace Hanyang
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedSchedulePage : ContentPage
    {
        #region 변수
        private string view; // 현재 보고있는 레이아웃
        private int viewDOW; // 현재 보고있는 요일 레이아웃
        private bool task; // 다른 작업 중인지 확인
        private List<string> rainbowColors; // 여러 색상
        #endregion

        #region 생성자
        public TabbedSchedulePage()
        {
            #region 변수 초기화
            view = string.Empty;
            task = false;

            rainbowColors = new List<string>
            {
                "#FF0000",
                "#FF5E00",
                "#FFBB00",
                "#47C83E",
                "#0054FF",
                "#6B66FF",
                "#8041D9",
                "#D941C5",
                "#FF007F",
                "#CC723D"
            };
            #endregion

            InitializeComponent();

            #region 현재 요일을 가져와 해당요일 시간표 보여주기
            var now = DateTime.Now;

            switch (now.DayOfWeek)
            {
                // 월요일
                case DayOfWeek.Monday:
                    viewDOW = 1;
                    break;
                // 화요일
                case DayOfWeek.Tuesday:
                    viewDOW = 2;
                    break;
                // 수요일
                case DayOfWeek.Wednesday:
                    viewDOW = 3;
                    break;
                // 목요일
                case DayOfWeek.Thursday:
                    viewDOW = 4;
                    break;
                // 금요일
                case DayOfWeek.Friday:
                    viewDOW = 5;
                    break;
                // 주말
                default:
                    viewDOW = 6;
                    break;
            }

            try
            {
                if (Int16.Parse(now.ToString("HH")) >= 18 && viewDOW < 6)
                    viewDOW += 1;
                Debug.WriteLine(now.ToString("HH"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR");
                Debug.WriteLine(e.Message);
            }

            if (viewDOW == 6)
                viewDOW = 1;

            _ = ViewScheduleAnimation();
            #endregion

            #region 학사 일정 시작 날짜 초기화
            
            #endregion

            #region 임시
            Schedule1Subject1.Text = "과학";
            Schedule1Subject2.Text = "수학";
            Schedule1Subject3.Text = "영어";
            Schedule1Subject4.Text = "일본어";
            Schedule1Subject5.Text = "프로그래밍";
            Schedule1Subject6.Text = "프로그래밍";
            Schedule1Subject7.Text = "프로그래밍";

            Schedule2Subject1.Text = "화면구현";
            Schedule2Subject2.Text = "화면구현";
            Schedule2Subject3.Text = "과학";
            Schedule2Subject4.Text = "체육";
            Schedule2Subject5.Text = "미술";
            Schedule2Subject6.Text = "한국사";
            Schedule2Subject7.Text = "수학";

            Schedule3Subject1.Text = "영어";
            Schedule3Subject2.Text = "일본어";
            Schedule3Subject3.Text = "진로";
            Schedule3Subject4.Text = "국어";
            Schedule3Subject5.Text = "봉사활동";
            Schedule3Subject6.Text = "동아리";

            Schedule4Subject1.Text = "공업";
            Schedule4Subject2.Text = "자료구조";
            Schedule4Subject3.Text = "자료구조";
            Schedule4Subject4.Text = "한국사";
            Schedule4Subject5.Text = "체육";
            Schedule4Subject6.Text = "프로그래밍";
            Schedule4Subject7.Text = "프로그래밍";

            Schedule5Subject1.Text = "수학";
            Schedule5Subject2.Text = "과학";
            Schedule5Subject3.Text = "공업";
            Schedule5Subject4.Text = "일본어";
            Schedule5Subject5.Text = "국어";
            Schedule5Subject6.Text = "체육";
            Schedule5Subject7.Text = "자율";

            Schedule1Time1.Text = "[8:30 - 9:20]";
            Schedule1Time2.Text = "[9:30 - 10:20]";
            Schedule1Time3.Text = "[10:30 - 11:20]";
            Schedule1Time4.Text = "[11:30 - 12:20]";
            Schedule1TimeLunch.Text = "[12:20 - 13:20]";
            Schedule1Time5.Text = "[13:20 - 14:10]";
            Schedule1Time6.Text = "[14:20 - 15:10]";
            Schedule1Time7.Text = "[15:20 - 16:10]";

            Schedule2Time1.Text = "[8:30 - 9:20]";
            Schedule2Time2.Text = "[9:30 - 10:20]";
            Schedule2Time3.Text = "[10:30 - 11:20]";
            Schedule2Time4.Text = "[11:30 - 12:20]";
            Schedule2TimeLunch.Text = "[12:20 - 13:20]";
            Schedule2Time5.Text = "[13:20 - 14:10]";
            Schedule2Time6.Text = "[14:20 - 15:10]";
            Schedule2Time7.Text = "[15:20 - 16:10]";

            Schedule3Time1.Text = "[8:30 - 9:20]";
            Schedule3Time2.Text = "[9:30 - 10:20]";
            Schedule3Time3.Text = "[10:30 - 11:20]";
            Schedule3Time4.Text = "[11:30 - 12:20]";
            Schedule3TimeLunch.Text = "[12:20 - 13:20]";
            Schedule3Time5.Text = "[13:20 - 14:10]";
            Schedule3Time6.Text = "[14:20 - 15:10]";

            Schedule4Time1.Text = "[8:30 - 9:20]";
            Schedule4Time2.Text = "[9:30 - 10:20]";
            Schedule4Time3.Text = "[10:30 - 11:20]";
            Schedule4Time4.Text = "[11:30 - 12:20]";
            Schedule4TimeLunch.Text = "[12:20 - 13:20]";
            Schedule4Time5.Text = "[13:20 - 14:10]";
            Schedule4Time6.Text = "[14:20 - 15:10]";
            Schedule4Time7.Text = "[15:20 - 16:10]";

            Schedule5Time1.Text = "[8:30 - 9:20]";
            Schedule5Time2.Text = "[9:30 - 10:20]";
            Schedule5Time3.Text = "[10:30 - 11:20]";
            Schedule5Time4.Text = "[11:30 - 12:20]";
            Schedule5TimeLunch.Text = "[12:20 - 13:20]";
            Schedule5Time5.Text = "[13:20 - 14:10]";
            Schedule5Time6.Text = "[14:20 - 15:10]";
            Schedule5Time7.Text = "[15:20 - 16:10]";
            #endregion

            List<LunchMenu> lunches = new List<LunchMenu>
            {
                new LunchMenu() { Symbol = rainbowColors[0], Food = "쌀밥"},
                new LunchMenu() { Symbol = rainbowColors[1], Food = "소고기고추장찌개"},
                new LunchMenu() { Symbol = rainbowColors[2], Food = "생선튀김&청초D"},
                new LunchMenu() { Symbol = rainbowColors[3], Food = "찐만두"},
                new LunchMenu() { Symbol = rainbowColors[4], Food = "부지깽이나물"},
                new LunchMenu() { Symbol = rainbowColors[5], Food = "깍두기"},
            };

            LunchMenuList.ItemsSource = lunches;
        }
        #endregion

        #region 애니메이션
        #region 보기 버튼 클릭
        private async void ViewButtonAnimation(Button b)
        {
            await b.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => b.BackgroundColor = c, 75);
            await b.ColorTo(Color.FromRgb(78, 78, 78), Color.FromRgb(248, 248, 255), c => b.TextColor = c, 50);
        }
        #endregion

        #region viewDOW 요일 시간표 보기
        private async Task ViewScheduleAnimation()
        {
            Description1.Opacity = 0;
            Description2.Opacity = 0;

            Button button = null;
            StackLayout layout = null;
            List<Grid> grids = new List<Grid>();

            #region 변수 추가
            switch (viewDOW)
            {
                case 1:
                    button = ViewSchedule1Button;
                    layout = Schedule1;
                    grids.Add(Schedule1Period1);
                    grids.Add(Schedule1Period2);
                    grids.Add(Schedule1Period3);
                    grids.Add(Schedule1Period4);
                    grids.Add(Schedule1Lunch);
                    grids.Add(Schedule1Period5);
                    grids.Add(Schedule1Period6);
                    grids.Add(Schedule1Period7);
                    break;
                case 2:
                    button = ViewSchedule2Button;
                    layout = Schedule2;
                    grids.Add(Schedule2Period1);
                    grids.Add(Schedule2Period2);
                    grids.Add(Schedule2Period3);
                    grids.Add(Schedule2Period4);
                    grids.Add(Schedule2Lunch);
                    grids.Add(Schedule2Period5);
                    grids.Add(Schedule2Period6);
                    grids.Add(Schedule2Period7);
                    break;
                case 3:
                    button = ViewSchedule3Button;
                    layout = Schedule3;
                    grids.Add(Schedule3Period1);
                    grids.Add(Schedule3Period2);
                    grids.Add(Schedule3Period3);
                    grids.Add(Schedule3Period4);
                    grids.Add(Schedule3Lunch);
                    grids.Add(Schedule3Period5);
                    grids.Add(Schedule3Period6);
                    break;
                case 4:
                    button = ViewSchedule4Button;
                    layout = Schedule4;
                    grids.Add(Schedule4Period1);
                    grids.Add(Schedule4Period2);
                    grids.Add(Schedule4Period3);
                    grids.Add(Schedule4Period4);
                    grids.Add(Schedule4Lunch);
                    grids.Add(Schedule4Period5);
                    grids.Add(Schedule4Period6);
                    grids.Add(Schedule4Period7);
                    break;
                case 5:
                    button = ViewSchedule5Button;
                    layout = Schedule5;
                    grids.Add(Schedule5Period1);
                    grids.Add(Schedule5Period2);
                    grids.Add(Schedule5Period3);
                    grids.Add(Schedule5Period4);
                    grids.Add(Schedule5Lunch);
                    grids.Add(Schedule5Period5);
                    grids.Add(Schedule5Period6);
                    grids.Add(Schedule5Period7);
                    break;
            }
            #endregion

            await button.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => button.BackgroundColor = c, 75);
            await button.ColorTo(Color.FromRgb(43, 43, 43), Color.White, c => button.TextColor = c, 50);

            foreach (Grid grid in grids)
                grid.IsVisible = false;
            layout.IsVisible = true;
            Schedule.IsVisible = true;

            await Task.Delay(100);

            foreach (Grid grid in grids)
            {
                await grid.TranslateTo(300, 0, 1, Easing.SpringOut);
                grid.IsVisible = true;
                _ = grid.TranslateTo(0, 0, 500, Easing.SpringOut);
                await Task.Delay(150);
            }

            await Description1.TranslateTo(300, 0, 1, Easing.SpringOut);
            Description1.Opacity = 1;
            _ = Description1.TranslateTo(0, 0, 500, Easing.SpringOut);
            await Description2.TranslateTo(300, 0, 1, Easing.SpringOut);
            Description2.Opacity = 1;
            _ = Description2.TranslateTo(0, 0, 500, Easing.SpringOut);
        }
        #endregion

        #region 급식 메뉴 보기
        private async Task ViewLunchMenuAnimation()
        {
            await Task.Delay(250);

            Device.BeginInvokeOnMainThread(() =>
            {
                LunchMenuDate.Opacity = 0;
                LunchMenuBackground.Opacity = 0;
                LunchMenuLine.Opacity = 0;
                LunchMenuImage.Opacity = 0;
                LunchMenuList.Opacity = 0;
                GestureDescription.Opacity = 0;
                LunchMenu.IsVisible = true;
            });
            
            await LunchMenuDate.FadeTo(1, 750, Easing.SpringOut);
            _ = LunchMenuBackground.FadeTo(1, 1500, Easing.SpringOut);
            await Task.Delay(150);

            await LunchMenuImage.FadeTo(1, 750, Easing.SpringIn);
            await Task.Delay(100);
            await GestureDescription.FadeTo(1, 500, Easing.SpringOut);
            await Task.Delay(100);

            Device.BeginInvokeOnMainThread(() =>
            {
                LunchMenuLine.ScaleX = 0;
                LunchMenuLine.Opacity = 1;
            });
            await LunchMenuLine.ScaleXTo(1, 500, Easing.SpringIn);

            await LunchMenuList.TranslateTo(300, 0, 1, Easing.SpringOut);
            LunchMenuList.Opacity = 1;
            await LunchMenuList.TranslateTo(0, 0, 500, Easing.SpringOut);
        }
        #endregion

        #region 학사 일정 보기
        private async Task ViewAcademicScheduleAnimation()
        {
            AcademicSchedule.IsVisible = true;
            AcademicSchedule.Opacity = 0;

            await AcademicSchedule.FadeTo(1, 750, Easing.SpringIn);
        }
        #endregion
        #endregion

        #region 버튼 클릭
        #region 시간표 보기 버튼
        private async void ViewScheduleButton_Clicked(object sender, System.EventArgs e)
        {
            if (!task && view != "schedule")
            {
                task = true;
                view = "schedule";

                ViewLunchMenuButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewAcademicScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewLunchMenuButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewAcademicScheduleButton.TextColor = Color.FromRgb(43, 43, 43);

                LunchMenu.IsVisible = false;
                AcademicSchedule.IsVisible = false;

                ViewButtonAnimation(sender as Button);
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 급식 메뉴 보기 버튼
        private async void ViewLunchMenuButton_Clicked(object sender, System.EventArgs e)
        {
            if (!task && view != "lunch_menu")
            {
                task = true;
                view = "lunch_menu";

                ViewScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewAcademicScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewScheduleButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewAcademicScheduleButton.TextColor = Color.FromRgb(43, 43, 43);

                Schedule.IsVisible = false;
                AcademicSchedule.IsVisible = false;

                ViewButtonAnimation(sender as Button);
                await ViewLunchMenuAnimation();
                task = false;
            }
        }
        #endregion

        #region 학사 일정 보기 버튼
        private async void ViewAcademicScheduleButton_Clicked(object sender, System.EventArgs e)
        {
            if (!task && view != "academic_schedule")
            {
                task = true;
                view = "academic_schedule";

                ViewScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewLunchMenuButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewScheduleButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewLunchMenuButton.TextColor = Color.FromRgb(43, 43, 43);

                Schedule.IsVisible = false;
                LunchMenu.IsVisible = false;

                ViewButtonAnimation(sender as Button);
                await ViewAcademicScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 월요일 보기 버튼
        private async void ViewSchedule1Button_Clicked(object sender, System.EventArgs e)
        {
            if(!task && viewDOW != 1)
            {
                task = true;
                viewDOW = 1;
                ViewButtonAnimation(sender as Button);

                ViewSchedule2Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule3Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule4Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule5Button.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSchedule2Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule3Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule4Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule5Button.TextColor = Color.FromRgb(43, 43, 43);

                Schedule2.IsVisible = false;
                Schedule3.IsVisible = false;
                Schedule4.IsVisible = false;
                Schedule5.IsVisible = false;
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 화요일 보기 버튼
        private async void ViewSchedule2Button_Clicked(object sender, System.EventArgs e)
        {
            if (!task && viewDOW != 2)
            {
                task = true;
                viewDOW = 2;
                ViewButtonAnimation(sender as Button);

                ViewSchedule1Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule3Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule4Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule5Button.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSchedule1Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule3Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule4Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule5Button.TextColor = Color.FromRgb(43, 43, 43);

                Schedule1.IsVisible = false;
                Schedule3.IsVisible = false;
                Schedule4.IsVisible = false;
                Schedule5.IsVisible = false;
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 수요일 보기 버튼
        private async void ViewSchedule3Button_Clicked(object sender, System.EventArgs e)
        {
            if (!task && viewDOW != 3)
            {
                task = true;
                viewDOW = 3;
                ViewButtonAnimation(sender as Button);

                ViewSchedule1Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule2Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule4Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule5Button.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSchedule1Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule2Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule4Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule5Button.TextColor = Color.FromRgb(43, 43, 43);

                Schedule1.IsVisible = false;
                Schedule2.IsVisible = false;
                Schedule4.IsVisible = false;
                Schedule5.IsVisible = false;
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 목요일 보기 버튼
        private async void ViewSchedule4Button_Clicked(object sender, System.EventArgs e)
        {
            if (!task && viewDOW != 4)
            {
                task = true;
                viewDOW = 4;
                ViewButtonAnimation(sender as Button);

                ViewSchedule1Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule2Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule3Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule5Button.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSchedule1Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule2Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule3Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule5Button.TextColor = Color.FromRgb(43, 43, 43);

                Schedule1.IsVisible = false;
                Schedule2.IsVisible = false;
                Schedule3.IsVisible = false;
                Schedule5.IsVisible = false;
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion

        #region 금요일 보기 버튼
        private async void ViewSchedule5Button_Clicked(object sender, System.EventArgs e)
        {
            if (!task && viewDOW != 5)
            {
                task = true;
                viewDOW = 5;
                ViewButtonAnimation(sender as Button);

                ViewSchedule1Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule2Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule3Button.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewSchedule4Button.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewSchedule1Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule2Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule3Button.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchedule4Button.TextColor = Color.FromRgb(43, 43, 43);

                Schedule1.IsVisible = false;
                Schedule2.IsVisible = false;
                Schedule3.IsVisible = false;
                Schedule4.IsVisible = false;
                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion
        #endregion
    }
}