using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Payment.Common;

namespace Kugar.Payment.DragonPay
{
    public class DragonPayConfig:ConfigBase
    {

        public DragonPayConfig() : base()
        {
            this.GatewayHost = "https://ibsbjstar.ccb.com.cn";
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantId { set; get; }

        /// <summary>
        /// 柜台号
        /// </summary>
        public string PosId { set; get; }

        /// <summary>
        /// 分行号
        /// </summary>
        public string BranchId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string PubKey { set; get; }

        /// <summary>
        /// 绑定的微支付AppId,可以不填
        /// </summary>
        public string WechatpayAppId { set; get; }

    }
}
