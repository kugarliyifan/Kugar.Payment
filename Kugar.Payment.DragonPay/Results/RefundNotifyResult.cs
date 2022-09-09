using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;

namespace Kugar.Payment.DragonPay.Results
{
    public class RefundNotifyResult:ResultBase, ICommonRefundResult
    {
        public RefundNotifyResult(IReadOnlyDictionary<string, string> source)
        {
            IsSuccess = true;

            TransactionId = source.TryGetValue("transaction_id");
            OutTradeNo = source.TryGetValue("out_trade_no");
            RefundId = source.TryGetValue("refund_id");
            OutRefundNo = source.TryGetValue("out_refund_no"); 

        }

        public string TransactionId { get; }
        public string OutTradeNo { get; }
        public string OutRefundNo { get; }
        public string RefundId { get; }
    }
}
