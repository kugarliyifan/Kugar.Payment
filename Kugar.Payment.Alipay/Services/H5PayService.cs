using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Alipay.Services
{
    public class MobileH5PayService:AliServiceBase
    {
        private string _authCode = "";
        private string _orderNo = "";
        private decimal _amunt = 0m;
        private string _sellerId = String.Empty;
        private string _storeId = String.Empty;
        private string _subject = string.Empty;
        private string _notifyUrl = String.Empty;
        private string _buyer_id=String.Empty;
        private string _operatorId = String.Empty;
        private string _terminalId = String.Empty;
        private DateTime? _timeExpire = null;
        private string _body=String.Empty;


        public MobileH5PayService(Alipay pay, AlipayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 商户单号
        /// </summary>
        public MobileH5PayService OutTradeNo(string orderNo)
        {
            _orderNo = orderNo;
            return this;
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        public MobileH5PayService Amount(decimal amount)
        {
            _amunt = amount;
            return this;
        }

        /// <summary>
        /// 订单标题。不可使用特殊字符，如 /，=，& 等(可选)
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public MobileH5PayService Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        /// <summary>
        /// 卖家支付宝用户ID (可选)
        /// </summary>
        public MobileH5PayService SellerId(string sellerId)
        {
            _sellerId = sellerId;
            return this;
        }

        /// <summary>
        /// 买家支付宝用户ID.2088开头的16位纯数字 
        /// </summary>
        /// <param name="buyerId"></param>
        /// <returns></returns>
        public MobileH5PayService BuyerId(string buyerId)
        {
            _buyer_id = buyerId;
            return this;
        }

        /// <summary>
        /// 商户操作员编号
        /// </summary>
        public MobileH5PayService OperatorId(string operatorId)
        {
            _operatorId = operatorId;
            return this;
        }

        /// <summary>
        /// 商户机具终端编号
        /// </summary>
        public MobileH5PayService TerminalId(string terminalId)
        {
            _terminalId = terminalId;
            return this;
        }

        /// <summary>
        /// 订单附加信息。如果请求时传递了该参数，将在异步通知、对账单中原样返回，同时会在商户和用户的pc账单详情中作为交易描述展示
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public MobileH5PayService Body(string body)
        {
            _body = body;

            return this;
        }
    }
}
