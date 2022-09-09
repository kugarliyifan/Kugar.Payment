using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Payment.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kugar.Payment.DragonPay.Web
{
    public class NotifyController : ControllerBase
    {
        /// <summary>
        /// 支付回调通知
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost, Route("Core/Payment/Callback/DragonPay/{appID}")]
        public async Task<IActionResult> PayCallback([FromRoute] string appId, [FromServices] IResultNotifyHandler handler = null,[FromServices] DragonPay pay=null)
        {
            //Debugger.Break();

            Request.EnableBuffering();

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var inputStream = Request.Body;
            inputStream.Position = 0;



            //Wechatpay pay = (Wechatpay)provider.GetService(typeof(Wechatpay));

            if (pay == null)
            {
                pay = DragonPayFactory.GetByAppId(appId: appId);
            }

            if (pay == null)
            {
                throw new ArgumentException("AppId不存在配置");
            }

            var xml = inputStream.ReadToEnd(); 

            LoggerManager.Default.Debug(xml);

            var notify = pay.NotifyHandler();

            var result =notify.DecodePaymentNotifyData(xml);

            if (!result)
            {
                return Content(notify.BuildFaildResponse(result.Message), "application/xml");
            }
            else
            {
                if (handler != null)
                {
                    result.ReturnData.RawResult = xml;
                    var ret = await handler.OnPaymentNotifyAsync(pay, result.ReturnData, appId);

                    if (ret)
                    {
                        return Ok();
                        return Content(notify.BuildSuccessResponse(), "application/xml");
                    }
                    else
                    {
                        return Content(notify.BuildFaildResponse(ret.Message), "application/xml");
                    }
                }
                else
                {
                    return Content(notify.BuildSuccessResponse(), "application/xml");
                }
            }
        }

        /// <summary>
        /// 退款回调通知
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost, Route("Core/Refund/Callback/DragonPay/{appID}")]
        public async Task<IActionResult> RefundCallback([FromRoute] string appId, [FromServices] IResultNotifyHandler handler = null, [FromServices] DragonPay pay = null)
        {
            Request.EnableBuffering();

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var inputStream = Request.Body;
            inputStream.Position = 0;

            if (pay == null)
            {
                pay = DragonPayFactory.GetByAppId(appId: appId);
            }

            if (pay == null)
            {
                throw new ArgumentException("AppId不存在配置");
            }

            //var pay = WechatpayFactory.GetByAppId(appId);

            var notifyHandler = pay.NotifyHandler();

            var result = await notifyHandler.DecodePaymentNotifyData(inputStream);

            if (!result)
            {
                return this.BadRequest();
            }
            else
            {
                if (handler != null)
                {
                    var ret = await handler.OnRefundNotifyAsync(pay, null, appId);

                    if (ret)
                    {
                        return this.Ok();
                    }
                    else
                    {
                        return this.BadRequest();
                    }
                }
                else
                {
                    return this.Ok();
                }
            }
        }
    }
}