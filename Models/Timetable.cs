#region API 참조
using System;
using System.Collections.Generic;
#endregion

namespace Models
{
    public class Timetable
    {
        /*
         # 결과 코드
         > 성공
         - 000: 클라이언트의 요청을 정상적으로 수행함.
         *
         > 실패
         - 001: API KEY 값을 입력하지 않음
         - 002: 유효하지 않은 API KEY 값
         */
        public string ResultCode { get; set; } = "000";

        // 결과 메세지
        public string ResultMsg { get; set; } = "정상적으로 요청되었습니다.";

        // 날짜: 1 string = 요일, 2 string = 날짜
        public Dictionary<string, DateTime> Date { get; set; }

        // 시간표 데이터: 1 string = 요일, 2 string = 교시, 3 string = 과목
        public Dictionary<string, Dictionary<string, string>> Data { get; set; }
    }
}
