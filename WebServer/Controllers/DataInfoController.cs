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
    public class DataInfoController : ControllerBase
    {
        #region 변수
        // API KEY 값
        private const string API_KEY = "3tcPgoxHf2XZboJWuoF3mOX2ZV2OXlfbunUpFvjUvBORUeYWZBApTsYh6PbBXyweF4iPO1wZXLoKXOCrykHMVTrBWvwEcWIOzl1a1CzswHEQvGTWp3hMJEMbFZtqxXcI";
        #endregion

        #region GET - API Key
        [HttpGet("{dataType}")]
        public Dictionary<string, string> Get(string dataType)
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                

                Program.Logger.LogInformation("<" + clientInfo + "> 시간표 요청: 결과 - 000 (정상적으로 요청되었습니다.)");
                return null;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, string>();
                errorDict.Add("ErrorCode", "999");

                Program.Logger.LogError("<" + clientInfo + "> 데이터 정보 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion

        #region GET - Not API Key
        [HttpGet]
        public Dictionary<string, Dictionary<string, string>> All()
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            //Program.Logger.LogInformation("<" + clientInfo + "> 시간표 요청: 결과 - " + errorDict["Error"].ResultCode + " (" + errorDict["Error"].ResultMsg + ")");
            //return errorDict;
            return null;
        }
        #endregion
    }
}