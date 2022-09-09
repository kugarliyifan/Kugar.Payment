using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.BaseStruct;

namespace Kugar.Payment.DragonPay.Requests
{
    public class RefundOrderRequest:DragonRequestBase
    {
        public RefundOrderRequest(DragonPayConfig config) : base(config)
        {
        }

        public decimal Amount { set; get; }

        public string OrderId { set; get; }

        public string RefundOrderId { set; get; }

        public override ResultReturn Check()
        {
            if (Amount<=0)
            {
                return new FailResultReturn("退款金额必须大于0");
            }

            if (string.IsNullOrWhiteSpace(OrderId))
            {
                return new FailResultReturn("商户交易单号必填");
            }

            return base.Check();
        }

        public override string ToUrl()
        {
            var xml = @$"<?xml version=""1.0"" encoding=""GB2312"" standalone=""yes"" ?> 
                        <TX> 
                          <REQUEST_SN>{DateTime.Now:yyyyMMddHHmmssffff}</REQUEST_SN> 
                          <CUST_ID>{Config.MerchantId}</CUST_ID> 
                          <USER_ID>{Config.RefundOperatorName}</USER_ID> 
                          <PASSWORD>{Config.RefundOperatorPassword}</PASSWORD> 
                          <TX_CODE>5W1004</TX_CODE> 
                          <LANGUAGE>CN</LANGUAGE> 
                          <TX_INFO> 
                            <MONEY>{this.Amount:F2}</MONEY> 
                            <ORDER>{this.OrderId}</ORDER> 
                            <REFUND_CODE>{this.RefundOrderId}</REFUND_CODE> 
                          </TX_INFO> 
                          <SIGN_INFO></SIGN_INFO> 
                          <SIGNCERT></SIGNCERT> 
                        </TX> 
                        ";

            return xml;
        }
    }
}
