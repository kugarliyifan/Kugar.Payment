using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Kugar.Payment.Common;
using Microsoft.AspNetCore.Http;

namespace Kugar.Payment.Alipay.Web
{
    public class NotifyController : ControllerBase
    {
        /// <summary>
        /// 支付回调通知
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost, Route("Core/Payment/Callback/Alipay/{appID}")]
        public async Task<IActionResult> PayCallback([FromRoute] string appId, [FromServices] IResultNotifyHandler handler = null)
        {
            Request.EnableBuffering();

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var inputStream = Request.Body;
            inputStream.Position = 0;

            var pay = AlipayFactory.GetByAppId(appId);

            var result = await pay.NotifyHandler().DecodePaymentNotifyData(inputStream);

            if (!result)
            {
                return Content(pay.NotifyHandler().BuildFaildResponse(result.Message), "application/xml");
            }
            else
            {
                if (handler != null)
                {
                    var ret = await handler.OnPaymentNotifyAsync(result.ReturnData, appId);

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

        /// <summary>
        /// 退款回调通知
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost, Route("Core/Refund/Callback/Alipay/{appID}")]
        public async Task<IActionResult> RefundCallback([FromRoute] string appId, [FromServices] IResultNotifyHandler handler = null)
        {
            Request.EnableBuffering();

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var inputStream = Request.Body;
            inputStream.Position = 0;

            var pay = AlipayFactory.GetByAppId(appId);

            var result = await pay.NotifyHandler().DecodeRefundNotifyData(inputStream);

            if (!result)
            {
                return Content(pay.NotifyHandler().BuildFaildResponse(result.Message), "application/xml");
            }
            else
            {
                if (handler != null)
                {
                    var ret = await handler.OnRefundNotifyAsync(result.ReturnData, appId);

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
    }
}
