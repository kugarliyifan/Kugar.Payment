using System;
using System.Collections.Generic;
using Kugar.Core.ExtMethod;
using Microsoft.Extensions.DependencyInjection;

namespace Kugar.Payment.Wechatpay
{
    public static class WechatpayFactory
    {
        private static Dictionary<string, Wechatpay> _cache = new Dictionary<string, Wechatpay>();

        public static void AddConfig(WechatpayConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Host))
            {
                throw new ArgumentNullException(nameof(WechatpayConfig.Host), "请填写Host参数为当前站点域名");
            }

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