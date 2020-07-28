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
        #region GET - API Key
        [HttpGet("{apiKey}")]
        public Dictionary<string, Dictionary<string, string>> Get(string apiKey)
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                if (apiKey != Program.API_KEY)
                {
                    var errorDict = new Dictionary<string, Dictionary<string, string>>
                    {
                        { "Error", new Dictionary<string, string>() }
                    };
                    errorDict["Error"].Add("ResultCode", "002");
                    errorDict["Error"].Add("ResultMsg", "유효하지 않은 API 키 값입니다.");

                    Program.Logger.LogInformation("<" + clientInfo + "> 데이터 정보 요청: 결과 - " + errorDict["Error"]["ResultCode"] + " (" + errorDict["Error"]["ResultMsg"] + ")");
                    return errorDict;
                }

                if(Program.DataInfo == null)
                {
                    var errorDict = new Dictionary<string, Dictionary<string, string>>
                    {
                        { "Error", new Dictionary<string, string>() }
                    };
                    errorDict["Error"].Add("ResultCode", "003");
                    errorDict["Error"].Add("ResultMsg", "데이터가 로드되지 않았습니다.\n잠시 후 다시 시도해 주시기 바랍니다.");

                    Program.Logger.LogInformation("<" + clientInfo + "> 데이터 정보 요청: 결과 - " + errorDict["Error"]["ResultCode"] + " (" + errorDict["Error"]["ResultMsg"] + ")");
                    return errorDict;
                }

                Program.Logger.LogInformation("<" + clientInfo + "> 데이터 정보 요청: 결과 - 000 (정상적으로 요청되었습니다.)");
                return Program.DataInfo;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, Dictionary<string, string>>
                {
                    { "Error", new Dictionary<string, string>() }
                };
                errorDict["Error"].Add("ResultCode", "999");
                errorDict["Error"].Add("ResultMsg", "알 수 없는 오류:\n" + e.Message);

                Program.Logger.LogError("<" + clientInfo + "> 데이터 정보 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion

        #region GET - Not API Key
        [HttpGet]
        public Dictionary<string, Dictionary<string, string>> NotKey()
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                var errorDict = new Dictionary<string, Dictionary<string, string>>
                {
                    { "Error", new Dictionary<string, string>() }
                };
                errorDict["Error"].Add("ResultCode", "001");
                errorDict["Error"].Add("ResultMsg", "API 키 값을 입력해주세요.");

                Program.Logger.LogInformation("<" + clientInfo + "> 데이터 정보 요청: 결과 - " + errorDict["Error"]["ResultCode"] + " (" + errorDict["Error"]["ResultMsg"] + ")");
                return errorDict;
            }
            catch (Exception e)
            {
                var errorDict = new Dictionary<string, Dictionary<string, string>>
                {
                    { "Error", new Dictionary<string, string>() }
                };
                errorDict["Error"].Add("ResultCode", "999");
                errorDict["Error"].Add("ResultMsg", "알 수 없는 오류:\n" + e.Message);

                Program.Logger.LogError("<" + clientInfo + "> 데이터 정보 요청: 결과 - 999 (" + e.Message + ")");
                return errorDict;
            }
        }
        #endregion
    }
}