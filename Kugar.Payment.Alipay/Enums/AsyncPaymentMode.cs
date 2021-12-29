using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Alipay.Enums
{
    /// <summary>
    /// 异步支付模式
    /// </summary>
    public enum  AsyncPaymentMode
    {
        /// <summary>
        /// 异步延时付款
        /// </summary>
        ASYNC_DELAY_PAY,

        /// <summary>
        /// 异步准实时付款
        /// </summary>
        ASYNC_REALTIME_PAY,

        /// <summary>
        /// 同步直接扣款
        /// </summary>
        SYNC_DIRECT_PAY,

        /// <summary>
        /// 纯异步付款
        /// </summary>
        NORMAL_ASYNC_PAY,

        /// <summary>
        /// 异步支付并且预占了先享后付额度
        /// </summary>
        QUOTA_OCCUPYIED_ASYNC_PAY
    }
}
