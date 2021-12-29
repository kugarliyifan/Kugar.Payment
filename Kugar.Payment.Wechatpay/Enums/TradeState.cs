using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kugar.Payment.Wechatpay.Enums
{
    /// <summary>
    /// 交易状态	
    /// </summary>
    public enum TradeState
    {
        /// <summary>
        /// 交易状态	
        /// </summary>
        [Description("支付成功")]
        Success,

        /// <summary>
        /// 转入退款
        /// </summary>
        [Description("转入退款")]
        Refund,

        /// <summary>
        /// 未支付
        /// </summary>
        [Description("未支付")]
        Notpay,

        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed,

        /// <summary>
        /// 已撤销(刷卡支付)
        /// </summary>
        [Description("已撤销(刷卡支付)")]
        Revoked,

        /// <summary>
        /// 用户支付中
        /// </summary>
        [Description("用户支付中")]
        Userpaying,

        /// <summary>
        /// 支付失败(其他原因，如银行返回失败)
        /// </summary>
        [Description("支付失败(其他原因，如银行返回失败)")]
        Payerror,

        /// <summary>
        /// 已接收，等待扣款
        /// </summary>
        [Description("已接收，等待扣款")]
        Accept
    }
}
