#region API 참조
using Hanyang.Animations;
using Hanyang.Interface;
using Hanyang.Models;

using Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Syncfusion.SfCalendar.XForms;

using ByteSizeLib;
#endregion

namespace Hanyang.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedSchedulePage : ContentPage
    {
        #region 변수
        public string view; // 현재 보고있는 레이아웃
        private int viewDOW; // 현재 보고있는 요일 레이아웃
        public bool task; // 다른 작업 중인지 확인
        private List<string> rainbowColors; // 여러 색상
        private static TabbedSchedulePage instance;

        public static bool InitDataBool { get; set; } = false; // 데이터 초기화
        #endregion

        #region 생성자
        public TabbedSchedulePage()
        {
            #region 변수 초기화
            view = "schedule";
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
                _ = MainPage.GetInstance().ErrorAlert("시간 정수 변환", "시간을 정수형으로 변환하는 도중 오류가 발생했습니다.\n" + e.Message);
            }

            if (viewDOW == 6)
                viewDOW = 1;
            #endregion
        }
        #endregion

        #region 애니메이션
        #region 이미지 버튼 클릭
        private async Task ImageButtonAnimation(ImageButton b)
        {
            await b.ScaleTo(0.8, 150, Easing.SinOut);
            await b.ScaleTo(1, 100, Easing.SinIn);
        }
        #endregion

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

        #region 시간표 보기
        public async Task ViewScheduleAnimation(Timetable arg = null)
        {
            Button button = null;

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

            if (App.Grade == 0 || App.Class == 0)
            {
                DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                    "프로필 설정을 완료해주세요.");
                task = false;
                return;
            }

            if (App.Timetable == null)
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    return;

                MainPage.GetInstance().GetTimetable();
            }

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

            Timetable timetable = App.Timetable;

            if (arg != null)
                timetable = arg;

            var className = App.GetClassName();

            var dow = ((DayOfWeek)viewDOW).ToString();

            if (App.Animation)
                await Task.Delay(100);

            // 시간표 초기화
            if (timetable.Data[dow].ContainsKey("1"))
            {
                ScheduleSubject1.Text = timetable.Data[dow]["1"];
                if (ScheduleSubject1.Text.Length > 15)
                    ScheduleSubject1.Text = ScheduleSubject1.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject1.Text.Substring(15).Trim();
                if (timetable.Data[dow].ContainsKey("2"))
                {
                    ScheduleSubject2.Text = timetable.Data[dow]["2"];
                    if (ScheduleSubject2.Text.Length > 15)
                        ScheduleSubject2.Text = ScheduleSubject2.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject2.Text.Substring(15).Trim();
                    if (timetable.Data[dow].ContainsKey("3"))
                    {
                        ScheduleSubject3.Text = timetable.Data[dow]["3"];
                        if (ScheduleSubject3.Text.Length > 15)
                            ScheduleSubject3.Text = ScheduleSubject3.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject3.Text.Substring(15).Trim();
                        if (timetable.Data[dow].ContainsKey("4"))
                        {
                            ScheduleSubject4.Text = timetable.Data[dow]["4"];
                            if (ScheduleSubject4.Text.Length > 15)
                                ScheduleSubject4.Text = ScheduleSubject4.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject4.Text.Substring(15).Trim();
                            if (timetable.Data[dow].ContainsKey("5"))
                            {
                                ScheduleSubject5.Text = timetable.Data[dow]["5"];
                                if (ScheduleSubject5.Text.Length > 15)
                                    ScheduleSubject5.Text = ScheduleSubject5.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject5.Text.Substring(15).Trim();
                                if (timetable.Data[dow].ContainsKey("6"))
                                {
                                    ScheduleSubject6.Text = timetable.Data[dow]["6"];
                                    if (ScheduleSubject6.Text.Length > 15)
                                        ScheduleSubject6.Text = ScheduleSubject6.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject6.Text.Substring(15).Trim();
                                    if (timetable.Data[dow].ContainsKey("7"))
                                    {
                                        ScheduleSubject7.Text = timetable.Data[dow]["7"];
                                        if (ScheduleSubject7.Text.Length > 15)
                                            ScheduleSubject7.Text = ScheduleSubject7.Text.Substring(0, 15).Trim() + "\n" + ScheduleSubject7.Text.Substring(15).Trim();
                                    }
                                    else
                                        grids.Remove(SchedulePeriod7);
                                }
                                else
                                {
                                    grids.Remove(SchedulePeriod6);
                                    grids.Remove(SchedulePeriod7);
                                }
                            }
                            else
                            {
                                grids.Remove(SchedulePeriod5);
                                grids.Remove(SchedulePeriod6);
                                grids.Remove(SchedulePeriod7);
                            }
                        }
                        else
                        {
                            grids.Remove(SchedulePeriod4);
                            grids.Remove(ScheduleLunch);
                            grids.Remove(SchedulePeriod5);
                            grids.Remove(SchedulePeriod6);
                            grids.Remove(SchedulePeriod7);
                        }
                    }
                    else
                    {
                        grids.Remove(SchedulePeriod3);
                        grids.Remove(SchedulePeriod4);
                        grids.Remove(ScheduleLunch);
                        grids.Remove(SchedulePeriod5);
                        grids.Remove(SchedulePeriod6);
                        grids.Remove(SchedulePeriod7);
                    }
                }
                else
                {
                    grids.Remove(SchedulePeriod2);
                    grids.Remove(SchedulePeriod3);
                    grids.Remove(SchedulePeriod4);
                    grids.Remove(ScheduleLunch);
                    grids.Remove(SchedulePeriod5);
                    grids.Remove(SchedulePeriod6);
                    grids.Remove(SchedulePeriod7);
                }
            }
            else
            {
                grids.Remove(SchedulePeriod1);
                grids.Remove(SchedulePeriod2);
                grids.Remove(SchedulePeriod3);
                grids.Remove(SchedulePeriod4);
                grids.Remove(ScheduleLunch);
                grids.Remove(SchedulePeriod5);
                grids.Remove(SchedulePeriod6);
                grids.Remove(SchedulePeriod7);
            }

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
            Date.Text = DateTime.ParseExact(timetable.Date[dow], "yyyyMMdd", null).ToString("yyyy년 M월 d일") + " 시간표입니다.";
            
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
            task = false;
        }
        #endregion

        #region 급식 메뉴 보기
        private async Task ViewLunchMenuAnimation()
        {
            LunchMenu.IsVisible = true;
            if (App.Animation)
            {
                LunchMenu.Opacity = 0;
                await LunchMenu.FadeTo(1, 750, Easing.SpringIn);
            }
        }
        #endregion

        #region 학사 일정 보기
        private async Task ViewSchoolScheduleAnimation()
        {
            SchoolSchedule.IsVisible = true;
            if (App.Animation)
            {
                SchoolSchedule.Opacity = 0;
                await SchoolSchedule.FadeTo(1, 750, Easing.SpringIn);
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
                ViewSchoolScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewLunchMenuButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchoolScheduleButton.TextColor = Color.FromRgb(43, 43, 43);

                LunchMenu.IsVisible = false;
                SchoolSchedule.IsVisible = false;

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
                ViewSchoolScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewScheduleButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewSchoolScheduleButton.TextColor = Color.FromRgb(43, 43, 43);

                Schedule.IsVisible = false;
                SchoolSchedule.IsVisible = false;

                ViewButtonAnimation(sender as Button);
                await ViewLunchMenuAnimation();
                task = false;
            }
        }
        #endregion

        #region 학사 일정 보기 버튼
        private async void ViewSchoolScheduleButton_Clicked(object sender, System.EventArgs e)
        {
            if (!task && view != "school_schedule")
            {
                task = true;
                view = "school_schedule";

                ViewScheduleButton.BackgroundColor = Color.FromRgb(248, 248, 255);
                ViewLunchMenuButton.BackgroundColor = Color.FromRgb(248, 248, 255);

                ViewScheduleButton.TextColor = Color.FromRgb(43, 43, 43);
                ViewLunchMenuButton.TextColor = Color.FromRgb(43, 43, 43);

                Schedule.IsVisible = false;
                LunchMenu.IsVisible = false;

                ViewButtonAnimation(sender as Button);
                await ViewSchoolScheduleAnimation();
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
            }
        }
        #endregion

        #region 새로고침 버튼
        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            await ImageButtonAnimation(sender as ImageButton);

            if (!task)
            {
                task = true;

                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    if(App.Class == 0)
                    {
                        DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                            "프로필 설정을 완료해주세요.");
                        task = false;
                        return;
                    }

                    var dataInfo = MainPage.GetInstance().GetDataInfo();

                    if (dataInfo == null)
                    {
                        DependencyService.Get<IToastMessage>().Longtime("서버에서 데이터를 받아오지 못했습니다.\n잠시 후 다시 시도해 주시기 바랍니다.");
                        task = false;
                        return;
                    }

                    var timetableByte = ByteSize.FromBytes(MainPage.GetInstance().GetJsonByteLength(App.Timetable).Result).ToString();
                    var lunchMenuByte = ByteSize.FromBytes(MainPage.GetInstance().GetJsonByteLength(App.LunchMenu).Result).ToString();
                    var schoolScheduleByte = ByteSize.FromBytes(MainPage.GetInstance().GetJsonByteLength(App.SchoolSchedule).Result).ToString();

                    string timetable;
                    string lunchMenu;
                    string schoolSchedule;

                    if (dataInfo["Timetable-" + App.GetClassName()].ContainsKey("Size"))
                        timetable = dataInfo["Timetable-" + App.GetClassName()]["Size"];
                    else
                        timetable = timetableByte;

                    if (dataInfo["LunchMenu"].ContainsKey("Size"))
                        lunchMenu = dataInfo["LunchMenu"]["Size"];
                    else
                        lunchMenu = lunchMenuByte;

                    if (dataInfo["SchoolSchedule"].ContainsKey("Size"))
                        schoolSchedule = dataInfo["SchoolSchedule"]["Size"];
                    else
                        schoolSchedule = schoolScheduleByte;

                    if(timetableByte == timetable && lunchMenuByte == lunchMenu && schoolScheduleByte == schoolSchedule)
                    {
                        await DisplayAlert("새로고침", "이미 최신 데이터입니다.", "확인");
                        task = false;
                        return;
                    }

                    var total = ByteSize.Parse(timetable);
                    total = total.Add(ByteSize.Parse(lunchMenu));
                    total = total.Add(ByteSize.Parse(schoolSchedule));

                    var result = await DisplayAlert("새로고침", 
                        "데이터를 새로 다운받습니다.\n" +
                        "LTE/5G를 사용 중인 경우 데이터가 사용됩니다.\n\n" + 
                        "※ 다운받는 데이터 크기\n" +
                        "- 시간표: " + timetable + "\n" +
                        "- 급식 메뉴: " + lunchMenu + "\n" +
                        "- 학사 일정: " + schoolSchedule + "\n\n" +
                        "총 [" + total.ToString() + "] 를 다운받습니다.",
                        "확인", "취소");

                    if(!result)
                    {
                        task = false;
                        return;
                    }
                    await MainPage.GetInstance().GetData(refresh: true);
                    task = false;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().Longtime("데이터를 가져올 수 없습니다.\n" +
                        "인터넷 상태를 확인해주세요.");
                    task = false;
                }
            }
        }
        #endregion
        #endregion

        #region 함수
        #region 급식 메뉴 데이터 초기화
        public void InitLunchMenu()
        {
            var appointments = new CalendarEventCollection();

            var lunchMenu = App.LunchMenu;
            var datas = new List<JsonData>();

            foreach (var key in lunchMenu.Data.Keys)
            {
                var date = DateTime.ParseExact(key, "yyyyMMdd", null).ToString();

                foreach (var item in lunchMenu.Data[key])
                {
                    var random = new Random();

                    datas.Add(new JsonData
                    {
                        Subject = item,
                        StartTime = date,
                        EndTime = date,
                        IsAllDay = "True",
                        Background = string.Format("#{0:X6}", random.Next(0x1000000))
                    });
                }
            }

            foreach (var data in datas)
            {
                appointments.Add(new CalendarInlineEvent()
                {
                    Subject = data.Subject,
                    StartTime = Convert.ToDateTime(data.StartTime),
                    EndTime = Convert.ToDateTime(data.EndTime),
                    Color = Color.FromHex(data.Background),
                    IsAllDay = Convert.ToBoolean(data.IsAllDay)
                });
            }

            LunchMenuCalendar.DataSource = appointments;
        }
        #endregion

        #region 학사 일정 데이터 초기화
        public void InitSchoolSchedule()
        {
            var appointments = new CalendarEventCollection();

            var schoolSchedule = App.SchoolSchedule;
            var datas = new List<JsonData>();

            foreach (var key in schoolSchedule.Keys)
            {
                foreach (var value in schoolSchedule[key].Data.Keys)
                {
                    var date = DateTime.ParseExact(key + value.PadLeft(2, '0'), "yyyyMMdd", null).ToString();

                    var random = new Random();

                    foreach (var item in schoolSchedule[key].Data[value])
                    {
                        datas.Add(new JsonData
                        {
                            Subject = item,
                            StartTime = date,
                            EndTime = date,
                            IsAllDay = "True",
                            Background = string.Format("#{0:X6}", random.Next(0x1000000))
                        });
                    }
                }
            }

            foreach (var data in datas)
            {
                appointments.Add(new CalendarInlineEvent()
                {
                    Subject = data.Subject,
                    StartTime = Convert.ToDateTime(data.StartTime),
                    EndTime = Convert.ToDateTime(data.EndTime),
                    Color = Color.FromHex(data.Background),
                    IsAllDay = Convert.ToBoolean(data.IsAllDay)
                });
            }

            SchoolScheduleCalendar.DataSource = appointments;
        }
        #endregion

        #region 인스턴스 가져오기
        public static TabbedSchedulePage GetInstance()
        {
            return instance;
        }
        #endregion
        #endregion
    }
}