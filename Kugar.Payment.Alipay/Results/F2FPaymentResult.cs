using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Alipay.Enums;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.Results
{
    public class F2FPaymentResult: AlipayPaymentResultBase
    {
        protected F2FPaymentResult(JObject json) : base(json)
        {
            BuyerLogonId = json.GetString("buyer_logon_id");
            TotalAmount = json.GetDecimal("total_amount");
            ReceiptAmount = json.GetDecimal("receipt_amount");
            BuyerPayAmount = json.GetDecimal("buyer_pay_amount");
            PointAmount = json.GetDecimal("point_amount");
            InvoiceAmount = json.GetDecimal("invoice_amount");
            PaymentDt = json.GetString("gmt_payment").ToDateTime();
            StoreName = json.GetString("store_name");
            BuyerUserId = json.GetString("buyer_user_id");
            AsyncPaymentMode = json.GetString("async_payment_mode")
                .Switch(AsyncPaymentMode.ASYNC_DELAY_PAY)
                .Case("ASYNC_DELAY_PAY",AsyncPaymentMode.ASYNC_DELAY_PAY)
                .Case("ASYNC_REALTIME_PAY", AsyncPaymentMode.ASYNC_REALTIME_PAY)
                .Case("SYNC_DIRECT_PAY", AsyncPaymentMode.SYNC_DIRECT_PAY)
                .Case("NORMAL_ASYNC_PAY", AsyncPaymentMode.NORMAL_ASYNC_PAY)
                .Case("QUOTA_OCCUPYIED_ASYNC_PAY", AsyncPaymentMode.QUOTA_OCCUPYIED_ASYNC_PAY)
                .Result
                ;
        }

        /// <summary>
        /// 买家支付宝账号
        /// </summary>
        public string BuyerLogonId {protected set; get; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TotalAmount { protected set; get; }

        /// <summary>
        /// 实收金额    
        /// </summary>
        public decimal ReceiptAmount { protected set; get; }

        /// <summary>
        /// 买家付款的金额	
        /// </summary>
        public decimal BuyerPayAmount { protected set; get; }

        /// <summary>
        /// 使用集分宝付款的金额 
        /// </summary>
        public decimal PointAmount { protected set; get; }

        /// <summary>
        /// 交易中可给用户开具发票的金额
        /// </summary>
        public decimal InvoiceAmount { protected set; get; }

        /// <summary>
        /// 交易支付时间	
        /// </summary>
        public DateTime PaymentDt { protected set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string StoreName { protected set; get; }

        public string BuyerUserId { protected set; get; }

        /// <summary>
        /// 异步支付模式
        /// </summary>
        public AsyncPaymentMode AsyncPaymentMode { protected set; get; }
    }
}
