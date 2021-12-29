using System.Collections.ObjectModel;
using System.Threading;
using Kugar.Core.Log;
using Kugar.Payment.Wechatpay.Enums;
using OneOf.Types;

namespace Kugar.Payment.Wechatpay
{
    public class WechatpayConfig
    {
        /// <summary>
        /// 绑定支付的APPID（必须配置）    
        /// </summary>
        public string AppId { set; get; }

        /// <summary>
        /// 商户号（必须配置）
        /// </summary>
        public string MchId { set; get; }

        /// <summary>
        /// 商户支付密钥，参考开户邮件设置（必须配置），请妥善保管，避免密钥泄露
        /// </summary>
        public string PayKey { set; get; }

        public SignType SignType { set; get; } = SignType.SHA256;

        /// <summary>
        /// 公众帐号secert（仅JSAPI支付的时候需要配置），请妥善保管，避免密钥泄露
        /// </summary>
        public string AppSecret { set; get; }

        /// <summary>
        /// 证书密码.如果为空,默认为MchId
        /// </summary>
        public string CertPassword { set; get; }

        /// <summary>
        /// 证书数据
        /// </summary>
        public byte[] CertData { set; get; }

        /// <summary>
        /// 默认异步通知地址
        /// </summary>
        public string PaymentNotifyUrl { set; get; }

        /// <summary>
        /// 退款通知默认地址
        /// </summary>
        public string RefundNotifyUrl { set; get; }

        /// <summary>
        /// 微支付网关域名
        /// </summary>
        public string GatewayHost { set; get; } = "https://api.mch.weixin.qq.com";
    }
}
