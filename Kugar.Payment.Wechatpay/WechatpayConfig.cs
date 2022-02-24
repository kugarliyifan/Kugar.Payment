using System;
using System.Collections.ObjectModel;
using System.Threading;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Payment.Common;
using Kugar.Payment.Wechatpay.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace Kugar.Payment.Wechatpay
{
    public class WechatpayConfig: ConfigBase
    {
        public WechatpayConfig() : base()
        {
            this.GatewayHost = "https://api.mch.weixin.qq.com";
        }

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

        public SignType SignType { set; get; } = SignType.MD5;

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
        public string PaymentNotifyUrl { set; get; } = "/Core/Payment/Callback/Wechatpay/{appID}";

        /// <summary>
        /// 退款通知默认地址
        /// </summary>
        public string RefundNotifyUrl { set; get; } = "/Core/Refund/Callback/Wechatpay/{appID}";
        
    }

    
}
