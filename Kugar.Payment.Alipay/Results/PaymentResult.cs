using System;
using System.Collections.Generic;
using System.Linq;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.Results
{
    public class NotifyPaymentResult : AlipayPaymentResultBase
    {
        public NotifyPaymentResult(JObject source) : base(source)
        {
            if (IsSuccess)
            {
                BuyerId = source.GetString("buyer_id");
                BuyerLogonId = source.GetString("buyer_logon_id");

                Body = source.GetString("body");
                BuyerLogonId = source.GetString("buyer_logon_id");
                Subject = source.GetString("subject");
                ReceiptAmount = source.GetDecimal("invoice_amount");
                InvoiceAmount = source.GetDecimal("invoice_amount");
                PaymentDt = source.GetString("gmt_payment").ToDateTime("yyyy-MM-dd HH:mm:ss");
                PaymentCreateDt = source.GetString("gmt_create").ToDateTime("yyyy-MM-dd HH:mm:ss")
                NotifyId = source.GetString("notify_id"); 

            }
        }

        public string BuyerId { set; get; }

        public string BuyerLogonId { set; get; }

        /// <summary>
        /// 订单的备注、描述、明细等。对应请求时的 body 参数，原样通知回来。
        /// </summary>
        public string Body { set; get; }

        /// <summary>
        /// 商品的标题/交易标题/订单标题/订单关键字等，是请求时对应的参数，原样通知回来。
        /// </summary>
        public string Subject { set; get; }

        /// <summary>
        /// 商家在收益中实际收到的款项，单位人民币（元）。
        /// </summary>
        public decimal ReceiptAmount { set; get; }

        /// <summary>
        /// 用户在交易中支付的可开发票的金额。
        /// </summary>
        public decimal InvoiceAmount { set; get; }

        /// <summary>
        /// 该笔交易 的买家付款时间
        /// </summary>
        public DateTime? PaymentDt { set; get; }

        /// <summary>
        /// 该笔交易创建的时间
        /// </summary>
        public DateTime? PaymentCreateDt { set; get; }

        /// <summary>
        /// 通知校验 ID。
        /// </summary> 
        public string NotifyId { set; get; }

        /// <summary>
        /// 签名类型
        /// </summary>
        public string SignType { set; get; }

        public string NotifyType { set; get; }
    }
}