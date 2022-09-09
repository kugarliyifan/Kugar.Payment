using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Collections;
using Kugar.Payment.DragonPay.Enums;

namespace Kugar.Payment.DragonPay.Requests
{
    public class WechatUnifiedRequest:DragonRequestBase
    {
        public WechatUnifiedRequest(DragonPayConfig config) : base(config)
        {
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { set; get; }

        /// <summary>
        /// 付款金额,小数点后2位
        /// </summary>
        public decimal Payment { set; get; }

        public IPAddress ClientIP { set; get; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string Body { set; get; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public DateTime? Timeout { set; get; }

        /// <summary>
        /// 用户公众号/小程序的OpenId
        /// </summary>
        public string OpenId { set; get; }
        
        public override string ToUrl()
        {
            if (!Check())
            {
                return "";
            }

            var p = new ParamtersCollection();

            p.Set("MERCHANTID", Config.MerchantId)
                .Set("POSID", Config.PosId)
                .Set("BRANCHID", Config.PosId)
                .Set("ORDERID", this.OrderId)
                .Set("PAYMENT", this.Payment.ToString("f2"))
                .Set("CURCODE", "01")
                .Set("TXCODE", "530590")
                .Set("TYPE", "1")
                .Set("GATEWAY", "0")
                .SetIf(ClientIP != null, "CLIENTIP", ClientIP?.ToString())
                .Set("PROINFO", Body, true)
                .SetIf(Timeout.HasValue, "TIMEOUT", Timeout?.ToStringEx("YYYYMMDDHHmmss"))
                .Set("TRADE_TYPE",
                    Config.WechatType.Switch("").Case(WechatType.MP, "JSAPI").Case(WechatType.MiniApp, "MINIPRO")
                        .Result)
                .Set("SUB_APPID", Config.WechatAppId)
                .Set("SUB_OPENID", OpenId)
                .Set("PUB", Config.PubKey.Substring(Config.PubKey.Length - 30,30));
                ;

            var mac = p.GetUrl().MD5_32(true);

            p.Remove("PUB").Set("MAC", mac);

            return p.GetUrl(); 
        }

        public override ResultReturn Check()
        {
            if (Payment<=0)
            {
                return new FailResultReturn("Payment字段必须大于0");
            }

            if (string.IsNullOrWhiteSpace(OrderId) )
            {
                return new FailResultReturn("OrderId字段必填");
            }

            if (OrderId.Length>30)
            {
                return new FailResultReturn("OrderId必须30个字符内");
            }

            if (Timeout.HasValue && Timeout<=DateTime.Now)
            {
                return new FailResultReturn("如需使用Timeout,必须大于当前时间");
            }

            if (string.IsNullOrWhiteSpace(OpenId))
            {
                return new FailResultReturn("OpenId不能为空");
            }
            
            return base.Check();
        }
    }
    
}
