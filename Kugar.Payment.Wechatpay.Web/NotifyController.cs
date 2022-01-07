using System.Threading.Tasks;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kugar.Payment.Wechatpay.Web
{
    public class NotifyController : ControllerBase
    {
        // GET
        [HttpPost,Route("Core/Payment/Callback/Wechatpay/{appID}")]
        public async Task<IActionResult> PayCallback([FromRoute]string appId,[FromServices] IResultNotifyHandler handler=null)
        {
            Request.EnableBuffering();

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var inputStream = Request.Body;
            inputStream.Position = 0;
             
            var pay = WechatpayFactory.GetByAppId(appId);

            var result=await pay.NotifyHandler().DecodePaymentNotifyData(inputStream);

            if (!result)
            {
                return Content(pay.NotifyHandler().BuildFaildResponse(result.Message),"application/xml");
            }
            else
            {
                if (handler!=null)
                {
                    var ret = await handler.OnPaymentNotifyAsync(result.ReturnData,appId);

                    if (ret)
                    {
                        return Content(pay.NotifyHandler().BuildSuccessResponse(), "application/xml");
                    }
                    else
                    {
                        return Content(pay.NotifyHandler().BuildFaildResponse(ret.Message), "application/xml");
                    }
                }
                else
                {
                    return Content(pay.NotifyHandler().BuildSuccessResponse(), "application/xml");
                }
            } 
        }

        [HttpPost, Route("Core/Refund/Callback/Wechatpay/{appID}")]
        public async Task<IActionResult> RefundCallback([FromRoute] string appId, [FromServices] IResultNotifyHandler handler = null)
        {
            return View();
        }
    }
}