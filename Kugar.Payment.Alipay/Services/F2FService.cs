using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Alipay.Enums;
using Kugar.Payment.Alipay.PaymentArguments;
using Kugar.Payment.Alipay.Results;
using Kugar.Payment.Common;
using Newtonsoft.Json;

namespace Kugar.Payment.Alipay.Services
{
    /// <summary>
    /// 面对面扫码支付,商户扫顾客
    /// </summary>
    public class F2FService:AliServiceBase
    {
        private string _authCode = "";
        private string _orderNo = "";
        private decimal _amunt = 0m;
        private string _sellerId = String.Empty;
        private string _storeId = String.Empty;
        private string _subject = string.Empty;
        private string _notifyUrl = String.Empty;

        private string _operatorId = String.Empty;
        private string _terminalId = String.Empty;
        private QueryOption[] _queryOptions = null;
        private F2FRequest.GoodDetail[] _goodsDetial = null;


        internal F2FService(Alipay pay, AlipayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 支付授权码25~30开头的长度为16~24位的数字或刷脸付fp开头的35位字符串
        /// </summary>
        public F2FService AuthCode(string authCode)
        {
            _authCode = authCode;
            return this;
        }

        /// <summary>
        /// 商户单号
        /// </summary>
        public F2FService OutTradeNo(string orderNo)
        {
            _orderNo = orderNo;
            return this;
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public F2FService Amount(decimal amount)
        {
            _amunt = amount;
            return this;
        }

        /// <summary>
        /// 订单标题。不可使用特殊字符，如 /，=，& 等(可选)
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public F2FService Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        /// <summary>
        /// 卖家支付宝用户ID (可选)
        /// </summary>
        public F2FService SellerId(string sellerId)
        {
            _sellerId = sellerId;
            return this;
        }

        /// <summary>
        /// 商户门店编号
        /// </summary>
        public F2FService StoreId(string storeId)
        {
            _storeId = storeId;
            return this;
        }

        /// <summary>
        /// 商户操作员编号
        /// </summary>
        public F2FService OperatorId(string operatorId)
        {
            _operatorId = operatorId;
            return this;
        }

        /// <summary>
        /// 商户机具终端编号
        /// </summary>
        public F2FService TerminalId(string terminalId)
        {
            _terminalId = terminalId;
            return this;
        }

        /// <summary>
        /// 返回参数选项。商户通过传递该参数来定制同步需要额外返回的信息字段
        /// </summary>
        public F2FService QueryOptions(QueryOption[] options)
        {
            _queryOptions = options;
            return this;
        }

        /// <summary>
        /// 订单包含的商品列表信息，(可选)
        /// </summary>
        public F2FService GoodDetail(F2FRequest.GoodDetail[] goods)
        {
            _goodsDetial = goods;
            return this;
        }

        /// <summary>
        /// 异步通知接口,如不配置,则使用Config中配置的默认地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public F2FService NotifyUrl(string url)
        {
            _notifyUrl = url;
            return this;
        }

        public async Task<ResultReturn<F2FPaymentResult>> Execute()
        {
            return await Execute(_orderNo, _authCode, _amunt);
        }

        public async Task<ResultReturn<F2FPaymentResult>> Execute(
            string outtradeNo,
            string authCode,
            decimal amount
            )
        {
            var request = new F2FRequest()
            {
                OutTradeNo = outtradeNo,
                AuthCode = authCode,
                Amount = amount,
                Subject=_subject,
                SellerId=_sellerId,
                StoreId=_storeId,
                OperatorId=_operatorId,
                TerminalId=_terminalId,
                QueryOptions=_queryOptions,
                GoodsDetail=_goodsDetial,
                NotifyUrl = _notifyUrl
            };

            var r = request.CheckParameter();

            if (!r)
            {
                return r.Cast<F2FPaymentResult>(null);
            }

            var response=await base.Post(request);

            return response.Cast((F2FPaymentResult)response.ReturnData, null);
            
        }
    }

}
