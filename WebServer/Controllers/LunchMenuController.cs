#region API 참조
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Models;

using System;
#endregion

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LunchMenuController : ControllerBase
    {
        #region GET - API Key
        [HttpGet("{apiKey}")]
        public LunchMenu Get(string apiKey)
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                if (apiKey != Program.API_KEY)
                {
                    var error = new LunchMenu
                    {
                        ResultCode = "002",
                        ResultMsg = "유효하지 않은 API 키 값입니다.",
                        Data = null
                    };

                    Program.Logger.LogInformation("<" + clientInfo + "> 급식 메뉴 요청: 결과 - " + error.ResultCode + " (" + error.ResultMsg + ")");
                    return error;
                }

                if(Program.LunchMenu == null)
                {
                    var error = new LunchMenu
                    {
                        ResultCode = "003",
                        ResultMsg = "데이터가 로드되지 않았습니다.\n잠시 후 다시 시도해 주시기 바랍니다.",
                        Data = null
                    };

                    Program.Logger.LogInformation("<" + clientInfo + "> 급식 메뉴 요청: 결과 - " + error.ResultCode + " (" + error.ResultMsg + ")");
                    return error;
                }

                Program.Logger.LogInformation("<" + clientInfo + "> 급식 메뉴 요청: 결과 - 000 (정상적으로 요청되었습니다.)");
                return Program.LunchMenu;
            }
            catch (Exception e)
            {
                var error = new LunchMenu
                {
                    ResultCode = "999",
                    ResultMsg = "알 수 없는 오류:\n" + e.Message,
                    Data = null
                };
                Program.Logger.LogError("<" + clientInfo + "> 급식 메뉴 요청: 결과 - 999 (" + e.Message + ")");
                return error;
            }
        }
        #endregion

        #region GET - Not API Key
        [HttpGet]
        public LunchMenu NotKey()
        {
            var clientInfo = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4() + ":" + Request.HttpContext.Connection.RemotePort;

            try
            {
                var error = new LunchMenu
                {
                    ResultCode = "001",
                    ResultMsg = "API 키 값을 입력해주세요.",
                    Data = null
                };

                Program.Logger.LogInformation("<" + clientInfo + "> 급식 메뉴 요청: 결과 - " + error.ResultCode + " (" + error.ResultMsg + ")");
                return error;
            }
            catch (Exception e)
            {
                var error = new LunchMenu
                {
                    ResultCode = "999",
                    ResultMsg = "알 수 없는 오류:\n" + e.Message,
                    Data = null
                };
                Program.Logger.LogError("<" + clientInfo + "> 급식 메뉴 요청: 결과 - 999 (" + e.Message + ")");
                return error;
            }
        }
        #endregion
    }
}