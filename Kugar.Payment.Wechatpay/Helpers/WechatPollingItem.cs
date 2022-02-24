using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Core.Collections;
using Kugar.Payment.Common;

namespace Kugar.Payment.Wechatpay.Helpers
{
    public abstract class WechatPollingItemBase: PollingItemBase
    {
        public WechatPollingItemBase(string appId,int initQueryTime=0):base(initQueryTime)
        {
            this.AppId = appId;
        }

        public string AppId {  get; }

        protected Wechatpay getWechatpay(IServiceProvider provider)
        {
            Wechatpay pay = (Wechatpay)provider.GetService(typeof(Wechatpay));

            if (pay == null)
            {
                pay = WechatpayFactory.GetByAppId(appId: AppId);
            }

            if (pay == null)
            {
                throw new ArgumentException("AppId不存在配置");
            }

            return pay;
        }
    }

    public class PaymentPollingItem: WechatPollingItemBase
    {

        internal PaymentPollingItem(string appId, string transactionId, string outTradeNo, int initQueryTime = 0) :base(appId, initQueryTime)
        {
            if (string.IsNullOrWhiteSpace(transactionId) && string.IsNullOrWhiteSpace(outTradeNo))
            {
                throw new ArgumentNullException("outTradeNo和transactionId必填一个");
            }

            this.TransactionId = transactionId;
            this.OutTradeNo= outTradeNo; 
        }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string TransactionId { get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符），
        /// </summary>
        public string OutTradeNo { get; }

        public override async Task<ResultReturn> Query(IServiceProvider provider)
        {
            var pay = getWechatpay(provider);

            ICommonPaymentResult payment = null;

            if (!string.IsNullOrWhiteSpace(TransactionId))
            {
                var ret = await pay.Common().QueryInfoByTransactionId(TransactionId);

                if (ret)
                {
                    payment = ret.ReturnData;
                }
                else
                {
                    return ret;
                }
            }
            else
            {
                var ret = await pay.Common().QueryInfoByOrderId(OutTradeNo);

                if (ret)
                {
                    payment = ret.ReturnData;
                }
                else
                {
                    return ret;
                }
            }

            if (payment.IsSuccess)
            {
                var hander = (IResultNotifyHandler)provider.GetService(typeof(IResultNotifyHandler));

                if (hander!=null)
                {
                    await hander.OnPaymentNotifyAsync(pay, payment, AppId);
                }

                return SuccessResultReturn.Default;
            }
            else
            {
                return new FailResultReturn("未付款完成");
            }
            
        }

        public override async Task ExpireQuery(IServiceProvider provider)
        {
            var pay = getWechatpay(provider);

            if (!string.IsNullOrWhiteSpace(TransactionId))
            {
                await pay.Common().CancelOrderByTransactionId(OutTradeNo);
            }
            else
            {
                await pay.Common().CancelOrderByOrderId(OutTradeNo);
            }
                

        }
        

        public override int MaxQueryTime => 5000;

    }

    public class RefundPollingItem : WechatPollingItemBase
    {
        internal RefundPollingItem(string appId, string refundId, string outRefundNo, int initQueryTime = 0) : base(appId, initQueryTime)
        {
            if (string.IsNullOrWhiteSpace(refundId) && string.IsNullOrWhiteSpace(outRefundNo))
            {
                throw new ArgumentNullException("outTradeNo和transactionId必填一个");
            }

            this.RefundId = refundId;
            this.OutRefundNo = outRefundNo;
        }

        /// <summary>
        /// 微信退款订单号
        /// </summary>
        public string RefundId { get; }

        /// <summary>
        /// 商户系统内部退款单号，要求32个字符内（最少6个字符），
        /// </summary>
        public string OutRefundNo { get; }

        public override async Task<ResultReturn> Query(IServiceProvider provider)
        {
            var pay = getWechatpay(provider);

            ICommonRefundResult refundResult = null;
            var ret = await pay.RefundQuery().RefundId(RefundId).OutRefundNo(OutRefundNo).ExecuteAsync();

            if (ret)
            {
                refundResult = ret.ReturnData;
            }
            else
            {
                return ret;
            }
      
            if (refundResult.IsSuccess)
            {
                var hander = (IResultNotifyHandler)provider.GetService(typeof(IResultNotifyHandler));

                if (hander != null)
                {
                    await hander.OnRefundNotifyAsync(pay, refundResult, AppId);
                }

                return SuccessResultReturn.Default;
            }
            else
            {
                return new FailResultReturn("未付款完成");
            }
        }

        public override Task ExpireQuery(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }

        public override int MaxQueryTime => 5000;
    }
}
