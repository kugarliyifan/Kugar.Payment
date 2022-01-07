using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;

namespace Kugar.Payment.Common
{
    public interface IResultNotifyHandler
    {
        //Task<ResultReturn> OnNotify(ICommonPaymentResult result);

        Task<ResultReturn> OnPaymentNotifyAsync(ICommonPaymentResult result,string appId);

        Task<ResultReturn> OnRefundNotifyAsync(ICommonRefundResult result,string appId);
    }
}
