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
    public class CancelOrderService:DragonServiceBase
    {
        private CancelOrderRequest _request = null;

        public CancelOrderService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new CancelOrderRequest(config);
        }

        public CancelOrderService OrderId(string orderId)
        {
            _request.OrderId = orderId;
            return this;
        }

        public CancelOrderService QrCodeType(QrCodeType type)
        {
            _request.QrCodeType=type;
            return this;
        }

        public async Task<ResultReturn<CancelOrderResult>> ExecuteAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                var ret = await base.PostData($"{Config.GatewayHost}/CCBIS/B2CMainPlat_00_BEPAY", _request);

                if (ret.IsSuccess)
                {
                    var result = new CancelOrderResult(ret.ReturnData);

                    if (!result.IsSuccess && result.ReCall)
                    {
                        await Task.Delay(5000);
                        continue;
                    }

                    return ret.Cast(result, null);
                }
                else
                {
                    return ret.Cast((CancelOrderResult)null);
                }
            }

            return new FailResultReturn<CancelOrderResult>("关闭支付单失败");


        }
    }
}
