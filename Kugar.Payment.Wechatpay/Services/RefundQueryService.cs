using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;
using Kugar.Payment.Wechatpay.Results;
using OneOf;

namespace Kugar.Payment.Wechatpay.Services
{
    /// <summary>
    /// 退款状态查询
    /// </summary>
    public class RefundQueryService:WechatpayServiceBase
    {
        private string _transId = "";
        private string _orderId = "";
        private string _refundOrderId = "";
        private string _refundTransId = "";
        private int? _offset = null;
        private string _notifyUrl = null;

        public RefundQueryService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 微信交易单号
        /// </summary>
        /// <param name="transId"></param>
        /// <returns></returns>
        public RefundQueryService TransactionId(string transId)
        {
            _transId = transId;
            return this;
        }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public RefundQueryService OutTradeNo(string orderId)
        {
            _orderId = orderId;
            return this;
        }

        /// <summary>
        /// 商户系统内部的退款单号，商户系统内部唯一
        /// </summary>
        /// <param name="refundOrderId"></param>
        /// <returns></returns>
        public RefundQueryService OutRefundNo(string refundOrderId)
        {
            _refundOrderId = refundOrderId;
            return this;
        }

        /// <summary>
        /// 微信生成的退款单号，在申请退款接口有返回
        /// </summary>
        /// <param name="refundTransId"></param>
        /// <returns></returns>
        public RefundQueryService RefundId(string refundTransId)
        {
            _refundTransId = refundTransId;
            return this;
        }

        /// <summary>
        /// 偏移量，当部分退款次数超过10次时可使用，表示返回的查询结果从这个偏移量开始取记录
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public RefundQueryService Offset(int offset)
        {
            _offset = offset;
            return this;
        }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public RefundQueryService NotifyUrl(string url)
        {
            _notifyUrl = url;

            return this;
        }

        public async Task<ResultReturn<RefundQueryResult>> ExecuteAsync()
        {
            var query= new Dictionary<string, OneOf<int, string>>();

            if (string.IsNullOrWhiteSpace(_transId) &&
                string.IsNullOrWhiteSpace(_orderId) &&
                string.IsNullOrWhiteSpace(_refundOrderId) &&
                string.IsNullOrWhiteSpace(_refundTransId)
                )
            {
                return new FailResultReturn<RefundQueryResult>("四种订单号,最少要填一种");
            }

            query.AddIf(!string.IsNullOrWhiteSpace(_transId), "transaction_id", _transId)
                .AddIf(!string.IsNullOrWhiteSpace(_orderId), "out_trade_no", _orderId)
                .AddIf(!string.IsNullOrWhiteSpace(_refundOrderId), "out_refund_no", _refundOrderId)
                .AddIf(!string.IsNullOrWhiteSpace(_refundTransId), "refund_id", _refundTransId)
                .AddIf(_offset.HasValue, "offset",_offset.Value)
                ;
            
            string url = $"{Config.GatewayHost}/pay/orderquery";

            var result = await PostData(url, query);

            if (CheckIsSuccess(result.ReturnData))
            {
                //支付成功
                if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "SUCCESS")
                {
                    return new SuccessResultReturn<RefundQueryResult>(new RefundQueryResult(result.ReturnData));
                }
                //用户支付中，需要继续查询
                else if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "USERPAYING")
                {
                    return new FailResultReturn<RefundQueryResult>("等待用户支付", 10003);
                }
                else
                {
                    return new FailResultReturn<RefundQueryResult>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
                }

            }
            else
            {
                return new FailResultReturn<RefundQueryResult>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
            }
        }


    }
}
