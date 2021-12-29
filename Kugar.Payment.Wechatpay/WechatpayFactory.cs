using System.Collections.Generic;
using Kugar.Core.ExtMethod;

namespace Kugar.Payment.Wechatpay
{
    public static class WechatpayFactory
    {
        private static Dictionary<string, Wechatpay> _cache = new Dictionary<string, Wechatpay>();

        public static void AddConfig(WechatpayConfig config)
        {
            if (_cache.ContainsKey(config.AppId))
            {
                return;
            }
            else
            {
                _cache.Add(config.AppId,new Wechatpay(config));
            }
        }

        public static Wechatpay GetByAppId(string appId)
        {
            return _cache.TryGetValue(appId, null);
        }
    }
}