using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;

namespace Kugar.Payment.Wechatpay.Results
{
    public class RefundNotifyResult: ResultBase
    {
        public RefundNotifyResult(IReadOnlyDictionary<string, string> source)
        {
            IsSuccess = true;

            TransactionId = source.TryGetValue("transaction_id");
            OutTradeNo = source.TryGetValue("out_trade_no");
            RefundId = source.TryGetValue("refund_id");
            OutRefundNo = source.TryGetValue("out_refund_no");
            TotalFee = source.TryGetValue("total_fee").ToDecimal()/100;
            SettlementTotalFee = source.TryGetValue("settlement_total_fee").ToDecimal() / 100;
            SettlementRefundFee = source.TryGetValue("settlement_refund_fee").ToDecimal() / 100;
            RefundFee = source.TryGetValue("refund_fee").ToDecimal() / 100;
            
            this.Status = source.TryGetValue($"refund_status")
                    .Switch(RefundStatus.Change)
                    .Case("SUCCESS", RefundStatus.Success)
                    .Case("REFUNDCLOSE", RefundStatus.RefundClose)
                    .Case("PROCESSING", RefundStatus.Processing)
                    .Case("CHANGE", RefundStatus.Change)
                    .Result
                ;

            RefundSuccessTime = source.TryGetValue("success_time").ToDateTimeNullable("yyyy-MM-dd HH:mm:ss");
            RefundRecvAccout = source.TryGetValue("refund_recv_accout");
            this.RefundAccount = source.TryGetValue($"refund_account")
                .Switch(RefundAccount.RefundSourceRechargeFunds)
                .Case("REFUND_SOURCE_RECHARGE_FUNDS", RefundAccount.RefundSourceRechargeFunds)
                .Case("REFUND_SOURCE_UNSETTLED_FUNDS", RefundAccount.RefundSourceUnsettledFunds)
                .Result;
            RefundRequestSource = source.TryGetValue("refund_request_source"); 


        }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string TransactionId { set; get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符），
        /// </summary>
        public string OutTradeNo { set; get; }

 
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal TotalFee { set; get; }

        /// <summary>
        /// 商户退款单号	
        /// </summary>
        public string OutRefundNo { set; get; }

        /// <summary>
        /// 申请退款金额	
        /// </summary>
        public decimal RefundFee { set; get; }

        /// <summary>
        /// 微信退款单号
        /// </summary>
        public string RefundId { set; get; }

        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额
        /// </summary>
        public decimal SettlementTotalFee { set; get; }

        /// <summary>
        /// 退款金额=申请退款金额-非充值代金券退款金额，退款金额<=申请退款金额
        /// </summary>
        public decimal SettlementRefundFee { set; get; }

        /// <summary>
        /// 退款状态	
        /// </summary>
        public RefundStatus Status { set; get; }

        /// <summary>
        /// 取当前退款单的退款入账方
        /// </summary>
        public string RefundRecvAccout { set; get; }

        /// <summary>
        /// 退款资金来源	
        /// </summary>
        public RefundAccount RefundAccount { set; get; }
         
        /// <summary>
        /// 退款成功时间	
        /// </summary>
        public DateTime? RefundSuccessTime { set; get; }

        /// <summary>
        /// 退款发起来源
        /// </summary>
        public string RefundRequestSource { set; get; }

    }
}
