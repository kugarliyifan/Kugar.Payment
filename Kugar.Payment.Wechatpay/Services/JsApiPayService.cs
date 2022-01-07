using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OneOf;
using static Kugar.Payment.Wechatpay.Services.MicropayService;

namespace Kugar.Payment.Wechatpay.Services
{
    /// <summary>
    /// 公众号/小程序使用的支付服务
    /// </summary>
    public class JsApiPayService: PayTradeServiceBase 
    {
        private string _openid = "";
        private string _notify_url = "";
        private string _ip = "";

        public JsApiPayService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 支付的用户OpenId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JsApiPayService OpenId(string openId)
        {
            _openid = openId;
            return this;
        }

        /// <summary>
        /// 通知地址,如未配置,则使用初始化配置中的NotifyUrl属性
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsApiPayService NotifyUrl(string url)
        {
            _notify_url = url;

            return this;
        }

        /// <summary>
        /// 调用方IP地址
        /// </summary>
        /// <param name="ipv4orv6"></param>
        /// <returns></returns>
        public JsApiPayService SpbillCreateIp(string ipv4orv6)
        {
            _ip = ipv4orv6;

            return this;
        }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual JsApiPayService Body(string body)
        {
            _body = body;

            return this;
        }

        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual JsApiPayService Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符）
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public virtual JsApiPayService OutTradeNo(string orderNo)
        {
            _tradeno = orderNo;

            return this;
        }

        /// <summary>
        /// 限制支付时间
        /// </summary>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        public virtual JsApiPayService LimitTime(DateTime? startDt, DateTime? endDt)
        {
            _time_start = startDt;
            _time_expire = endDt;

            return this;
        }

        /// <summary>
        /// 是否不允许使用信用卡支付
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual JsApiPayService NoCredit(bool enabled)
        {
            _no_credit = enabled;

            return this;
        }

        /// <summary>
        /// 是否使用分账功能
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual JsApiPayService ProfitSharing(bool enabled)
        {
            _profit_sharing = enabled;

            return this;
        }

        /// <summary>
        /// 执行下单,返回支付所需参数
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<JsApiPayArgument>> ExecuteAsync()
        {
            var data = new Dictionary<string, OneOf<int, string>>();

            if (string.IsNullOrWhiteSpace(_openid))
            {
                return new FailResultReturn<JsApiPayArgument>("openId不能为空");
            }

            if (string.IsNullOrWhiteSpace(_body))
            {
                return new FailResultReturn<JsApiPayArgument>("body不能为空");
            }

            if (_amount <= 0)
            {
                return new FailResultReturn<JsApiPayArgument>("amount必须大于0");
            }

            if (string.IsNullOrWhiteSpace(_tradeno))
            {
                return new FailResultReturn<JsApiPayArgument>("out_trade_no不能为空");
            }

            if (string.IsNullOrWhiteSpace(_ip))
            {
                return new FailResultReturn<JsApiPayArgument>("spbill_create_ip不能为空");
            }

            var notifyUrl= string.IsNullOrWhiteSpace(_notify_url) ? Config.PaymentNotifyUrl : _notify_url;

            notifyUrl = notifyUrl.Replace("{appID}", Config.AppId);

            notifyUrl = Parent.BuildNotifyUrl(notifyUrl);

            data.AddOrUpdate("body", _body); //商品描述
            data.AddOrUpdate("total_fee", (int)(_amount * 100)); //总金额
            data.AddOrUpdate("out_trade_no", _tradeno);
            data.AddOrUpdate("trade_type", "JSAPI");
            data.AddOrUpdate("openid", _openid);
            data.AddOrUpdate("spbill_create_ip", _ip);
            data.AddOrUpdate("notify_url", notifyUrl) ;
            

            data.AddIf(!string.IsNullOrWhiteSpace(_attach), "attach", _attach);
            data.AddIf(!string.IsNullOrWhiteSpace(_fee_type), "fee_type", _fee_type)
                //.AddIf(!string.IsNullOrWhiteSpace(_ip), "spbill_create_ip", _ip)
                .AddIf(_time_start.HasValue, "time_start", _time_start.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_time_expire.HasValue, "time_expire", _time_expire.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_no_credit, "limit_pay", "no_credit")
                .AddIf(_profit_sharing, "profit_sharing", "Y");

            var ret=await base.Parent.Common().UnifiedOrder(data, 3);

            if (ret.IsSuccess && CheckIsSuccess(ret.ReturnData))
            {
                var args = new JsApiPayArgument()
                {
                    AppId = ret.ReturnData.TryGetValue("appid"),
                    Timestamp = GenerateTimeStamp(),
                    NonceStr = GenerateNonceStr(),
                    Package = $"prepay_id={ret.ReturnData.TryGetValue("prepay_id")}",
                    SignType = "MD5"
                };
                
                args.PaySign = MakeSign(ToUrl(new Dictionary<string, OneOf<int, string>>()
                {
                    ["appId"]=args.AppId,
                    ["timeStamp"]=args.Timestamp,
                    ["nonceStr"]=args.NonceStr,
                    ["package"]=args.Package,
                    ["signType"]=args.SignType
                }).ReturnData);

                return new SuccessResultReturn<JsApiPayArgument>(args);
            }
            else
            {
                return ret.Cast<JsApiPayArgument>(null);
            }


        }

        /// <summary>
        /// 小程序/公众号页面支付所需的参数,可直接用ToJson生成JObject对象返回
        /// </summary>
        public class JsApiPayArgument
        {
            public string AppId { set; get; }

            public string Timestamp { set; get; }

            public string NonceStr { set; get; }

            public string Package { set; get; }

            public string SignType { set; get; }

            public string PaySign { set; get; }

            public JObject ToJson()
            {
                return new JObject()
                {
                    ["appId"] = this.AppId,
                    ["timeStamp"] = this.Timestamp,
                    ["nonceStr"] = this.NonceStr,
                    ["package"] = this.Package,
                    ["signType"] = this.SignType,
                    ["paySign"] = this.PaySign
                };
            }
            
        }
    }
}
