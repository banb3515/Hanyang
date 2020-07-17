using RESTfulAPI.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RESTfulAPI.Controllers
{
    /// <summary>
    /// 한양공업고등학교의 시간표를 가져오는 API입니다.
    /// </summary>

    public class TimetableController : ApiController
    {
        Dictionary<string, Timetable> timetables = new Dictionary<string, Timetable>();

        public TimetableController()
        {
            var dict2 = new Dictionary<string, Dictionary<string, string>>();
            var dict1T = new Dictionary<string, string>();
            dict1T.Add("1", "과학");
            dict1T.Add("2", "일본어");
            dict1T.Add("3", "영어");
            dict1T.Add("4", "수학");
            dict1T.Add("5", "자료구조");
            dict1T.Add("6", "체육");
            dict1T.Add("7", "한국사");

            var dict2T = new Dictionary<string, string>();
            dict2T.Add("1", "수학");
            dict2T.Add("2", "수학");
            dict2T.Add("3", "영어");
            dict2T.Add("4", "한국사");
            dict2T.Add("5", "일본어");
            dict2T.Add("6", "자료구조");
            dict2T.Add("7", "프로그래밍");

            dict2.Add("Monday", dict1T);
            dict2.Add("Tuesday", dict2T);

            var tt2 = new Timetable()
            {
                Class = "2컴넷A",
                Department = "컴퓨터네트워크과",
                Grade = 2,
                Data = dict2
            };

            timetables.Add(tt2.Class, tt2);

            var dict1 = new Dictionary<string, Dictionary<string, string>>();
            dict1T = new Dictionary<string, string>();
            dict1T.Add("1", "수학");
            dict1T.Add("2", "수학");
            dict1T.Add("3", "영어");
            dict1T.Add("4", "한국사");
            dict1T.Add("5", "일본어");
            dict1T.Add("6", "자료구조");
            dict1T.Add("7", "프로그래밍");

            dict2T = new Dictionary<string, string>();
            dict2T.Add("1", "과학");
            dict2T.Add("2", "일본어");
            dict2T.Add("3", "영어");
            dict2T.Add("4", "수학");
            dict2T.Add("5", "자료구조");
            dict2T.Add("6", "체육");
            dict2T.Add("7", "한국사");

            dict1.Add("Monday", dict1T);
            dict1.Add("Tuesday", dict2T);

            var tt1 = new Timetable()
            {
                Class = "2컴넷B",
                Department = "컴퓨터네트워크과",
                Grade = 2,
                Data = dict1
            };

            timetables.Add(tt1.Class, tt1);
        }

        // GET: api/Timetable
        public Dictionary<string, Timetable> Get()
        {
            return timetables;
        }

        // GET: api/Timetable/5
        public Timetable Get(string id)
        {
            Timetable timetable = null;

            foreach (var tt in timetables)
            {
                if (tt.Key == id)
                {
                    timetable = tt.Value;
                    break;
                }
            }

            return timetable;
        }

        // POST: api/Timetable
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Timetable/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Timetable/5
        public void Delete(int id)
        {
        }
    }
}
