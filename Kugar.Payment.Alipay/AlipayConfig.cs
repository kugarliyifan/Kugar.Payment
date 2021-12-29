using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Payment.Alipay.Enums;

namespace Kugar.Payment.Alipay
{
    public class AlipayConfig
    {
        public string AppId { set; get; }

        public string GatewayUrl { set; get; } = "https://openapi.alipay.com/gateway.do";

        public string AlipayPublicKey { set; get; }

        public string PrivateKey { set; get; }

        public SignType SignType { set; get; } = SignType.RSA2;

        public string Charset { set; get; } = "utf-8";

        public string NotifyUrl { set; get; }

        /// <summary>
        /// 应用授权
        /// </summary>
        public string AppAuthToken { set; get; }
    }
}
