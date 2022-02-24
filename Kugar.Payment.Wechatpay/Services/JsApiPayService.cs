using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;
using Kugar.Payment.Wechatpay.Requests;
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
        private JsApiPayRequest _request = null;

        public JsApiPayService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
            _request = new JsApiPayRequest(config);
        }

        /// <summary>
        /// 支付的用户OpenId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JsApiPayService OpenId(string openId)
        {
            _request.OpenId = openId;
            return this;
        }

        /// <summary>
        /// 通知地址,如未配置,则使用初始化配置中的NotifyUrl属性
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsApiPayService NotifyUrl(string url)
        {
            _request.NotifyUrl = url;

            return this;
        }

        /// <summary>
        /// 调用方IP地址
        /// </summary>
        /// <param name="ipv4orv6"></param>
        /// <returns></returns>
        public JsApiPayService SpbillCreateIp(string ipv4orv6)
        {
            _request.SpbillCreateIp  = ipv4orv6;

            return this;
        }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual JsApiPayService Body(string body)
        {
            _request.Body = body;

            return this;
        }

        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual JsApiPayService Amount(decimal amount)
        {
            _request.Amount = amount;
            return this;
        }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符）
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public virtual JsApiPayService OutTradeNo(string orderNo)
        {
            _request.OutTradeNo = orderNo;

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
            _request.LimitTimeStartDt = startDt;
            _request.LimitTimeEndDt = endDt;

            return this;
        }

        /// <summary>
        /// 是否不允许使用信用卡支付
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual JsApiPayService NoCredit(bool enabled)
        {
            _request.NoCredit = enabled;

            return this;
        }

        /// <summary>
        /// 是否使用分账功能
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual JsApiPayService ProfitSharing(bool enabled)
        {
            _request.ProfitSharing = enabled;

            return this;
        }

        /// <summary>
        /// 执行下单,返回支付所需参数
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<JsApiPayArgument>> ExecuteAsync()
        {
            var vr = _request.Validate();

            if (!vr)
            {
                return vr.Cast((JsApiPayArgument)null);
            }

            var data = _request.ToData();

            var ret=await base.Parent.Common().UnifiedOrder(data, 3);

            if (ret.IsSuccess && CheckIsSuccess(ret.ReturnData))
            {
                var args = new JsApiPayArgument()
                {
                    AppId = ret.ReturnData.TryGetValue("appid"),
                    TimeStamp = GenerateTimeStamp(),
                    NonceStr = GenerateNonceStr(),
                    Package = $"prepay_id={ret.ReturnData.TryGetValue("prepay_id")}",
                    SignType = "MD5"
                };
                
                args.PaySign = MakeSign(ToUrl(new Dictionary<string, OneOf<int, string>>()
                {
                    ["appId"]=args.AppId,
                    ["timeStamp"]=args.TimeStamp,
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

            public string TimeStamp { set; get; }

            public string NonceStr { set; get; }

            public string Package { set; get; }

            public string SignType { set; get; }

            public string PaySign { set; get; }

            public JObject ToJson()
            {
                return new JObject()
                {
                    ["appId"] = this.AppId,
                    ["timeStamp"] = this.TimeStamp,
                    ["nonceStr"] = this.NonceStr,
                    ["package"] = this.Package,
                    ["signType"] = this.SignType,
                    ["paySign"] = this.PaySign
                };
            }
            
        }
    }
}
