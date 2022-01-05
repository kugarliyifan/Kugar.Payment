using System.Collections.Generic;
using System.Linq;
using Kugar.Core.ExtMethod;

namespace Kugar.Payment.DragonPay
{
    public static class DragonPayFactory
    {
        private static Dictionary<string, DragonPay> _cache = new Dictionary<string, DragonPay>();

        public static void AddConfig(DragonPayConfig config)
        {
            if (_cache.ContainsKey(config.MerchantId))
            {
                return;
            }
            else
            {
                _cache.Add(config.MerchantId, new DragonPay(config));
            }
        }

        public static DragonPay GetByAppId(string appId)
        {
            return _cache.TryGetValue(appId, null);
        }

        public static DragonPay GetDefault()
        {
            return _cache.FirstOrDefault().Value;
        }
    }
}
