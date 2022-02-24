using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Payment.Common.Enum;

namespace Kugar.Payment.Common
{
    public interface IPayment
    {
        PaymentType Type { get; }


    }
}
