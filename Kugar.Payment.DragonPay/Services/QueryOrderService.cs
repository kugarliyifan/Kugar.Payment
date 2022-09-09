using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Payment.DragonPay.Enums;
using Kugar.Payment.DragonPay.Requests;
using Kugar.Payment.DragonPay.Results;

namespace Kugar.Payment.DragonPay.Services
{
    public class QueryScanQrCodeOrderService:DragonServiceBase
    {
        private QueryScanQrCodeOrderRequest _request = null;

        public QueryScanQrCodeOrderService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new QueryScanQrCodeOrderRequest(config);
        }

        public QueryScanQrCodeOrderService OrderId(string orderId)
        {
            _request.OrderId = orderId;
            return this;
        }

        public QueryScanQrCodeOrderService QrCodeType(QrCodeType type)
        {
            _request.QrCodeType = type;
            return this;
        }

        public QueryScanQrCodeOrderService AuthCode(string authCode)
        {
            _request.AuthCode = authCode;
            return this;
        }

        public QueryScanQrCodeOrderService ReturnOpenId(bool isReturn = true)
        {
            _request.ReturnOpenId = isReturn;
            return this;
        }

        public async Task<ResultReturn<QueryOrderResult>> ExecuteAsync()
        {
            var ret = await base.PostData($"{Config.GatewayHost}/CCBIS/B2CMainPlat_00_BEPAY", _request);

            if (ret.IsSuccess)
            {
                return ret.Cast(new QueryOrderResult(ret.ReturnData), null);
            }
            else
            {
                return ret.Cast((QueryOrderResult)null);
            }

            
        }
    }
}
