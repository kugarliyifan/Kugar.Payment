using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fasterflect;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Wechatpay.Services;
using OneOf;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Results;

namespace Kugar.Payment.Wechatpay.Services
{
    public class RefundService : WechatpayServiceBase
    {
        private string _transaction_id = string.Empty;
        private string _orderNo=String.Empty;
        private decimal _totalAmount = 0m;
        private decimal _refundAmount = 0m;
        private string _refundOrderNo = "";
        private string _refund_desc = "";
        private string _notifyUrl = "";

        public RefundService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 付款单交易号
        /// </summary>
        /// <param name="transaction_id"></param>
        /// <returns></returns>
        public RefundService TransactionId(string transaction_id)
        {
            _transaction_id = transaction_id;
            return this;
        }

        /// <summary>
        /// 付款单,,单号
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public RefundService OutTradeNo(string orderNo)
        {
            _orderNo = orderNo;
            return this;
        }

        /// <summary>
        /// 单据总金额
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public RefundService TotalAmount(decimal amount)
        {
            _totalAmount = amount;
            return this;
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public RefundService RefundAmount(decimal amount)
        {
            _refundAmount = amount;
            return this;
        }

        /// <summary>
        /// 退款单号
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public RefundService RefundOrderNo(string orderNo)
        {
            _refundOrderNo = orderNo;
            return this;
        }

        /// <summary>
        /// 异步通知地址,如果不填,则使用默认通知地址
        /// </summary>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public RefundService NotifyUrl(string notifyUrl)
        {
            _notifyUrl = notifyUrl;
            return this;
        }

        /// <summary>
        /// 退款原因
        /// </summary>
        /// <param name="desc">退款原因</param>
        /// <returns></returns>
        public RefundService RefundDesc(string desc)
        {
            this._refund_desc = desc;
            return this;
        }

        public async Task<ResultReturn<RefundResult>> ExecuteAsync()
        {
            var dic = new Dictionary<string, OneOf<int, string>>();

            if (string.IsNullOrWhiteSpace(_transaction_id) && string.IsNullOrWhiteSpace(_orderNo))
            {
                return new FailResultReturn<RefundResult>("transaction_id和OutTradeNo不能同时为空");
            }

            if (!string.IsNullOrWhiteSpace(_refundOrderNo))
            {
                return new FailResultReturn<RefundResult>("RefundOrderNo不能为空");
            }

            if (_refundAmount <= 0)
            {
                return new FailResultReturn<RefundResult>("RefundAmount必须大于0");
            }

            if (!string.IsNullOrWhiteSpace(_transaction_id))
            {
                dic.AddOrUpdate("transaction_id", _transaction_id);
            }
            else
            {
                dic.AddOrUpdate("out_trade_no", _orderNo);
            }
             
            dic.AddOrUpdate("total_fee", (int)(_totalAmount * 100));//订单总金额
            dic.AddOrUpdate("refund_fee", (int)(_refundAmount * 100));//退款金额
            dic.AddOrUpdate("out_refund_no", _refundOrderNo);//随机生成商户退款单号
            dic.AddOrUpdate("op_user_id", Config.MchId);//操作员，默认为商户号
            dic.AddOrUpdate("refund_desc", _refund_desc);

            var notifyUrl = string.IsNullOrWhiteSpace(_notifyUrl) ? Config.RefundNotifyUrl : _notifyUrl;
            
            dic.AddIf(!string.IsNullOrWhiteSpace(notifyUrl), "notify_url", notifyUrl);


            string url = $"{Config.GatewayHost}/secapi/pay/refund";

            var result = await PostData(url, dic);

            if (result.IsSuccess && CheckIsSuccess(result.ReturnData))
            {
                return new SuccessResultReturn<RefundResult>(new RefundResult(result.ReturnData));
            }
            else
            {
                return new FailResultReturn<RefundResult>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx());
            }
        }
    }

    /// <summary>
    /// 退款操作返回结果
    /// </summary>
    public class RefundResult:WechatPayResultBase
    {
        public RefundResult(IReadOnlyDictionary<string, string> source):base(source)
        {
            if (IsSuccess)
            {
                TransactionId = source.TryGetValue("transaction_id","");
                OutTradeNo = source.TryGetValue("out_trade_no");
                OutRefundNo = source.TryGetValue("out_refund_no");
                RefundId = source.TryGetValue("refund_id");
                RefundFee = source.TryGetValue("refund_fee").ToDecimal() / 100;
                SettlementRefundFee= source.TryGetValue("settlement_refund_fee").ToDecimal() / 100;
                TotalFee= source.TryGetValue("total_fee").ToDecimal() / 100;
                SettlementTotalFee= source.TryGetValue("settlement_total_fee").ToDecimal() / 100;
            }
        }

        /// <summary>
        ///  商户系统内部的退款单号，商户系统内部唯一
        /// </summary>
        public string OutRefundNo { set; get; }

        /// <summary>
        /// 微信退款单号
        /// </summary>
        public string RefundId { set; get; }

        /// <summary>
        /// 退款总金额,已自动换算为元,可以做部分退款
        /// </summary>
        public decimal RefundFee { set; get; }

        /// <summary>
        /// 已自动换算为元  去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额<=申请退款金额
        /// </summary>
        public decimal SettlementRefundFee { set; get; }

        /// <summary>
        /// 订单总金额,已自动换算为元
        /// </summary>
        public decimal TotalFee { set; get; }

        /// <summary>
        /// 去掉非充值代金券金额后的订单总金额，应结订单金额=订单金额-非充值代金券金额，应结订单金额 <= 订单金额 已自动换算为元
        /// </summary>
        public decimal SettlementTotalFee { set; get; }

        public static implicit operator ResultReturn<string>(RefundResult d)
        {
            return new ResultReturn<string>(d.IsSuccess, d.RefundId, message: d.IsSuccess ? "" : $"code={d.Err_Code},desc={d.Err_Code_Des}");
        }
    }
}