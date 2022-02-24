using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using OneOf;

namespace Kugar.Payment.Common.Helpers
{
    public static  class OneOfExtMethod
    {
        public static string ToStringEx<T1, T2>(this OneOf<T1, T2> src)
        {
            if (src.IsT0)
            {
                return src.AsT0.ToStringEx();
            }
            else
            {
                return src.AsT1.ToStringEx();
            }
        }
    }
}
