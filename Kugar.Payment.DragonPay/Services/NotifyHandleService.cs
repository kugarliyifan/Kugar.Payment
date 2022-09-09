using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CCB_B2CPay_Util;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Payment.Common.Collections;
using Kugar.Payment.DragonPay;
using Kugar.Payment.DragonPay.Results;
using Kugar.Payment.DragonPay.Services;
using Newtonsoft.Json.Linq;
using ResponseData=System.String;


namespace Kugar.Payment.DragonPay.Services
{
    /// <summary>
    /// 通知处理
    /// </summary>
    public class NotifyHandlerService: DragonServiceBase
    {
        public NotifyHandlerService(DragonPay pay, DragonPayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        /// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public ResultReturn<NotifyPaymentResult> DecodePaymentNotifyData(string postData)
        {
            LoggerManager.Default.Debug("龙支付回调通知:" + postData);

            var result = FromPostData(postData);

            if (result==null)
            {
                return new FailResultReturn<NotifyPaymentResult>("数据无效,或校验错误");
            }

            if (result.GetString("SUCCESS","N")=="N")
            {
                return new FailResultReturn<NotifyPaymentResult>("交易失败:" + result.GetString("ERRMSG",""));
            }

            if (!new CCBPayUtil().verifyNotifySign(getUrlForSign(result), result.GetString("SIGN"), Config.PubKey))
            {
                return new FailResultReturn<NotifyPaymentResult>("签名验证失败");
            }

            var data = new NotifyPaymentResult(result);

            data.RawResult = postData;

            if (!data.IsSuccess)
            {
                return new SuccessResultReturn<NotifyPaymentResult>(data);
            }
             

            if (string.IsNullOrWhiteSpace(data.TransactionId))
            {
                return new FailResultReturn<NotifyPaymentResult>(BuildFaildResponse("交易单号不存在"));
            }

            return new SuccessResultReturn<NotifyPaymentResult>(data);
        
        }

        /// <summary>
        /// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        /// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<NotifyPaymentResult>> DecodePaymentNotifyData(Stream data)
        {
            var xml = Encoding.UTF8.GetString(await data.ReadAllBytesAsync());

            return DecodePaymentNotifyData(xml);
        }

        ///// <summary>
        ///// 解码退款通知
        ///// </summary>
        ///// <param name="xml"></param>
        ///// <returns></returns>
        //public async Task<ResultReturn<RefundNotifyResult>> DecodeRefundNotifyData(string xml)
        //{
        //    var xmlDoc = new XmlDocument();

        //    try
        //    {
        //        xmlDoc.LoadXml(xml);

        //        if (xmlDoc.GetFirstElementsByTagName("return_code")?.InnerText != "SUCCESS")
        //        {
        //            return new FailResultReturn<RefundNotifyResult>("解码前数据错误");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return new FailResultReturn<RefundNotifyResult>("解码前数据错误");
        //    }

        //    var base64Str = xmlDoc.GetFirstElementsByTagName("req_info")?.InnerText;

        //    if (string.IsNullOrWhiteSpace(base64Str))
        //    {
        //        return new FailResultReturn<RefundNotifyResult>("req_info数据为空");
        //    }

        //    var md5Paykey = Config..MD5_32(true).ToLower();

        //    var data = AESDecrypt(base64Str, md5Paykey);

        //    var d = FromXml(data);

        //    var result = new RefundNotifyResult(d.ReturnData);

        //    result.RawResult = data;

        //    return new SuccessResultReturn<RefundNotifyResult>(result);
        //}

        ///// <summary>
        ///// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        ///// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        ///// </summary>
        ///// <returns></returns>
        //public async Task<ResultReturn<RefundNotifyResult>> DecodeRefundNotifyData(Stream data)
        //{
        //    var xml = Encoding.UTF8.GetString(await data.ReadAllBytesAsync());

        //    return await DecodeRefundNotifyData(xml);
        //}

        /// <summary>
        /// 输出成功信息
        /// </summary>
        /// <returns></returns>
        public string BuildSuccessResponse()
        {
            var xml = @"<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";

            return xml;
        }

        /// <summary>
        /// 构建输出失败信息
        /// </summary>
        /// <param name="reson"></param>
        /// <returns></returns>
        public string BuildFaildResponse(string reson)
        {
            var xml = @$"<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[{reson}]]></return_msg></xml>";

            return xml;
        }
        

        public static JObject FromPostData(string postData)
        {
            var pairs = postData.Split('&');

            var json = new JObject();

            foreach (var pair in pairs)
            {
                var t = pair.Split('=');
                var key = t[0];
                var value = t.Length == 2 ? t[1] : string.Empty;

                json.Add(key,value);
            }

            return json;
        }

        private string getUrlForSign(JObject json)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("POSID={0}&",json.GetString("POSID"))
                .AppendFormat("BRANCHID={0}&",json.GetString("BRANCHID"))
                .AppendFormat("ORDERID={0}&", json.GetString("ORDERID"))
                .AppendFormat("PAYMENT={0}&", json.GetString("PAYMENT"))
                .AppendFormat("CURCODE={0}&", json.GetString("CURCODE"))
                .AppendFormat("REMARK1={0}&", json.GetString("REMARK1"))
                .AppendFormat("REMARK2={0}&", json.GetString("REMARK2"))
                .AppendFormat("ACC_TYPE={0}&", json.GetString("ACC_TYPE"))
                .AppendFormatIf(json.ContainsKey("TYPE"), "TYPE={0}&", json.GetString("TYPE"))
                .AppendFormatIf(json.ContainsKey("REFERER"), "REFERER={0}&", json.GetString("REFERER"))
                .AppendFormatIf(json.ContainsKey("CLIENTIP"), "CLIENTIP={0}&", json.GetString("CLIENTIP"))
                .AppendFormatIf(json.ContainsKey("ACCDATE"), "ACCDATE={0}&", json.GetString("ACCDATE"))
                .AppendFormatIf(json.ContainsKey("INSTALLNUM"), "INSTALLNUM={0}&", json.GetString("INSTALLNUM"))
                .AppendFormatIf(json.ContainsKey("ERRMSG"), "ERRMSG={0}&", json.GetString("ERRMSG"))
                .AppendFormatIf(json.ContainsKey("USRMSG"), "USRMSG={0}&", json.GetString("USRMSG"))
                .AppendFormatIf(json.ContainsKey("USRINFO"), "USRINFO={0}&", json.GetString("USRINFO"))
                .AppendFormatIf(json.ContainsKey("DISCOUNT"),"DISCOUNT={0}&", json.GetString("DISCOUNT"))
                .AppendFormatIf(json.ContainsKey("ZHJF"),"ZHJF={0}&", json.GetString("ZHJF"))
                .AppendFormatIf(json.ContainsKey("OPENID"),"OPENID={0}&", json.GetString("OPENID"))
                .AppendFormatIf(json.ContainsKey("SUB_OPENID"),"SUB_OPENID={0}&", json.GetString("SUB_OPENID"))
                .AppendFormatIf(json.ContainsKey("PAYMENT_DETAILS"),"PAYMENT_DETAILS={0}&", json.GetString("PAYMENT_DETAILS"))
                ;

            return sb.ToString();
        }
    }

    public static class StringBuilderExt
    {
        public static StringBuilder AppendFormatIf(this StringBuilder sb, bool check, string format, params object[] values)
        {
            if (!check)
            {
                return sb;
            }

            sb.AppendFormat(format, args: values);

            return sb;
        }
    }
}
