using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Payment.Common.Helpers;
using OneOf;
using static Kugar.Payment.Wechatpay.Services.JsApiPayService;

namespace Kugar.Payment.Wechatpay.Requests
{
    public class JsApiPayRequest:WechatPaymentRequestBase
    {
        /// <summary>
        /// 支付的用户OpenId
        /// </summary>
        /// <returns></returns>
        public string OpenId { set; get;}

        /// <summary>
        /// 通知地址,如未配置,则使用初始化配置中的NotifyUrl属性
        /// </summary>
        /// <returns></returns>
        public string NotifyUrl { set; get; }

        /// <summary>
        /// 调用方IP地址
        /// </summary>
        /// <returns></returns>
        public string SpbillCreateIp { set; get; }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <returns></returns>
        public string Body { set; get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符）
        /// </summary>
        /// <returns></returns>
        public string OutTradeNo { set; get; }

        public DateTime? LimitTimeStartDt { set; get; }

        public DateTime? LimitTimeEndDt { set; get; }

        public bool NoCredit { set; get; }

        public bool ProfitSharing { set; get; }

        public override ResultReturn Validate()
        {
            if (string.IsNullOrWhiteSpace(OpenId))
            {
                return new FailResultReturn<JsApiPayArgument>("openId不能为空");
            }

            if (string.IsNullOrWhiteSpace(Body))
            {
                return new FailResultReturn<JsApiPayArgument>("body不能为空");
            }

            if (Amount <= 0)
            {
                return new FailResultReturn<JsApiPayArgument>("amount必须大于0");
            }

            if (string.IsNullOrWhiteSpace(OutTradeNo))
            {
                return new FailResultReturn<JsApiPayArgument>("out_trade_no不能为空");
            }

            if (OutTradeNo.Length>32)
            {
                return new FailResultReturn<JsApiPayArgument>("out_trade_no不能超过32个字符");
            }
            
            if (string.IsNullOrWhiteSpace(SpbillCreateIp))
            {
                return new FailResultReturn<JsApiPayArgument>("spbill_create_ip不能为空");
            }
            
            

            return SuccessResultReturn.Default;
        }

        public override Dictionary<string, OneOf<int, string>> ToData()
        {
            var data= new Dictionary<string,OneOf<int, string>>();

            var notifyUrl = string.IsNullOrWhiteSpace(NotifyUrl) ? Config.PaymentNotifyUrl : NotifyUrl;
            
            notifyUrl = BuildNotifyUrl(notifyUrl);

            data.AddOrUpdate("body", Body); //商品描述
            data.AddOrUpdate("total_fee", (int)(Amount * 100)); //总金额
            data.AddOrUpdate("out_trade_no", OutTradeNo);
            data.AddOrUpdate("trade_type", "JSAPI");
            data.AddOrUpdate("openid", OpenId);
            data.AddOrUpdate("spbill_create_ip", SpbillCreateIp);
            data.AddOrUpdate("notify_url", notifyUrl);

            //Debugger.Break();

            data.AddIf(!string.IsNullOrWhiteSpace(Attach), "attach", Attach);
            data
                //.AddIf(!string.IsNullOrWhiteSpace(_fee_type), "fee_type", _fee_type)
                //.AddIf(!string.IsNullOrWhiteSpace(_ip), "spbill_create_ip", _ip)
                .AddIf(LimitTimeStartDt.HasValue, "time_start", ()=>LimitTimeStartDt.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(LimitTimeEndDt.HasValue, "time_expire", () => LimitTimeEndDt.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(NoCredit, "limit_pay", "no_credit")
                .AddIf(ProfitSharing, "profit_sharing", "Y");

            LoggerManager.Default.Debug("回调地址:" + notifyUrl);

            return data;
        }

        public JsApiPayRequest(WechatpayConfig config) : base(config)
        {
        }
    }
}
