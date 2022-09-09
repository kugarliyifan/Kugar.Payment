using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kugar.Payment.DragonPay.Enums
{
    /// <summary>
    /// 当前微信账号类型
    /// </summary>
    public enum WechatType
    {
        /// <summary>
        /// 公众号
        /// </summary>
        [Description("公众号")]
        MP,

        /// <summary>
        /// 小程序
        /// </summary>
        [Description("小程序")]
        MiniApp
    }
}
