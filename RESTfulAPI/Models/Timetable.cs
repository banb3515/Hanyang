using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTfulAPI.Models
{
    /// <summary>
    /// 반 별 시간표입니다.
    /// </summary>
    public class Timetable
    {
        /// <summary>
        /// 학년
        /// </summary>
        public int Grade { get; set; } = -1;

        /// <summary>
        /// 반
        /// </summary>
        public string Class { get; set; } = "None";

        /// <summary>
        /// 학과
        /// </summary>
        public string Department { get; set; } = "None";

        /// <summary>
        /// 시간표 데이터
        /// 첫 번째 문자열: 요일
        /// 두 번째 문자열: 교시
        /// 세 번째 문자열: 과목
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Data { get; set; }
    }
}