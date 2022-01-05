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

        public QueryOrderService QueryOrder() => new QueryOrderService(this, _config);

        public ScanQrCodeService ScanQrCode() => new ScanQrCodeService(this, _config);
    }
}
