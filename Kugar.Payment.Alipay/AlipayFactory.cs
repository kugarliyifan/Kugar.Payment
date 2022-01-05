using System.Collections.Generic;
using Kugar.Core.ExtMethod;

namespace Kugar.Payment.Alipay
{
    public class AlipayFactory
    {
        private static Dictionary<string, Alipay> _cache = new Dictionary<string, Alipay>();

        public static void AddConfig(AlipayConfig config)
        {
            if (_cache.ContainsKey(config.AppId))
            {
                return;
            }
            else
            {
                _cache.Add(config.AppId, new Alipay(config));
            }
        }

        public static Alipay GetByAppId(string appId)
        {
            return _cache.TryGetValue(appId, null);
        }
    }
}
