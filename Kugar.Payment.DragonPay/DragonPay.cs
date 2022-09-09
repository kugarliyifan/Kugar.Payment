using System;
using Kugar.Payment.DragonPay.Requests;
using Kugar.Payment.DragonPay.Services;

namespace Kugar.Payment.DragonPay
{
    public class DragonPay
    {
        private DragonPayConfig _config = null;

        public DragonPay(DragonPayConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 查询扫码支付订单
        /// </summary>
        /// <returns></returns>
        public QueryScanQrCodeOrderService QueryScanQrCodeOrder() => new QueryScanQrCodeOrderService(this, _config);

        /// <summary>
        /// 扫码支付
        /// </summary>
        /// <returns></returns>
        public ScanQrCodeService ScanQrCode() => new ScanQrCodeService(this, _config);

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <returns></returns>
        public CancelOrderService CancelOrder() => new CancelOrderService(this, _config);

        /// <summary>
        /// 微信公众号/小程序等环境下调用龙支付
        /// </summary>
        /// <returns></returns>
        public WechatUnifiedService WechatPay() => new WechatUnifiedService(this, _config);

        /// <summary>
        /// 通知处理
        /// </summary>
        /// <returns></returns>
        public NotifyHandlerService NotifyHandler() => new NotifyHandlerService(this, _config);

        /// <summary>
        /// 退款
        /// </summary>
        /// <returns></returns>
        public RefundService Refund() => new RefundService(this, _config);
    }
}
