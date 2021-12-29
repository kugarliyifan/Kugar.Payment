using System;
using System.Collections.Generic;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;
using Kugar.Payment.Common.Helpers;

namespace Kugar.Payment.Wechatpay.Results
{
    public abstract class WechatPayResultBase: ResultBase,ICommonPaymentResult
    {
        protected WechatPayResultBase(IReadOnlyDictionary<string, string> source)
        {
            IsSuccess = source.TryGetValue("return_code") == "SUCCESS" &&
                        source.TryGetValue("result_code") == "SUCCESS";

            Return_Code = source.TryGetValue("return_code");
            Return_Msg = source.TryGetValue("return_msg");
            Result_Code = source.TryGetValue("result_code");
            if (source.TryGetValue("return_code")== "SUCCESS")
            {
                
                Err_Code = source.TryGetValue("err_code");
                Err_Code_Des = source.TryGetValue("err_code_des");
                AppId = source.TryGetValue("appid");

                if (Result_Code== "SUCCESS")
                {
                    TransactionId = source.TryGetValue("transaction_id");
                    OutTradeNo = source.TryGetValue("out_trade_no");
                    Attach = source.TryGetValue("attach");
                    TimeEnd = source.TryGetValue("time_end").ToDateTimeNullable("yyyyMMddHHmmss");
                    
                }
            } 
        }

        /// <summary>
        /// 返回状态码	
        /// </summary>
        public string Return_Code { set; get; }

        /// <summary>
        /// 返回信息	
        /// </summary>
        public string Return_Msg { set; get; }

        /// <summary>
        /// 业务结果	
        /// </summary>
        public string Result_Code { set; get; }

        /// <summary>
        /// 错误代码	
        /// </summary>
        public string Err_Code { set; get; }

        /// <summary>
        /// 错误代码描述	
        /// </summary>
        public string Err_Code_Des { set; get; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string TransactionId { set; get; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符），
        /// </summary>
        public string OutTradeNo { set; get; }

        /// <summary>
        /// 商家数据包，原样返回
        /// </summary>
        public string Attach { set; get; }

        /// <summary>
        /// 支付完成时间	
        /// </summary>
        public DateTime? TimeEnd { set; get;}




        public string AppId { set; get; }
        
        public static implicit operator ResultReturn(WechatPayResultBase d)
        {
            return new ResultReturn(d.IsSuccess,d,message:d.IsSuccess?"":$"code={d.Err_Code},desc={d.Err_Code_Des}");
        }

        
    }
}
