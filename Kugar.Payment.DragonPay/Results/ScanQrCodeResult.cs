using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.DragonPay.Enums;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    public class ScanQrCodeResult: DragonPaymentResultBase
    {
        public ScanQrCodeResult(JObject json) : base(json)
        {
            //IsSuccess = json.GetString("RESULT") == "Y";

            var result = json.GetString("RESULT");

            if (result=="Q")
            {
                IsNeedForCheck = true;
                //Code = result;
            }
            else
            {
                //Code = result;
            }

            //Msg = json.GetString("ERRMSG");
            
            TransactionId = json.GetString("TRACEID");
            OutTradeNo = json.GetString("ORDERID");
            WaitTime = json.GetInt("WAITTIME");
            QrCodeType = (QrCodeType)json.GetInt("QRCODETYPE");
            Amount = json.GetDecimal("AMOUNT");
        }

        /// <summary>
        /// 是否支付成功,false时,检查IsNeedForCheck或者Code
        /// </summary>
        public override bool IsSuccess { set; get; }

        /// <summary>
        /// 二维码类型
        /// </summary>
        public QrCodeType QrCodeType { set; get; }

        /// <summary>
        /// 建议等待确认时间
        /// </summary>
        public int WaitTime { set; get; }

        /// <summary>
        /// 是否需要等待确认支付,比如扫码后等待用户确认等
        /// </summary>
        public bool IsNeedForCheck { set; get; }

        public decimal Amount { set; get; }

    }
}
