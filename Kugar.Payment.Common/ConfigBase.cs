using Microsoft.Extensions.DependencyInjection;
using OneOf;
using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Microsoft.AspNetCore.Http;

namespace Kugar.Payment.Common
{
    public abstract class ConfigBase
    {
        protected ConfigBase()
        {
            Host = (GetHostHandler)hostFactory;
        }

        /// <summary>
        /// 当前站点的域名,可以是固定的String,也可以传入GetHostHandler类型的委托进行运行时处理,返回的域名,必须带http或者https头
        /// </summary>
        public virtual OneOf<string, GetHostHandler> Host { set; get; }

        /// <summary>
        /// 支付网关域名
        /// </summary>
        public virtual string GatewayHost { set; get; } 

        private string hostFactory(IServiceProvider services)
        {
            var httpContext = services.GetService<IHttpContextAccessor>();

            var includeHttp = true;
            var request = httpContext.HttpContext.Request;

            return
                $"{(includeHttp ? "http" : "")}{((includeHttp && request.IsHttps) ? "s" : "")}{(includeHttp ? "://" : "")}{request.Host.Host}{((request.Host.Port == null || request.Host.Port == 80 || request.Host.Port == 443) ? "" : ":" + request.Host.Port.ToStringEx())}";
        }
    }

    public delegate string GetHostHandler(IServiceProvider services);
}
