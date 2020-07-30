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
    public class AppInfoController : ControllerBase
    {
        #region GET - API Key
        [HttpGet("{apiKey}")]
        public Dictionary<string, string> Get(string apiKey)
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                if (apiKey != Program.API_KEY)
                {
                    var errorDict = new Dictionary<string, string>
                    {
                        { "ResultCode", "002" },
                        { "ResultMsg", "유효하지 않은 API 키 값입니다." }
                    };

                    Program.Logger.LogInformation("<" + clientInfo + "> 앱 정보 요청: 결과 - " + errorDict["ResultCode"] + " (" + errorDict["ResultMsg"] + ")");
                    return errorDict;
                }

                if(Program.AppInfo == null)
                {
                    var errorDict = new Dictionary<string, string>
                    {
                        { "ResultCode", "003" },
                        { "ResultMsg", "데이터가 로드되지 않았습니다.\n잠시 후 다시 시도해 주시기 바랍니다." }
                    };

                    Program.Logger.LogInformation("<" + clientInfo + "> 앱 정보 요청: 결과 - " + errorDict["ResultCode"] + " (" + errorDict["ResultMsg"] + ")");
                    return errorDict;
                }

                Program.Logger.LogInformation("<" + clientInfo + "> 앱 정보 요청: 결과 - 000 (정상적으로 요청되었습니다.)");
                return Program.AppInfo;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, string>
                    {
                        { "ResultCode", "999" },
                        { "ResultMsg", "알 수 없는 오류:\n" + e.Message }
                    };

                Program.Logger.LogError("<" + clientInfo + "> 앱 정보 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion

        #region GET - Not API Key
        [HttpGet]
        public Dictionary<string, string> NotKey()
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                var errorDict = new Dictionary<string, string>
                {
                    { "ResultCode", "001" },
                    { "ResultMsg", "API 키 값을 입력해주세요." }
                };

                Program.Logger.LogInformation("<" + clientInfo + "> 앱 정보 요청: 결과 - " + errorDict["ResultCode"] + " (" + errorDict["ResultMsg"] + ")");
                return errorDict;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, string>
                {
                    { "ResultCode", "999" },
                    { "ResultMsg", "알 수 없는 오류:\n" + e.Message }
                };

                Program.Logger.LogError("<" + clientInfo + "> 앱 정보 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion
    }
}