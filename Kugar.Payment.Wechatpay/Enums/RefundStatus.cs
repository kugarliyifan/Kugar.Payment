using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kugar.Payment.Wechatpay.Enums
{
    /// <summary>
    /// 退款状态
    /// </summary>
    public enum RefundStatus
    {
        /// <summary>
        /// 退款成功
        /// </summary>
        [Description("退款成功")]
        Success,

        /// <summary>
        /// 退款关闭，指商户发起退款失败的情况
        /// </summary>
        [Description("退款关闭")]
        RefundClose,

        /// <summary>
        /// 退款处理中
        /// </summary>
        [Description("退款处理中")]
        Processing,

        /// <summary>
        /// 退款异常，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败
        /// </summary>
        [Description("退款异常")]
        Change,

        /// <summary>
        /// 状态值无效
        /// </summary>
        Error
    }
}
