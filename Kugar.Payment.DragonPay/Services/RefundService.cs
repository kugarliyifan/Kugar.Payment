using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Flurl.Http;
using Kugar.Core.BaseStruct;
using Kugar.Payment.DragonPay.Requests;
using Kugar.Payment.DragonPay.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Services
{
    public class RefundService:DragonServiceBase
    {
        private RefundOrderRequest _request = null;

        public RefundService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new RefundOrderRequest(config);
        }

        public RefundService Amount(decimal amount)
        {
            if (amount<=0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "退款金额必须大于0");
            }

            _request.Amount = amount;
            return this;
        }

        public RefundService OrderId(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new ArgumentNullException(nameof(orderId), "订单编号必填");
            }

            _request.OrderId = orderId;
            return this;
        }

        public RefundService RefundOrderId(string refundOrderId)
        {
            if (string.IsNullOrWhiteSpace(refundOrderId))
            {
                throw new ArgumentNullException(nameof(refundOrderId), "订单编号必填");
            }

            _request.RefundOrderId = refundOrderId;
            return this;
        }

        public async Task<ResultReturn<RefundOrderResult>> ExecuteAsync()
        {
            var s1 = _request.Check();

            if (!s1)
            {
                return s1.Cast<RefundOrderResult>(null);
            }

            var xml = _request.ToUrl();

            try
            {
                var s = await ("127.0.0.1:" + Config.RefundPort)
                    .WithHeader("content-type","application/xml")
                    .PostStringAsync(xml);

                var resultStr =await s.GetStringAsync();

                var resultXmlDoc = new XmlDocument();

                resultXmlDoc.Load(resultStr);
            
                var result = new RefundOrderResult(resultXmlDoc);

                return new SuccessResultReturn<RefundOrderResult>(result);
            }
            catch (Exception e)
            {
                return new FailResultReturn<RefundOrderResult>(e);
            }
            
        }
    }
}
