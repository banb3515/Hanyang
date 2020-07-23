#region API 참조
using Hanyang.Models;

using Newtonsoft.Json;

using Syncfusion.SfCalendar.XForms;

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;
#endregion

namespace Hanyang.Others
{
    public class CalendarContainer : INotifyPropertyChanged
    {
        #region 변수
        private CalendarEventCollection appointments;

        public CalendarEventCollection Appointments
        {
            get { return this.appointments; }
            set
            {
                this.appointments = value;
                this.OnPropertyChanged("Appointments");
            }
        }
        #endregion

        #region 이벤트
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region JSON 데이터
        // 서버에서 받은 데이터로 변경
        private string JsonData =
            "[{\"Subject\": \"한양제\",\"StartTime\": \"30 June 2020 02:00:00 PM\",\"EndTime\":\"30 June 2020 03:00:00 PM\",\"Background\":\"#5944dd\", \"IsAllDay\":\"True\"}, " +
            "{\"Subject\": \"기말고사\",\"StartTime\": \"22 June 2020 08:00:00 AM\",\"EndTime\":\"25 June 2020 10:00:00 AM\",\"Background\":\"#ff0000\", \"IsAllDay\":\"True\"}," +
            "{\"Subject\": \"수행평가\",\"StartTime\": \"17 June 2020 05:00:00 PM\",\"EndTime\":\"17 June 2020 06:00:00 PM\",\"Background\":\"#5944dd\", \"IsAllDay\":\"True\"}," +
            "{\"Subject\": \"놀기\",\"StartTime\": \"03 June 2020 09:00:00 AM\",\"EndTime\":\"03 June 2020 11:00:00 AM\",\"Background\":\"#ed0497\", \"IsAllDay\":\"True\"}," +
            "{\"Subject\": \"몰라\",\"StartTime\": \"27 June 2020 09:00:00 AM\",\"EndTime\":\"27 June 2020 11:00:00 AM\",\"Background\":\"#ff0000\", \"IsAllDay\":\"True\"}," +
            "{\"Subject\": \"ㅇㅇ\",\"StartTime\": \"30 June 2020 10:00:00 AM\",\"EndTime\":\"30 June 2020 11:00:00 AM\",\"Background\":\"#ed0497\", \"IsAllDay\":\"True\"} ]";
        #endregion

        #region 생성자
        public CalendarContainer()
        {
            this.Appointments = new CalendarEventCollection();

            List<JsonData> jsonDataCollection = JsonConvert.DeserializeObject<List<JsonData>>(JsonData);

            foreach (var data in jsonDataCollection)
            {
                this.Appointments.Add(new CalendarInlineEvent()
                {
                    Subject = data.Subject,
                    StartTime = Convert.ToDateTime(data.StartTime),
                    EndTime = Convert.ToDateTime(data.EndTime),
                    Color = Color.FromHex(data.Background),
                    IsAllDay = Convert.ToBoolean(data.IsAllDay)
                });
            }
        }
        #endregion

        #region OnPropertyChanged
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}