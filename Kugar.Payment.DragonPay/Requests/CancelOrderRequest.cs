using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Payment.DragonPay.Enums;

namespace Kugar.Payment.DragonPay.Requests
{
    public class CancelOrderRequest:DragonRequestBase
    {
        public CancelOrderRequest(DragonPayConfig config) : base(config)
        {
        }

        /// <summary>
        /// 订单号Id
        /// </summary>
        public string OrderId { set; get; }

        /// <summary>
        /// 二维码类型
        /// </summary>
        public QrCodeType QrCodeType { set; get;  }

        public override string ToUrl()
        {
            if (string.IsNullOrWhiteSpace(OrderId))
            {
                throw new ArgumentNullException(nameof(OrderId));
            }

            var sb = new StringBuilder();
            
            sb.Append("TXCODE=PAY103&")
                .AppendFormat("MERFLAG={0}&", string.IsNullOrWhiteSpace(Config.PosId) ? "2" : "1")
                .AppendFormat("ORDERID={0}&", OrderId)
                .AppendFormat("QRCODETYPE={0}", (int)QrCodeType);
            

            if (sb[^1] == '&')
            {
                return sb.ToString(0, sb.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }
    }
}
