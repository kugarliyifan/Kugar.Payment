using System;
using System.Collections.Generic;
using System.Text;
using Aop.Api; 
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.Results
{
    public abstract class AlipayResultBase
    {
        protected AlipayResultBase(JObject json)
        {
            Code = json.GetString("code");
            Msg = json.GetString("msg");
            SubCode = json.GetString("sub_code");
            SubMsg = json.GetString("sub_msg");

            RawData = json;
        }

        public string Code {protected set; get; }

        public string Msg { protected set; get; }

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

            if (Code == "10000")
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }
        }

        public string TransactionId { get;protected set; }

        public string OutTradeNo { get; protected set; }

        public bool IsSuccess {protected  set; get; }
        
    }


}
