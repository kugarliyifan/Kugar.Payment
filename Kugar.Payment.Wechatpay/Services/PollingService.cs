using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.Collections;
using Kugar.Core.Services;
using Kugar.Payment.Common;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Helpers;

namespace Kugar.Payment.Wechatpay.Services
{
    public class PollingService: WechatpayServiceBase
    {
        public void PollingPayOrder( string transactionId, string outTradeNo,int initQueryTime=0)
        {
            if (string.IsNullOrWhiteSpace(transactionId) && string.IsNullOrWhiteSpace(outTradeNo))
            {
                throw new ArgumentNullException("outTradeNo和transactionId必填一个");
            }

            //if (string.IsNullOrWhiteSpace(appId))
            //{
            //    throw new ArgumentOutOfRangeException(nameof(appId));
            //}

            var coll = getPollingCollection();

            coll.Insert(new PaymentPollingItem(Config.AppId, transactionId, outTradeNo, initQueryTime));
        }

        public void PollingRefundOrder( string refundId, string outRefundNo, int initQueryTime = 0)
        {
            if (string.IsNullOrWhiteSpace(refundId) && string.IsNullOrWhiteSpace(outRefundNo))
            {
                throw new ArgumentNullException("outRefundNo和refundId必填一个");
            }

            var coll = getPollingCollection();

            coll.Insert(new RefundPollingItem(Config.AppId, refundId, outRefundNo, initQueryTime));
        }

        public PollingService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        private PollingCollection getPollingCollection()
        {
            var coll = (PollingCollection)GlobalProvider.Provider.GetService(typeof(PollingCollection));

            return coll;
        }
    }
}
