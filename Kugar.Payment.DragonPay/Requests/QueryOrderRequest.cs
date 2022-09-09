using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Payment.DragonPay.Enums;

namespace Kugar.Payment.DragonPay.Requests
{
    public class QueryScanQrCodeOrderRequest:DragonRequestBase
    {
        public QueryScanQrCodeOrderRequest(DragonPayConfig config) : base(config)
        {
        }

        /// <summary>
        /// 单据号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 上送查询的次数，从1开始
        /// </summary>
        public int QueryTime { set; get; }


        public bool ReturnOpenId { set; get; }

        /// <summary>
        /// 支付二维码类型..与AuthCode必须选填一个
        /// </summary>
        public QrCodeType? QrCodeType { set; get; }

        /// <summary>
        /// 查询的扫码内容,与QrCodeType必须选填一个
        /// </summary>
        public string AuthCode { set; get; }


        public override string ToUrl()
        {
            var sb = new StringBuilder();

            sb.Append("TXCODE=PAY101&")
                .AppendFormat("MERFLAG={0}&", string.IsNullOrWhiteSpace(Config.PosId) ? "2" : "1")
                .AppendFormat("ORDERID={0}&", OrderId)
                .AppendFormat("QRYTIME={0}&",QueryTime);


            if (QrCodeType.HasValue)
            {
                sb.AppendFormat("QRCODETYPE={0}&", (int)QrCodeType.Value);
            }
            else if (!string.IsNullOrWhiteSpace(AuthCode))
            {
                sb.AppendFormat("QRCODE={0}&", AuthCode);
            }
            else
            {
                throw new Exception("QrCodeType或AuthCode必须选填一个");
            }


            if (!string.IsNullOrWhiteSpace(Config.WechatpayAppId))
            {
                sb.AppendFormat("SUB_APPID={0}&", Config.WechatpayAppId);
            }

            sb.AppendFormat("RETURN_FIELD={0}&",
                $"{(ReturnOpenId ? "1" : "0")}001".PadRight(20, '0'));
 

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
