using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Payment.DragonPay.Requests;
using Kugar.Payment.DragonPay.Results;

namespace Kugar.Payment.DragonPay.Services
{
    public class ScanQrCodeService : DragonServiceBase
    {
        private ScanQrcodeRequest _request = null;
        private int _pollQueryCount = 0;
        
        public ScanQrCodeService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
            _request = new ScanQrcodeRequest(config);
        }

        public ScanQrCodeService OrderId(string orderId)
        {
            _request.OrderId = orderId;
            return this;
        }

        public ScanQrCodeService Amount(decimal amount)
        {
            _request.Amount = amount;
            return this;
        }

        public ScanQrCodeService ReturnOpenId(bool isReturn)
        {
            _request.ReturnOpenId = isReturn;
            return this;
        }

        public ScanQrCodeService Body(string body)
        {
            _request.Body = body;
            return this;
        }

        public ScanQrCodeService AuthCode(string autoCode)
        {
            _request.AuthCode = autoCode;
            return this;
        }

        /// <summary>
        /// 自动轮询次数,默认为0,不轮询
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ScanQrCodeService PollQueryCount(int count = 5)
        {
            _pollQueryCount = count;
            return this;
        }

        public async Task<ResultReturn<ScanQrCodeResult>> ExecuteAsync()
        {
            var ret= await base.PostData($"{Config.GatewayHost}/CCBIS/B2CMainPlat_00_BEPAY", _request,1);

            if (ret.IsSuccess)
            {
                var result = new ScanQrCodeResult(ret.ReturnData);

                if (result.IsNeedForCheck && _pollQueryCount>0)
                {
                    if (_pollQueryCount>0)
                    {
                        var tmp = _pollQueryCount;

                        for (int i = 0; i < tmp; i++)
                        {
                            await Task.Delay(result.WaitTime);

                            var tmpRet = await Parent.QueryScanQrCodeOrder().AuthCode(_request.AuthCode)
                                .OrderId(_request.OrderId)
                                .ExecuteAsync();

                            if (tmpRet.IsSuccess && tmpRet.ReturnData.IsSuccess && !tmpRet.ReturnData.IsNeedForCheck)
                            {
                                result.TransactionId = tmpRet.ReturnData.TransactionId;
                                result.OutTradeNo = tmpRet.ReturnData.OutTradeNo;
                                result.IsSuccess = tmpRet.ReturnData.IsSuccess;
                                result.IsNeedForCheck = false;
                                result.Code = tmpRet.ReturnData.Code;
                                result.Message = tmpRet.ReturnData.Message;
                                return new SuccessResultReturn<ScanQrCodeResult>(result);
                            }
                        }
                    }
                    else
                    {
                        ret.Cast(result);
                    }

                }

                return ret.Cast(result, null);
            }
            else
            {
                await Parent.CancelOrder().OrderId(_request.OrderId)
                    .QrCodeType(_request.QrCodeType)
                    .ExecuteAsync();

                return ret.Cast((ScanQrCodeResult)null);
            }
        }
    }
}
