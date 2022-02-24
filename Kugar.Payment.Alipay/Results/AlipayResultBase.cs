using System;
using System.Collections.Generic;
using System.Text;
using Aop.Api; 
using Kugar.Core.ExtMethod;
using Kugar.Payment.Alipay.Enums;
using Kugar.Payment.Common;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.Results
{
    public abstract class AlipayResultBase:ResultBase
    {
        protected AlipayResultBase(JObject json)
        {
            Code = json.GetString("code");
            Message = json.GetString("msg");
            SubCode = json.GetString("sub_code");
            SubMsg = json.GetString("sub_msg");

            RawData = json;
        }

        //public string Code {protected set; get; }

        //public string Msg { protected set; get; }

        public string SubCode { protected set; get; }

        public string SubMsg { protected set; get; }
        
        public JObject RawData { protected set; get; }
    }

    public class AlipayPaymentResultBase: AlipayResultBase,ICommonPaymentResult
    {
        protected AlipayPaymentResultBase(JObject json) : base(json)
        {
            TransactionId = json.GetString("trade_no");
            OutTradeNo = json.GetString("out_trade_no");
            SellerId = json.GetString("seller_id");
            TotalAmount = json.GetDecimal("total_amount");

            TradeStatus = json.GetString("trade_status")
                .Switch(PaymentTradeStatus.TRADE_CLOSED)
                .Case("WAIT_BUYER_PAY", PaymentTradeStatus.WAIT_BUYER_PAY)
                .Case("TRADE_CLOSED", PaymentTradeStatus.TRADE_CLOSED)
                .Case("TRADE_SUCCESS", PaymentTradeStatus.TRADE_SUCCESS)
                .Case("TRADE_FINISHED", PaymentTradeStatus.TRADE_FINISHED)
                .Result;


            IsSuccess = TradeStatus == PaymentTradeStatus.TRADE_SUCCESS ||
                        TradeStatus == PaymentTradeStatus.TRADE_FINISHED;


            //if (Code == "10000")
            //{
            //    IsSuccess = true;
            //}
            //else
            //{
            //    IsSuccess = false;
            //}
        }

        public string SellerId { set; get; }

        public string TransactionId { get;protected set; }

        public string OutTradeNo { get; protected set; }

        public bool IsSuccess {protected  set; get; }

        public decimal TotalAmount { get; set; }

        public PaymentTradeStatus TradeStatus { set; get; }
    }


}
