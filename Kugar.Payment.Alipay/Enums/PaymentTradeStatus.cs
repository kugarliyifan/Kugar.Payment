using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Alipay.Enums
{
    public enum PaymentTradeStatus
    {
        TRADE_CLOSED,

        WAIT_BUYER_PAY,

        TRADE_SUCCESS,

        TRADE_FINISHED
    }
}
