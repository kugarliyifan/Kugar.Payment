using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.DragonPay.Enums;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class QueryOrderResult:DragonPaymentResultBase
    {
        public QueryOrderResult(JObject json) : base(json)
        {
            //IsSuccess = json.GetString("RESULT") == "Y";

            var result = json.GetString("RESULT");

            if (result == "Q")
            {
                IsNeedForCheck = true;
                Code = result;
            }
            else
            {
                Code = result;
            }
             
            WaitTime = json.GetInt("WAITTIME");
            Amount = json.GetDecimal("AMOUNT");

            

            if (IsSuccess)
            {
                var wechatOpenId = json.GetString("SUB_OPENID");

                if (!string.IsNullOrWhiteSpace(wechatOpenId))
                {
                    OpenId = wechatOpenId;
                }
                else
                {
                    OpenId = json.GetString("OPENID");
                }

                PaidAmount = json.GetDecimal("PAID_AMOUNT");

                TotalAmount = Amount;

                ReceiptAmount = json.GetDecimal("RECEIPT_AMOUNT");

                var p = json.GetJObject("PAYMENT_DETAILS");

                TransactionId = p.GetString("THIRD_TRADE_NO");

                if (this.Message.Contains("请撤销订单"))
                {
                    this.IsCanceled = true;
                }
            }
            
        }
        
        
        /// <summary>
        /// 建议等待确认时间
        /// </summary>
        public int WaitTime { set; get; }

        /// <summary>
        /// 是否需要等待确认支付,比如扫码后等待用户确认等
        /// </summary>
        public bool IsNeedForCheck { set; get; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { set; get; }

        /// <summary>
        /// 客户识别码/微信OpenId
        /// </summary>
        public string OpenId { set; get; }

        /// <summary>
        /// 顾客实际支付金额
        /// </summary>
        public decimal PaidAmount { set; get; }

        /// <summary>
        /// 商户实收金额
        /// </summary>
        public decimal ReceiptAmount { set; get; }
        
    }
}
