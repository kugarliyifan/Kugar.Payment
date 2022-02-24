using System.Collections.Generic;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;
using Kugar.Payment.Wechatpay.Results;

namespace Kugar.Payment.Wechatpay.Services
{
    public class QueryInfoResult : NotifyPaymentResult
    {
        public QueryInfoResult(IReadOnlyDictionary<string, string> source) : base(source)
        {
            TradeState = source.TryGetValue("trade_state")
                .Switch(TradeState.Notpay)
                .Case("SUCCESS", TradeState.Success)
                .Case("REFUND", TradeState.Refund)
                .Case("NOTPAY", TradeState.Notpay)
                .Case("CLOSED", TradeState.Closed)
                .Case("REVOKED", TradeState.Revoked)
                .Case("USERPAYING", TradeState.Userpaying)
                .Case("PAYERROR", TradeState.Payerror)
                .Case("ACCEPT", TradeState.Accept)
                .Result;

            this.IsSuccess = source.TryGetValue("return_code") == "SUCCESS" &&
                             source.TryGetValue("result_code") == "SUCCESS" &&
                             source.TryGetValue("trade_state") == "SUCCESS";

            if (TradeState== TradeState.Revoked)
            {
                this.IsCanceled = true;
            }
        }

        /// <summary>
        /// 交易状态
        /// </summary>
        public TradeState TradeState { set; get; }
    }
}