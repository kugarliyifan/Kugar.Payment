using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Kugar.Core.BaseStruct;
using System.Threading.Tasks;
using Flurl.Http;
using Kugar.Core.ExtMethod;
using Kugar.Payment.DragonPay.Requests;
using Kugar.Payment.DragonPay.Results;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Services
{
    /// <summary>
    /// 微支付统一下单(公众号/小程序使用)
    /// </summary>
    public class WechatUnifiedService: DragonServiceBase
    {
        private WechatUnifiedRequest _request = null;

        public WechatUnifiedService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new WechatUnifiedRequest(config);
        }

        public WechatUnifiedService OpenId(string openId)
        {
            if (string.IsNullOrWhiteSpace(openId))
            {
                throw new ArgumentOutOfRangeException(nameof(openId));
            }

            _request.OpenId = openId;

            return this;
        }

        public WechatUnifiedService Body(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentOutOfRangeException(nameof(body));
            }

            if (body.Length>256)
            {
                throw new ArgumentOutOfRangeException(nameof(body), "最大256个字符");
            }

            _request.Body = body;

            return this;
        }

        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public WechatUnifiedService Amount(decimal amount)
        {
            if (amount<0.01m) //不能低于1分钱
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "不能低于1分钱");
            }

            _request.Payment = amount;

            return this;
        }

        public WechatUnifiedService ClientIp(IPAddress ip)
        {
            _request.ClientIP = ip;
            return this;
        }

        public WechatUnifiedService Timeout(DateTime timeout)
        {
            _request.Timeout = timeout;

            return this;
        }

        public WechatUnifiedService OrderId(string orderId)
        {
            _request.OrderId = orderId;
            return this;
        }

        public async Task<ResultReturn<WechatUnifiedResult>> ExecuteAsync()
        {
            var ret= await base.PostData($"{Config.GatewayHost}/CCBIS/ccbMain", _request,1);

            if (ret.IsSuccess)
            {
                var success = ret.ReturnData.GetString("SUCCESS").ToBool();
                
                if (success)
                {
                    var payUrl = ret.ReturnData.GetString("PAYURL");

                    var jsonStr = await payUrl.GetStringAsync();

                    var json = JObject.Parse(jsonStr);

                    var result = new WechatUnifiedResult(json);

                    return new SuccessResultReturn<WechatUnifiedResult>(result);
                }
                else
                {
                    return new FailResultReturn<WechatUnifiedResult>(ret.ReturnData.GetString("ERRMSG"));
                } 
            }
            else
            {
                return new FailResultReturn<WechatUnifiedResult>(ret.ReturnData.GetString("ERRMSG"));
            }
        }

        //public 
    }
}
