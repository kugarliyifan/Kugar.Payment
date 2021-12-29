using System;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;

namespace Kugar.Payment.Common
{
    public abstract class ResultBase
    {

        public virtual bool IsSuccess { set; get; }


        public static implicit operator bool(ResultBase d)
        {
            return d.IsSuccess;
        }
    }
    
}
