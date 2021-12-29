using Kugar.Payment.Wechatpay.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;

namespace Kugar.Payment.Wechatpay.Results
{
    /// <summary>
    /// 退款查询结果
    /// </summary>
    public class RefundQueryResult : WechatPayResultBase
    {
        internal RefundQueryResult(IReadOnlyDictionary<string, string> source) : base(source)
        {
            IsSuccess = source.TryGetValue("return_code") == "SUCCESS" &&
                        source.TryGetValue("result_code") == "SUCCESS" &&
                        source.TryGetValue("refund_status")== "SUCCESS"
                        ;

            if (IsSuccess)
            {
                this.TotalRefundCount = source.TryGetValue("total_refund_count").ToInt();
                this.TotalFee = source.TryGetValue("total_fee").ToDecimal() / 100;
                this.SettlementTotalFee = source.TryGetValue("settlement_total_fee").ToDecimal() / 100;
                this.RefundCount = source.TryGetValue("refund_count").ToInt();

                if (RefundCount > 0)
                {
                    this.Logs = Enumerable.Range(0, RefundCount).Select(x => new RefundLogs(source, x)).ToList();
                }
            }
        }

        /// <summary>
        /// 订单总共已发生的部分退款次数，当请求参数传入offset后有返回
        /// </summary>
        public int TotalRefundCount { set; get; }

        /// <summary>
        /// 订单总金额，单位为元
        /// </summary>
        public decimal TotalFee { set; get; }

        /// <summary>
        /// 当前返回退款笔数
        /// </summary>
        public int RefundCount { set; get; }

        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额
        /// </summary>
        public decimal SettlementTotalFee { set; get; }

        /// <summary>
        /// 当前交易单,退款记录
        /// </summary>
        public IReadOnlyCollection<RefundLogs> Logs { set; get; }

        /// <summary>
        /// 交易单退款记录
        /// </summary>
        public class RefundLogs
        {
            internal RefundLogs(IReadOnlyDictionary<string, string> source, int index)
            {
                this.OutRefundNo = source.TryGetValue($"out_refund_no_{index}");
                this.RefundFee = source.TryGetValue($"refund_fee_{index}").ToDecimal() / 100;
                this.RefundId = source.TryGetValue($"refund_id_{index}");
                this.Status = source.TryGetValue($"refund_status_{index}")
                    .Switch(RefundStatus.Change)
                    .Case("SUCCESS", RefundStatus.Success)
                    .Case("REFUNDCLOSE", RefundStatus.RefundClose)
                    .Case("PROCESSING", RefundStatus.Processing)
                    .Case("CHANGE", RefundStatus.Change)
                    .Result
                    ;
                this.RefundRecvAccout = source.TryGetValue($"refund_recv_accout_{index}");
                this.RefundAccount = source.TryGetValue($"refund_account_{index}")
                    .Switch(RefundAccount.RefundSourceRechargeFunds)
                    .Case("REFUND_SOURCE_RECHARGE_FUNDS", RefundAccount.RefundSourceRechargeFunds)
                    .Case("REFUND_SOURCE_UNSETTLED_FUNDS", RefundAccount.RefundSourceUnsettledFunds)
                    .Result;

                this.RefundChannel = source.TryGetValue($"refund_channel_{index}")
                    .Switch(RefundChannel.Original)
                    .Case("ORIGINAL", RefundChannel.Original)
                    .Case("BALANCE", RefundChannel.Balance)
                    .Case("OTHER_BALANCE", RefundChannel.OtherBalance)
                    .Case("OTHER_BANKCARD", RefundChannel.OtherBankcard)
                    .Result;

                this.RefundSuccessTime = source.TryGetValue($"refund_success_time_{index}").ToDateTimeNullable("yyyy-MM-dd HH:mm:ss");

                this.IsSuccess = this.Status == RefundStatus.Success && this.RefundSuccessTime.HasValue;

            }

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
            /// 退款渠道	
            /// </summary>
            public RefundChannel RefundChannel { set; get; }

            /// <summary>
            /// 退款成功时间	
            /// </summary>
            public DateTime? RefundSuccessTime { set; get; }

            /// <summary>
            /// 当前记录是否退款成功
            /// </summary>
            public bool IsSuccess { set; get; }
        }
    }
}
