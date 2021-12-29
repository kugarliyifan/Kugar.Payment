using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Wechatpay.Enums
{
    /// <summary>
    /// 优惠券类型
    /// </summary>
    public enum CouponType
    {
        /// <summary>
        /// 非充值代金券
        /// </summary>
        Cash,

        /// <summary>
        /// 充值代金券
        /// </summary>
        NoCash
    }
}
