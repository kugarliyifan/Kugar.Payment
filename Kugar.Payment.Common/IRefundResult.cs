using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Common
{
    public interface ICommonRefundResult
    {
        /// <summary>
        /// 退款单交易号
        /// </summary>
        string TransactionId { get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符），
        /// </summary>
        string OutTradeNo { get; }

        /// <summary>
        /// 退款单号
        /// </summary>
        string OutRefundNo { get; }

        string RefundId { get; }

        bool IsSuccess { get; }
    }
}
