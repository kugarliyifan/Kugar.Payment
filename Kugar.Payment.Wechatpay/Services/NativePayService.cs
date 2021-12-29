using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fasterflect;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Services;
using OneOf;
using QRCodeData=System.String;

namespace Kugar.Payment.Wechatpay.Services
{
    /// <summary>
    /// 生成扫描支付,用户扫商家的二维码模式,调用后,返回支付二维码的内容,用户自行生成二维码图片
    /// </summary>
    public class NativePayService : PayTradeServiceBase 
    {
        private string _productId = "";
        private string _orderId = "";
        private DateTime _startDt = DateTime.Now;
        private DateTime _endDt = DateTime.Now.AddMinutes(10);
        private string _attach = string.Empty;


        public NativePayService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 商品Id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public NativePayService ProductId(string productId)
        {
            _productId = productId;
            return this;
        }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual NativePayService Body(string body)
        {
            _body = body;

            return this;
        }

        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual NativePayService Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }


        /// <summary>
        /// 限制支付时间
        /// </summary>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        public virtual NativePayService LimitTime(DateTime? startDt, DateTime? endDt)
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
        public virtual NativePayService NoCredit(bool enabled)
        {
            _no_credit = enabled;

            return this;
        }

        /// <summary>
        /// 是否使用分账功能
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual NativePayService ProfitSharing(bool enabled)
        {
            _profit_sharing = enabled;

            return this;
        }

        public virtual NativePayService Attach(string attach)
        {
            _attach = attach;
            return this;
        }

        /// <summary>
        /// 生成扫描支付模式一URL,离线生成二维码,不支持有效时间
        /// </summary>
        /// <returns>返回支付二维码的内容</returns>
        public async Task<ResultReturn<QRCodeData>> ExecuteMode1Async()
        {
            if (string.IsNullOrWhiteSpace(_productId))
            {
                return new FailResultReturn<string>("productId不能为空");
            }

            var data = new Dictionary<string, OneOf<int, string>>()
            {
                ["appid"] = Config.AppId,
                ["mch_id"] = Config.MchId,
                ["time_stamp"] = GenerateTimeStamp(),
                ["nonce_str"] = GenerateNonceStr(),
                ["product_id"] = _productId
            };

            var str = ToUrl(data);

            if (!str.IsSuccess)
            {
                return str;
            }

            data.AddOrUpdate("sign", MakeSign(str.ReturnData));//签名

            var ret = ToUrl(data);//转换为URL串
            string url = "weixin://wxpay/bizpayurl?" + ret.ReturnData;

            return new SuccessResultReturn<string>(url);

        }

        /// <summary>
        /// 生成直接支付url，支持有效时间,模式二
        /// </summary>
        /// <returns>返回支付二维码的内容</returns>
        public async Task<ResultReturn<QRCodeData>> ExecuteMode2Async()
        {
            if (string.IsNullOrWhiteSpace(_orderId))
            {
                return new FailResultReturn<string>("orderId不能为空");
            }

            if (_amount<=0)
            {
                return new FailResultReturn<string>("amount必须大于0");
            }

            if (string.IsNullOrWhiteSpace(_productId))
            {
                return new FailResultReturn<string>("productId不能为空");
            }

            var data = new Dictionary<string, OneOf<int, string>>()
            {
                ["body"] = _body,
                ["out_trade_no"] = _orderId,
                ["total_fee"] = (int)(_amount * 100),
                //["time_start"] = _startDt.ToString("yyyyMMddHHmmss"),
                //["time_expire"] = _endDt.ToString("yyyyMMddHHmmss"),
                ["trade_type"] = "NATIVE",
                ["product_id"]=_productId
            };

            data.AddIf(!string.IsNullOrWhiteSpace(_attach), "attach", _attach);
            data.AddIf(!string.IsNullOrWhiteSpace(_fee_type), "fee_type", _fee_type)
                .AddIf(_time_start.HasValue, "time_start", _time_start.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_time_expire.HasValue, "time_expire", _time_expire.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_no_credit, "limit_pay", "no_credit")
                .AddIf(_profit_sharing, "profit_sharing", "Y");

            var result = await Parent.Common().UnifiedOrder(inputData: data);

            if (result.IsSuccess && CheckIsSuccess(result.ReturnData))
            {
                var code = result.ReturnData.TryGetValue("code_url");

                return new SuccessResultReturn<string>(code.ToStringEx());
            }
            else
            {
                return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx());
            }

            //data.SetValue("", "test");//商品描述
            //data.SetValue("attach", "test");//附加数据
            //data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            //data.SetValue("total_fee", 1);//总金额
            //data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            //data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            //data.SetValue("goods_tag", "jjj");//商品标记
            //data.SetValue("trade_type", "NATIVE");//交易类型
            //data.SetValue("product_id", productId);//商品ID

            //WxPayData result = WxPayApi.UnifiedOrder(data);//调用统一下单接口
            //string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

            //Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + url);
            //return url;
        }
    }
}