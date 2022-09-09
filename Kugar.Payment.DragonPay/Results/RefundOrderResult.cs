using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Kugar.Core.ExtMethod;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class RefundOrderResult:DragonResultBase
    {
        public RefundOrderResult(XmlDocument xml) : base(null)
        {
            IsSuccess = xml.GetFirstElementsByTagName("RETURN_CODE").InnerText == "000000";

            Code =xml.GetFirstElementsByTagName("RETURN_CODE").InnerText;
            Message = xml.GetFirstElementsByTagName("RETURN_MSG").InnerText;
            RefundSN = xml.GetFirstElementsByTagName("REQUEST_SN").InnerText;

            if (IsSuccess)
            {
                PayAmount = xml.GetFirstElementsByTagName("PAY_AMOUNT").ToDecimal();
                RefundAmount = xml.GetFirstElementsByTagName("AMOUNT").ToDecimal();
                OrderId = xml.GetFirstElementsByTagName("ORDER_NUM")?.InnerText;
            }
            
        }

        public string RefundSN { set; get; }

        public string OrderId { set; get; }

        public decimal PayAmount { set; get; }

        public decimal RefundAmount { set; get; }


    }
}
