using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.BaseStruct;
using Kugar.Core.Services;
using OneOf;

namespace Kugar.Payment.Wechatpay.Requests
{
    public abstract class WechatRequestBase
    {
        public WechatRequestBase(WechatpayConfig config)
        {
            Config = config;
        }

        protected WechatpayConfig Config { get; }

        public virtual ResultReturn Validate()
        {
            return SuccessResultReturn.Default;
        }

        public virtual Dictionary<string, OneOf<int, string>> ToData()
        {
            return null;
        }
    }

    public abstract class WechatPaymentRequestBase : WechatRequestBase
    {
        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        public decimal Amount { set; get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符）
        /// </summary>
        public string OutTradeNo { set; get; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string Attach { set; get; }

        protected WechatPaymentRequestBase(WechatpayConfig config) : base(config)
        {
        }

        internal string BuildNotifyUrl(string notifyUrl)
        {
            notifyUrl = notifyUrl.Replace("{appID}", Config.AppId);

            if (!notifyUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                var host = Config.Host.Match(
                    i => i,
                    j => j(GlobalProvider.Provider)
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
