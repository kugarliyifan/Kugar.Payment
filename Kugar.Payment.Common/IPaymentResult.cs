using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Common
{
    /// <summary>
    /// 通用支付结果
    /// </summary>
    public interface ICommonPaymentResult
    {
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        string TransactionId {get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符），
        /// </summary>
        string OutTradeNo {  get; }

        bool IsSuccess {  get; }

        /// <summary>
        /// 本次交易支付的订单金额,单位为元
        /// </summary>
        public decimal TotalAmount { set; get; }

        string RawResult { set; get; }
    }
}
