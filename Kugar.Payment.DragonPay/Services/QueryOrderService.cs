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
    public class QueryOrderService:DragonServiceBase
    {
        private QueryOrderRequest _request = null;

        public QueryOrderService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new QueryOrderRequest(config);
        }

        public QueryOrderService OrderId(string orderId)
        {
            _request.OrderId = orderId;
            return this;
        }

        public QueryOrderService QrCodeType(QrCodeType type)
        {
            _request.QrCodeType = type;
            return this;
        }

        public QueryOrderService AuthCode(string authCode)
        {
            _request.AuthCode = authCode;
            return this;
        }

        public QueryOrderService ReturnOpenId(bool isReturn = true)
        {
            _request.ReturnOpenId = isReturn;
            return this;
        }

        public async Task<ResultReturn<QueryOrderResult>> ExecuteAsync()
        {
            var ret = await base.PostData("https://ibsbjstar.ccb.com.cn/CCBIS/B2CMainPlat_00_BEPAY", _request);

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
