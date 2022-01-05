using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.DragonPay.Requests
{
    public abstract class DragonRequestBase
    {
        protected DragonRequestBase(DragonPayConfig config)
        {
            Config = config;
        }

        protected DragonPayConfig Config { set; get; }

        public abstract string ToUrl();
    }
}
