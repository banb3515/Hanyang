#region API 참조
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Models;

using System;
using System.Collections.Generic;
#endregion

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        #region 변수
        // API KEY 값
        private const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";
        #endregion

        #region GET - API Key
        [HttpGet("{apiKey}")]
        public Dictionary<string, Timetable> Get(string apiKey)
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                if (apiKey != API_KEY)
                {
                    var errorDict = new Dictionary<string, Timetable>();
                    errorDict.Add("Error",
                        new Timetable
                        {
                            ResultCode = "002",
                            ResultMsg = "유효하지 않은 API 키 값입니다.",
                            Data = null,
                            Date = null
                        });

                    Program.Logger.LogInformation("<" + clientInfo + "> 시간표 요청: 결과 - " + errorDict["Error"].ResultCode + " (" + errorDict["Error"].ResultMsg + ")");
                    return errorDict;
                }



                Program.Logger.LogInformation("<" + clientInfo + "> 시간표 요청: 결과 - 000 (정상적으로 요청되었습니다.)");
                return null;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, Timetable>();
                errorDict.Add("Error",
                    new Timetable
                    {

                        ResultCode = "999",
                        ResultMsg = "알 수 없는 오류:\n" + e.Message,
                        Data = null,
                        Date = null
                    });

                Program.Logger.LogError("<" + clientInfo + "> 시간표 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion

        #region GET - Not API Key
        [HttpGet]
        public Dictionary<string, Timetable> NotKey()
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            var errorDict = new Dictionary<string, Timetable>();
            errorDict.Add("Error",
                new Timetable
                {
                    ResultCode = "001",
                    ResultMsg = "API 키 값을 입력해주세요.",
                    Data = null,
                    Date = null
                });

            Program.Logger.LogInformation("<" + clientInfo + "> 시간표 요청: 결과 - " + errorDict["Error"].ResultCode + " (" + errorDict["Error"].ResultMsg + ")");
            return errorDict;
        }
        #endregion
    }
}