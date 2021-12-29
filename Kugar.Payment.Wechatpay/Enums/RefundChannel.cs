using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kugar.Payment.Wechatpay.Enums
{
    /// <summary>
    /// 退款渠道	
    /// </summary>
    public enum  RefundChannel
    {
        /// <summary>
        /// 原路退款
        /// </summary>
        [Description("原路退款")]
        Original,

        /// <summary>
        /// 退回到余额
        /// </summary>
        [Description("退回到余额")]
        Balance,

        /// <summary>
        /// 原账户异常退到其他余额账户
        /// </summary>
        [Description("原账户异常退到其他余额账户")]
        OtherBalance,

        /// <summary>
        /// 原银行卡异常退到其他银行卡
        /// </summary>
        [Description("原银行卡异常退到其他银行卡")]
        OtherBankcard
    }
}
