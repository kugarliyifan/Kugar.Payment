using System;
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
        /// 查询订单
        /// </summary>
        /// <returns></returns>
        public QueryOrderService QueryOrder() => new QueryOrderService(this, _config);

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
    }
}
