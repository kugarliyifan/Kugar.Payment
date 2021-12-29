using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Aop.Api.Util;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Core.Network;
using Kugar.Payment.Alipay.Enums;
using Kugar.Payment.Alipay.PaymentArguments;
using Kugar.Payment.Alipay.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.Services
{
    public class AliServiceBase
    {
        private Dictionary<string, object> _otherOptions = new Dictionary<string, object>();


        public AliServiceBase(Alipay pay, AlipayConfig config)
        {
            Parent = pay;
            Config = config;
        }

        protected async Task<ResultReturn<AlipayResultBase>> Post(AlipayRequestBase args)
        {
            var r = args.CheckParameter();
            
            if (!r)
            {
                return r.Cast<AlipayResultBase>(null);
            }

            var tmp = args.GetParameters();

            var bizJson=tmp.Where(x => x.Value != null && x.Value != "" && (x.Value is IEnumerable t && !t.HasData()))
                .Select(x =>
                {
                    var p = new JProperty(x.Key);

                    if (x.Value is string)
                    {
                        p.Value = x.Value.ToStringEx();
                    }
                    else if (x.Value is decimal d)
                    {
                        p.Value = d;
                    }
                    else if (x.Value is int i)
                    {
                        p.Value = i;
                    }

                    if (x.Value is IEnumerable t)
                    {
                        var arr = new JArray();
                        foreach (var item in t)
                        {
                            if (item is IExtentData e)
                            {
                                arr.Add(e.ToJson());
                            }
                            else
                            {
                                arr.Add(x.Value);
                            }
                        }
                    }

                    return p;
                }).ToArrayEx();

            var j = new JObject();

            foreach (var prop in bizJson)
            {
                j.Add(prop);
            }

            var json = new JObject()
            {
                ["app_id"] = Config.AppId,
                ["method"] = args.Method,
                ["format"] = "JSON",
                ["charset"] = Config.Charset,
                ["sign_type"] = Config.SignType == SignType.RSA2 ? "RSA2" : "RSA",
                ["timestamp"] = DateTime.Now.ToStringEx(DateFormat.DateWithTime),
                ["version"] = "1.0",
                ["notify_url"] = string.IsNullOrWhiteSpace(args.NotifyUrl) ? Config.NotifyUrl : args.NotifyUrl,
                ["biz_content"] = j.ToStringEx(Formatting.None)
            };

            if (!string.IsNullOrWhiteSpace(Config.AppAuthToken))
            {
                json.Add("app_auth_token", Config.AppAuthToken);
            }

            var waitForSign = ToSignStr(json);

            var sign = AlipaySignature.RSASignCharSet(waitForSign, Config.PrivateKey, Config.Charset,
                Config.SignType.ToStringEx().ToLower());

            json.Add("sign",sign);

            var result =await WebHelper.Create(Config.GatewayUrl)
                .SetContent(json.ToStringEx())
                .ContentType(WebHelper.ContentTypeEnum.Json)
                .Post_JsonAsync();

            var t = args.GetType().GetGenericArguments()[0];

            var ac = (AlipayResultBase)Activator.CreateInstance(t,result);
            
            return new SuccessResultReturn<AlipayResultBase>(ac);
        }

        protected string ToSignStr(JObject arguments)
        {
            return arguments.Properties().OrderBy(x => x.Name)
                .Select(x => $"{x.Name}={JsonConvert.SerializeObject(x.Value)}")
                .JoinToString('&');
        }

        protected Alipay Parent { get; }

        protected AlipayConfig Config { get; }
        
        
    }
}
