using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Common.Helpers
{
    public static class GlobalExtMethod
    {
        public static IServiceProvider RegisterPayment(this IServiceProvider provider)
        {
            Provider = provider;

            return provider;
        }

        public static IServiceProvider Provider { get; private set; }
    }
}
