using Kugar.Payment.Wechatpay.Services;

namespace Kugar.Payment.Wechatpay
{
    public class Wechatpay
    {
        private WechatpayConfig _config = null;

        public Wechatpay(WechatpayConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 刷卡支付,商家扫用户二维码
        /// </summary>
        /// <returns></returns>
        public MicropayService Micropay() => new MicropayService(this, config: _config);

        /// <summary>
        /// 扫码支付,用户扫商家二维码
        /// </summary>
        /// <returns></returns>
        public NativePayService NativePay() => new NativePayService(this, config: _config);

        /// <summary>
        /// 退款操作
        /// </summary>
        /// <returns></returns>
        public RefundService Refund() => new RefundService(this, config: _config);

        /// <summary>
        /// 退款状态查询
        /// </summary>
        /// <returns></returns>
        public RefundQueryService RefundQuery() => new RefundQueryService(this, config: _config);

        /// <summary>
        /// 公众号/小程序支付
        /// </summary>
        /// <returns></returns>
        public JsApiPayService JsApiPay() => new JsApiPayService(this, config: _config);
        
        /// <summary>
        /// 通用操作
        /// </summary>
        /// <returns></returns>
        public CommonService Common() => new CommonService(this, config: _config);

        /// <summary>
        /// 通知处理
        /// </summary>
        /// <returns></returns>
        public NotifyHandlerService NotifyHandler() => new NotifyHandlerService(this, config: _config);
    }
}