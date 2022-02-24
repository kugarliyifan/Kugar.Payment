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

        Task<ResultReturn> OnPaymentNotifyAsync(object sender,ICommonPaymentResult result,string appId);

        Task<ResultReturn> OnRefundNotifyAsync(object sender, ICommonRefundResult result,string appId);
    }
}
