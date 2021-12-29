using Aop.Api;
using System;
using Kugar.Payment.Alipay.Services;

namespace Kugar.Payment.Alipay
{
    public class Alipay
    { 
        public Alipay(AlipayConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// 面对面支付
        /// </summary>
        /// <returns></returns>
        public F2FService F2F() => new F2FService(this, config: Config);



        public AlipayConfig Config { get; }
        
    }
}
