using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.DragonPay.Enums;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class CancelOrderResult:DragonResultBase
    {
        public CancelOrderResult(JObject json) : base(json)
        {
            ReCall =  json.GetString("ReCall")=="Y";
            OrderId = json.GetString("ORDERID");
        }

        /// <summary>
        /// 从请求参数中获得
        /// </summary>
        public string OrderId { set; get; }

        /// <summary>
        /// 是否需要继续调用订单关闭交易
        /// </summary>
        public bool ReCall { set; get; } 
    }
}
