using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fasterflect;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Wechatpay.Services;
using OneOf;
using OpenId=System.String;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Results;

namespace Kugar.Payment.Wechatpay.Services
{
    public class CommonService : WechatpayServiceBase
    {
        public CommonService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        public virtual async Task<ResultReturn<string>> QueryByOrderId(String orderId)
        {
            var queryOrderInput = new Dictionary<string, OneOf<int, string>>()
            {
                ["out_trade_no"]= orderId
            };

            string url = $"{Config.GatewayHost}/pay/orderquery";

            var result = await PostData(url, queryOrderInput);

            if (CheckIsSuccess(result.ReturnData))
            {
                //支付成功
                if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "SUCCESS")
                {
                    return new SuccessResultReturn<string>(result.ReturnData.TryGetValue("transaction_id").ToStringEx());
                }
                //用户支付中，需要继续查询
                else if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "USERPAYING")
                {
                    return new FailResultReturn<string>("等待用户支付", 10003);
                }
                else
                {
                    return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
                }

            }
            else
            {
                return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
            }

        }

        public virtual async Task<ResultReturn<string>> QueryByTransactionId(String transaction_id)
        {
            if (string.IsNullOrWhiteSpace(transaction_id))
            {
                return new FailResultReturn<string>("transaction_id为空");
            }

            var queryOrderInput = new Dictionary<string, OneOf<int, string>>()
            {
                ["transaction_id"]= transaction_id
            }; 

            string url = $"{Config.GatewayHost}/pay/orderquery";

            var result = await PostData(url, queryOrderInput);

            if (CheckIsSuccess(result.ReturnData))
            {
                //支付成功
                if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "SUCCESS")
                {
                    return new SuccessResultReturn<string>(result.ReturnData.TryGetValue("transaction_id").ToStringEx());
                }
                //用户支付中，需要继续查询
                else if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "USERPAYING")
                {
                    return new FailResultReturn<string>("等待用户支付", 10003);
                }
                else
                {
                    return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
                }

            }
            else
            {
                return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
            }
        }

        public virtual async Task<ResultReturn<QueryInfoResult>> QueryInfoByOrderId(String orderId)
        {
            var queryOrderInput = new Dictionary<string, OneOf<int, string>>()
            {
                ["out_trade_no"] = orderId
            };

            string url = $"{Config.GatewayHost}/pay/orderquery";

            var result = await PostData(url, queryOrderInput);

            var payResult = new QueryInfoResult(result.ReturnData);

            if (CheckIsSuccess(result.ReturnData))
            {
                //支付成功
                if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "SUCCESS")
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult);
                }
                //用户支付中，需要继续查询
                else if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "USERPAYING")
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult)
                    {
                        Message = "等待用户支付",
                        ReturnCode = 10003
                    };
                }
                else
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult);
                }

            }
            else
            {
                return new FailResultReturn<QueryInfoResult>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}");
            }

        }


        public virtual async Task<ResultReturn<QueryInfoResult>> QueryInfoByTransactionId(String transaction_id)
        {
            if (string.IsNullOrWhiteSpace(transaction_id))
            {
                return new FailResultReturn<QueryInfoResult>("transaction_id为空");
            }

            var queryOrderInput = new Dictionary<string, OneOf<int, string>>()
            {
                ["transaction_id"] = transaction_id
            };

            string url = $"{Config.GatewayHost}/pay/orderquery";

            var result = await PostData(url, queryOrderInput);

            var payResult = new QueryInfoResult(result.ReturnData);

            if (CheckIsSuccess(result.ReturnData))
            {
                //支付成功
                if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "SUCCESS")
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult);
                }
                //用户支付中，需要继续查询
                else if (result.ReturnData.TryGetValue("trade_state").ToStringEx() == "USERPAYING")
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult)
                    {
                        Message = "等待用户支付",
                        ReturnCode = 10003
                    };
                }
                else
                {
                    return new SuccessResultReturn<QueryInfoResult>(payResult);
                }

            }
            else
            {
                return new FailResultReturn<QueryInfoResult>($"{result.ReturnData.TryGetValue("err_code")},{result.ReturnData.TryGetValue("err_code_des")}");  
            }
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public virtual async Task<ResultReturn<IReadOnlyDictionary<string, string>>> UnifiedOrder(
            Dictionary<string, OneOf<int, string>> inputData, int retryCount = 1)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputData.ContainsKey("out_trade_no"))
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>("缺少统一支付接口必填参数out_trade_no");
                //throw new WxPayException("！");
            }
            else if (!inputData.ContainsKey("body"))
            {
                //throw new WxPayException("缺少统一支付接口必填参数body！");

                return new FailResultReturn<IReadOnlyDictionary<string, string>>("缺少统一支付接口必填参数body！");
            }
            else if (!inputData.ContainsKey("total_fee"))
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>("缺少统一支付接口必填参数total_fee！");
                //throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputData.ContainsKey("trade_type"))
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>("缺少统一支付接口必填参数trade_type！");

                //throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputData.TryGetValue("trade_type").ToString() == "JSAPI" && !inputData.ContainsKey("openid"))
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>(
                    "统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
                //throw new WxPayException();
            }
            if (inputData.TryGetValue("trade_type").ToString() == "NATIVE" && !inputData.ContainsKey("product_id"))
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>(
                    "统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
                //throw new WxPayException();
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputData.ContainsKey("notify_url"))
            {
                inputData.AddOrUpdate("notify_url", this.Config.PaymentNotifyUrl);//异步通知url
            }

            return await this.PostData(url, inputData, retryCount);

            //inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            //inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            //inputObj.SetValue("spbill_create_ip", WxPayConfig.GetConfig().GetIp());//终端ip	  	    
            //inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            //inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型

            ////签名
            //inputObj.SetValue("sign", inputObj.MakeSign());
            //string xml = inputObj.ToXml();

            //var start = DateTime.Now;

            //Log.Debug("WxPayApi", "UnfiedOrder request : " + xml);
            //string response = HttpService.Post(xml, url, false, timeOut);
            //Log.Debug("WxPayApi", "UnfiedOrder response : " + response);

            //var end = DateTime.Now;
            //int timeCost = (int)((end - start).TotalMilliseconds);

            //WxPayData result = new WxPayData();
            //result.FromXml(response);

            //ReportCostTime(url, timeCost, result);//测速上报

            //return result;
        }

        /// <summary>
        /// 取消支付单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultReturn> CancelOrder(string orderId)
        {
            string url = $"{Config.GatewayHost}/secapi/pay/reverse";

            if (orderId == null)
            {
                return new FailResultReturn("单号无效");
            }

            var dic = new Dictionary<string, OneOf<int, string>>()
            {
                ["out_trade_no"] = orderId
            };

            var count = 5;

            retry:

            var result = await base.PostData(url, dic, 3);

            if (result.IsSuccess && CheckIsSuccess(result.ReturnData))
            {
                if (result.ReturnData.TryGetValue("transaction_id", out var t))
                {
                    return new SuccessResultReturn<string>(t);
                }
            }



            //如果结果为success且不需要重新调用撤销，则表示撤销成功
            if (result.ReturnData.TryGetValue("result_code").ToString() != "SUCCESS" && result.ReturnData.TryGetValue("recall").ToString() == "N")
            {
                return SuccessResultReturn.Default;
            }
            else if (result.ReturnData.TryGetValue("recall").ToString() == "Y")
            {
                count--;

                if (count <= 0)
                {
                    return new FailResultReturn("超过重试次数");
                }

                goto retry;
            }
            //接口调用失败
            else if (result.ReturnData.TryGetValue("return_code").ToString() != "SUCCESS")
            {
                return new FailResultReturn($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx());
            }
            return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
        }
        
        /// <summary>
        /// 通过付款码查询用户对应OpenId
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns>returnData为用户OpenId</returns>
        public async Task<ResultReturn<OpenId>> AutoCodeToOpenId(string authCode)
        {
            Dictionary<string, OneOf<int, string>> query = new Dictionary<string, OneOf<int, string>>();
            query.AddOrUpdate("auth_code", authCode);

            string url = $"{Config.GatewayHost}/tools/authcodetoopenid";

            var result = await PostData(url, query);

            if (CheckIsSuccess(result.ReturnData))
            {
                return new SuccessResultReturn<string>(result.ReturnData.TryGetValue("openid"));

            }
            else
            {
                return new FailResultReturn<string>($"{result.ReturnData.TryGetValue("err_code").ToString()},{result.ReturnData.TryGetValue("err_code_des")}".ToStringEx(), 0);
            }
        }

        
    }
}