using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.DragonPay.Requests
{
    public class ScanQrcodeRequest : DragonRequestBase
    {
        public string OrderId { set; get; }

        public decimal Amount { set; get; }

        /// <summary>
        /// 是否返回OpenId
        /// </summary>
        public bool ReturnOpenId { set; get; }

        /// <summary>
        /// 扫码内容
        /// </summary>
        public string AuthCode { set; get; }

        /// <summary>
        /// 用户订单中查看到的标题
        /// </summary>
        public string Body { set; get; }


        public override string ToUrl()
        {
            var sb = new StringBuilder();

            sb.Append("TXCODE=PAY100&")
                .AppendFormat("MERFLAG={0}&", string.IsNullOrWhiteSpace(Config.PosId) ? "2" : "1")
                .AppendFormat("ORDERID={0}&", OrderId)
                .AppendFormat("QRCODE={0}&", AuthCode)
                .AppendFormat("AMOUNT={0:f2}&", Amount);

            //var s = $"&MERFLAG=1&TERMNO1=&TERMNO2=&ORDERID={OrderId}" +
            //        $"&QRCODE={AuthCode}&AMOUNT={Amount:f2}&TXCODE=PAY100&PROINFO={Body}&REMARK1=&REMARK2=&SMERID=&SMERNAME=&SMERTYPEID=" +
            //        "&SMERTYPE=&TRADECODE=&TRADENAME=&SMEPROTYPE=&PRONAME=";

            //return s;

            if (!string.IsNullOrWhiteSpace(Config.WechatpayAppId))
            {
                sb.AppendFormat("SUB_APPID={0}&", Config.WechatpayAppId);
            }

            sb.AppendFormat("RETURN_FIELD={0}&",
                $"{(ReturnOpenId ? "1" : "0")}0{(string.IsNullOrWhiteSpace(Body) ? "0" : "1")}1".PadRight(20, '0'));

            if (!string.IsNullOrWhiteSpace(Body))
            {
                sb.AppendFormat("PROINFO={0}", Body);
            }

            if (sb[^1] == '&')
            {
                return sb.ToString(0, sb.Length - 1);
            }
            else
            {
                return sb.ToString();
            }
        }

        public ScanQrcodeRequest(DragonPayConfig config) : base(config)
        {
        }
    }
}
