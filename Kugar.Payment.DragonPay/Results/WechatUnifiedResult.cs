using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class WechatUnifiedResult:DragonResultBase
    {
        public WechatUnifiedResult(JObject json) : base(json)
        {
            this.IsSuccess = json.GetBool("SUCCESS");

            AppId = json.GetString("appId");
            TimeStamp = json.GetString("timeStamp");
            NonceStr = json.GetString("nonceStr");
            Package = json.GetString("package");
            SignType = json.GetString("signType");
            PaySign = json.GetString("paySign");
            PartnerId = json.GetString("partnerid");
            PrepayId = json.GetString("prepayid");
            MWebUrl = json.GetString("mweb_url");
            
        }

        /// <summary>
        /// 微信分配的APPID
        /// </summary>
        public string AppId { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { set; get; }

        /// <summary>
        /// 随机串
        /// </summary>
        public string NonceStr { set; get; }

        /// <summary>
        /// 数据包
        /// </summary>
        public string Package { set; get; }

        /// <summary>
        /// 签名方式
        /// </summary>
        public string SignType { set; get; }

        /// <summary>
        /// 签名数据
        /// </summary>
        public string PaySign { set; get; }

        /// <summary>
        /// 子商户的商户号
        /// </summary>
        public string PartnerId { set; get; }

        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string PrepayId { set; get; }

        /// <summary>
        /// 微信H5支付中间页面URL
        /// </summary>
        public string MWebUrl { set; get; }
    }
}
