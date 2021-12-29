using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kugar.Payment.Wechatpay.Enums
{
    /// <summary>
    /// 退款资金来源
    /// </summary>
    public enum  RefundAccount
    {
        /// <summary>
        /// 可用余额退款/基本账户
        /// </summary>
        [Description("可用余额退款/基本账户")]
        RefundSourceRechargeFunds,

        /// <summary>
        /// 未结算资金退款
        /// </summary>
        [Description("未结算资金退款")]
        RefundSourceUnsettledFunds
    }
}
