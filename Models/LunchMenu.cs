#region API 참조
using System.Collections.Generic;
#endregion

namespace Models
{
    public class LunchMenu
    {
        /*
         # 결과 코드
         > 성공
         - 000: 클라이언트의 요청을 정상적으로 수행함.
         *
         > 실패
         - 001: API KEY 값을 입력하지 않음
         - 002: 유효하지 않은 API KEY 값
         - 003: 데이터가 로드되지 않음
         */
        public string ResultCode { get; set; } = "000";

        // 결과 메세지
        public string ResultMsg { get; set; } = "정상 처리되었습니다.";

        // 급식 메뉴 데이터: 1 string = 날짜, 2 string = 메뉴
        public Dictionary<string, List<string>> Data { get; set; }
    }
}
