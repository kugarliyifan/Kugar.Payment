using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using OneOf;
using static Kugar.Payment.Wechatpay.Services.MicropayService;

namespace Kugar.Payment.Wechatpay.Requests
{
    public class MicropayRequest:WechatPaymentRequestBase
    {
        /// <summary>
        /// 扫码支付付款码，设备读取用户微信中的条码或者二维码信息,前缀 10、11、12、13、14、15开头
        /// </summary>
        /// <returns></returns>
        public string AuthCode { set; get; }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <returns></returns>
        public string Body { set; get; }
        
        public DateTime? LimitTimeStartDt { set; get; }

        public DateTime? LimitTimeEndDt { set; get; }

        public bool NoCredit { set; get; }

        public bool ProfitSharing { set; get; }

        public override ResultReturn Validate()
        {
            if (string.IsNullOrWhiteSpace(AuthCode))
            {
                return new FailResultReturn<MicropayResult>("AuthCode不能为空");
            }

            if (string.IsNullOrWhiteSpace(Body))
            {
                return new FailResultReturn<MicropayResult>("body不能为空");
            }

            if (Amount <= 0)
            {
                return new FailResultReturn<MicropayResult>("amount必须大于0");
            }

            if (string.IsNullOrWhiteSpace(OutTradeNo))
            {
                return new FailResultReturn<MicropayResult>("out_trade_no不能为空");
            }

            return base.Validate();
        }

        public override Dictionary<string, OneOf<int, string>> ToData()
        {
            var data = new Dictionary<string, OneOf<int, string>>();

            data.AddOrUpdate("auth_code", AuthCode); //授权码
            data.AddOrUpdate("body", Body); //商品描述
            data.AddOrUpdate("total_fee", (int)(Amount * 100)); //总金额
            data.AddOrUpdate("out_trade_no", OutTradeNo); //产生随机的商户订单号
            data.AddIf(!string.IsNullOrWhiteSpace(Attach), "attach", Attach);
            data
                //.AddIf(!string.IsNullOrWhiteSpace(_fee_type), "fee_type", _fee_type)
                //.AddIf(!string.IsNullOrWhiteSpace(_spbill_create_ip), "spbill_create_ip", _spbill_create_ip)
                .AddIf(LimitTimeStartDt.HasValue, "time_start", LimitTimeStartDt.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(LimitTimeEndDt.HasValue, "time_expire", LimitTimeEndDt.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(NoCredit, "limit_pay", "no_credit")
                .AddIf(ProfitSharing, "profit_sharing", "Y")
                ;

            return base.ToData();
        }

        public MicropayRequest(WechatpayConfig config) : base(config)
        {
        }
    }
}
