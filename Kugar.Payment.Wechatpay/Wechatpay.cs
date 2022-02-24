using Kugar.Payment.Wechatpay.Services;
using Microsoft.AspNetCore.Http;
using System;
using Kugar.Core.Services;
using Kugar.Payment.Common.Helpers;

namespace Kugar.Payment.Wechatpay
{
    public class Wechatpay
    {
        private WechatpayConfig _config = null;

        public Wechatpay(WechatpayConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 刷卡支付,商家扫用户二维码
        /// </summary>
        /// <returns></returns>
        public MicropayService Micropay() => new MicropayService(this, config: _config);

        /// <summary>
        /// 扫码支付,用户扫商家二维码
        /// </summary>
        /// <returns></returns>
        public NativePayService NativePay() => new NativePayService(this, config: _config);

        /// <summary>
        /// 退款操作
        /// </summary>
        /// <returns></returns>
        public RefundService Refund() => new RefundService(this, config: _config);

        /// <summary>
        /// 退款状态查询
        /// </summary>
        /// <returns></returns>
        public RefundQueryService RefundQuery() => new RefundQueryService(this, config: _config);

        /// <summary>
        /// 公众号/小程序支付
        /// </summary>
        /// <returns></returns>
        public JsApiPayService JsApiPay() => new JsApiPayService(this, config: _config);
        
        /// <summary>
        /// 通用操作
        /// </summary>
        /// <returns></returns>
        public CommonService Common() => new CommonService(this, config: _config);

        public PollingService Polling() => new PollingService(this, _config);

        /// <summary>
        /// 通知处理
        /// </summary>
        /// <returns></returns>
        public NotifyHandlerService NotifyHandler() => new NotifyHandlerService(this, config: _config);

        internal string BuildNotifyUrl(string notifyUrl)
        {
            if (!notifyUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                var host = _config.Host.Match(
                    i=>i,
                    j=>j(GlobalProvider.Provider)
                    );
                //if (_config.Host.)
                //{
                //    if (_config.Host.Value.IsT0)
                //    {
                //        host = _config.Host.Value.AsT0;
                //    }
                //    else
                //    {
                //        host = _config.Host.Value.AsT1(GlobalProvider.Provider);
                //    }
                //}
                //else
                //{
                //    var h = (IHttpContextAccessor)GlobalProvider.Provider.GetService(typeof(IHttpContextAccessor));

                //    var t1 = h.HttpContext.Request.Host;

                //    host =
                //        $"http{(h.HttpContext.Request.IsHttps ? "s" : "")}://{t1.Host}:{(t1.Port.HasValue ? t1.Port.Value.ToString() : "")}";
                //}

                if (notifyUrl.StartsWith('/'))
                {
                    notifyUrl = host + notifyUrl;
                }
                else
                {
                    notifyUrl = $"{host}/{notifyUrl}";
                }

                return notifyUrl;
            }
            else
            {
                return notifyUrl;
            }
        }
    }
}