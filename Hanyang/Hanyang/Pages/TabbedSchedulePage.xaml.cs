#region API 참조
using Hanyang.Animations;
using Hanyang.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        private static TabbedSchedulePage instance;

        public static bool InitDataBool { get; set; } = false; // 데이터 초기화
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

            instance = this;
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
            }
            catch (Exception e)
            {
                DisplayAlert("시간표 오류", e.Message, "확인");
            }

            if (viewDOW == 6)
                viewDOW = 1;
            #endregion

            #region 학사 일정 시작 날짜 초기화

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
            if (App.Animation)
            {
                await b.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => b.BackgroundColor = c, 75);
                await b.ColorTo(Color.FromRgb(43, 43, 43), Color.White, c => b.TextColor = c, 50);
            }
            else
            {
                b.BackgroundColor = Color.FromRgb(78, 78, 78);
                b.TextColor = Color.White;
            }
        }
        #endregion

        #region viewDOW 요일 시간표 보기
        public async Task ViewScheduleAnimation(Dictionary<string, Timetable> arg = null)
        {
            Button button = null;
            List<Grid> grids = new List<Grid>();

            ScheduleView.IsVisible = true;
            Schedule.IsVisible = true;
            grids.Add(SchedulePeriod1);
            grids.Add(SchedulePeriod2);
            grids.Add(SchedulePeriod3);
            grids.Add(SchedulePeriod4);
            grids.Add(ScheduleLunch);
            grids.Add(SchedulePeriod5);
            grids.Add(SchedulePeriod6);
            grids.Add(SchedulePeriod7);

            foreach (Grid grid in grids)
                grid.IsVisible = false;
            Date.Opacity = 0;
            Description.Opacity = 0;

            Dictionary<string, Timetable> timetable = App.Timetable;

            if (arg != null)
                timetable = arg;

            var className = App.GetClassName();

            // 변수 추가
            switch (viewDOW)
            {
                case 1:
                    button = ViewSchedule1Button;
                    break;
                case 2:
                    button = ViewSchedule2Button;
                    break;
                case 3:
                    button = ViewSchedule3Button;
                    break;
                case 4:
                    button = ViewSchedule4Button;
                    break;
                case 5:
                    button = ViewSchedule5Button;
                    break;
            }

            var dow = "";
            switch (viewDOW)
            {
                case 1:
                    dow = "Monday";
                    break;
                case 2:
                    dow = "Tuesday";
                    break;
                case 3:
                    dow = "Wednesday";
                    break;
                case 4:
                    dow = "Thursday";
                    break;
                case 5:
                    dow = "Friday";
                    break;
            }

            if (App.Animation)
            {
                await button.ColorTo(Color.FromRgb(248, 248, 255), Color.FromRgb(78, 78, 78), c => button.BackgroundColor = c, 75);
                await button.ColorTo(Color.FromRgb(43, 43, 43), Color.White, c => button.TextColor = c, 50);
            }
            else
            {
                button.BackgroundColor = Color.FromRgb(78, 78, 78);
                button.TextColor = Color.White;
            }

            if (App.Animation)
                await Task.Delay(100);

            // 시간표 초기화
            ScheduleSubject1.Text = timetable[className].Data[dow]["1"];
            ScheduleSubject2.Text = timetable[className].Data[dow]["2"];
            ScheduleSubject3.Text = timetable[className].Data[dow]["3"];
            ScheduleSubject4.Text = timetable[className].Data[dow]["4"];
            ScheduleSubject5.Text = timetable[className].Data[dow]["5"];
            ScheduleSubject6.Text = timetable[className].Data[dow]["6"];
            if (timetable[className].Data[dow].ContainsKey("7"))
                ScheduleSubject7.Text = timetable[className].Data[dow]["7"];
            else
                grids.Remove(SchedulePeriod7);

            foreach (Grid grid in grids)
            {
                grid.IsVisible = true;
                if (App.Animation)
                {
                    await grid.TranslateTo(300, 0, 1, Easing.SpringOut);
                    _ = grid.TranslateTo(0, 0, 500, Easing.SpringOut);
                    await Task.Delay(150);
                }
            }

            Description.Text = "[" + className + "반 시간표]";
            Date.Text = DateTime.ParseExact(timetable[className].Date[dow], "yyyyMMdd", null).ToString("yyyy년 M월 d일") + " 이후 시간표입니다.";

            if (App.Animation)
            {
                await Date.TranslateTo(300, 0, 1, Easing.SpringOut);
                Date.Opacity = 1;
                _ = Date.TranslateTo(0, 0, 500, Easing.SpringOut);
                await Description.TranslateTo(300, 0, 1, Easing.SpringOut);
                Description.Opacity = 1;
                _ = Description.TranslateTo(0, 0, 500, Easing.SpringOut);
            }
            else
            {
                Description.Opacity = 1;
                Date.Opacity = 1;
            }
        }
        #endregion

        #region 급식 메뉴 보기
        private async Task ViewLunchMenuAnimation()
        {
            if (App.Animation)
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
                    if (App.Animation)
                        LunchMenuLine.ScaleX = 0;
                    LunchMenuLine.Opacity = 1;
                });

                await LunchMenuLine.ScaleXTo(1, 500, Easing.SpringIn);
                await LunchMenuList.TranslateTo(300, 0, 1, Easing.SpringOut);
                LunchMenuList.Opacity = 1;
                await LunchMenuList.TranslateTo(0, 0, 500, Easing.SpringOut);
            }
            else
            {
                LunchMenuDate.Opacity = 1;
                LunchMenuBackground.Opacity = 1;
                LunchMenuLine.Opacity = 1;
                LunchMenuImage.Opacity = 1;
                LunchMenuList.Opacity = 1;
                GestureDescription.Opacity = 1;
                LunchMenu.IsVisible = true;
            }
        }
        #endregion

        #region 학사 일정 보기
        private async Task ViewAcademicScheduleAnimation()
        {
            AcademicSchedule.IsVisible = true;
            if (App.Animation)
            {
                AcademicSchedule.Opacity = 0;
                await AcademicSchedule.FadeTo(1, 750, Easing.SpringIn);
            }
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
            if (!task && viewDOW != 1)
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

                await ViewScheduleAnimation();
                task = false;
            }
        }
        #endregion
        #endregion

        #region 함수
        #region 인스턴스 가져오기
        public static TabbedSchedulePage GetInstance()
        {
            return instance;
        }
        #endregion
        #endregion
    }
}