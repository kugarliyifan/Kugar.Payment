using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using Kugar.Core.ExtMethod;
using Kugar.Payment.DragonPay.Enums;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class NotifyPaymentResult:DragonPaymentResultBase
    {
        public NotifyPaymentResult(JObject json) : base(json)
        {
            
            this.PayType = json.GetString("ACC_TYPE")
                .Switch(PayType.Other)
                .Case("AL", PayType.Alipay)
                .Case("WX", PayType.Wechatpay)
                .Result;

            this.Remark1 = json.GetString("REMARK1");
            this.Remark2 = json.GetString("REMARK2");

            var ip = json.GetString("CLIENTIP");

            if (!string.IsNullOrWhiteSpace(ip))
            {
                this.ClientIP = IPAddress.Parse(ip);
            }

            if (IsSuccess)
            {
                var payDt = json.GetString("ACCDATE");

                if (!string.IsNullOrWhiteSpace(payDt))
                {
                    this.PayDt=payDt.ToDateTime("YYYYMMDD",DateTime.Now);
                }

                this.TotalAmount = json.GetDecimal("DISCOUNT");

                if (PayType== PayType.Alipay || PayType== PayType.Other)
                {
                    this.OpenId = json.GetString("OPENID");
                }
                else
                {
                    this.OpenId = json.GetString("SUB_OPENID");
                }

                var payDetailStr = HttpUtility.UrlDecode(json.GetString("PAYMENT_DETAILS"),Encoding.UTF8);

                var payDetailJson = JObject.Parse(payDetailStr);

                this.ThirdTransactionId = payDetailJson.GetString("THIRD_TRADE_NO");
            }
        }
        
        public DateTime PayDt { set; get; }

        public string OpenId { set; get; }

        public PayType PayType { set; get; }

        /// <summary>
        /// 第三方支付的交易单号
        /// </summary>
        public string ThirdTransactionId { set; get; }

        public string Remark1 { set; get; }

        public string Remark2 { set; get; }

        public IPAddress ClientIP { set; get; }
    }
}
