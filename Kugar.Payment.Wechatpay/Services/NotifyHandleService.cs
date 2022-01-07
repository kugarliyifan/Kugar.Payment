using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Wechatpay.Results;
using ResponseData=System.String;


namespace Kugar.Payment.Wechatpay.Services
{
    /// <summary>
    /// 通知处理
    /// </summary>
    public class NotifyHandlerService: WechatpayServiceBase
    {
        public NotifyHandlerService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }

        /// <summary>
        /// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        /// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public async Task<ResultReturn<NotifyPaymentResult>> DecodePaymentNotifyData(string xml)
        {
            var result = FromXml(xml);

            if (!result.IsSuccess)
            {
                return new FailResultReturn<NotifyPaymentResult>("数据无效,或校验错误");
            }

            var data = new NotifyPaymentResult(result.ReturnData);

            if (!data.IsSuccess)
            {
                return new SuccessResultReturn<NotifyPaymentResult>(data);
            }
             

            if (string.IsNullOrWhiteSpace(data.TransactionId))
            {
                return new FailResultReturn<NotifyPaymentResult>(BuildFaildResponse("交易单号不存在"));
            }

            if (!await Parent.Common().QueryByTransactionId(data.TransactionId))
            {
                return new FailResultReturn<NotifyPaymentResult>(BuildFaildResponse("订单查询失败") );
            }
            //查询订单成功
            else
            {
                return new SuccessResultReturn<NotifyPaymentResult>(data);
            }
        }

        /// <summary>
        /// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        /// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<NotifyPaymentResult>> DecodePaymentNotifyData(Stream data)
        {
            var xml = Encoding.UTF8.GetString(await data.ReadAllBytesAsync());

            return await DecodePaymentNotifyData(xml);
        }

        /// <summary>
        /// 解码退款通知
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public async Task<ResultReturn<RefundNotifyResult>> DecodeRefundNotifyData(string xml)
        {
            var xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xml);

                if (xmlDoc.GetFirstElementsByTagName("return_code")?.InnerText != "SUCCESS")
                {
                    return new FailResultReturn<RefundNotifyResult>("解码前数据错误");
                }
            }
            catch (Exception e)
            {
                return new FailResultReturn<RefundNotifyResult>("解码前数据错误");
            }

            var base64Str = xmlDoc.GetFirstElementsByTagName("req_info")?.InnerText;

            if (string.IsNullOrWhiteSpace(base64Str))
            {
                return new FailResultReturn<RefundNotifyResult>("req_info数据为空");
            }

            var md5Paykey = Config.PayKey.MD5_32(true).ToLower();

            var data = AESDecrypt(base64Str, md5Paykey);

            var d = FromXml(data);

            var result = new RefundNotifyResult(d.ReturnData);

            return new SuccessResultReturn<RefundNotifyResult>(result);
        }

        /// <summary>
        /// 解析通知的数据,需要判断 IsSuccess==true && returnData.IsSuccess==true 才保证交易成功 <br />
        /// ResultReturn.IsSuccess=false 为表示解析错误,如校验码错误之类的
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<RefundNotifyResult>> DecodeRefundNotifyData(Stream data)
        {
            var xml = Encoding.UTF8.GetString(await data.ReadAllBytesAsync());

            return await DecodeRefundNotifyData(xml);
        }

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

        private string AESDecrypt(String base64Str, String Key)
        {
            //base64
            var encryptedBytes = Convert.FromBase64String(base64Str);
            //hex
            //Byte[] encryptedBytes = hexStringToByteArray(Data);
            var bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

            using var mStream = new MemoryStream(encryptedBytes);
            //mStream.Write( encryptedBytes, 0, encryptedBytes.Length );  
            //mStream.Seek( 0, SeekOrigin.Begin );  
            using var aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            //aes.IV = _iV;  
            using var cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[encryptedBytes.Length + 32];
                int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length + 32);
                byte[] ret = new byte[len];
                Array.Copy(tmp, 0, ret, 0, len);
                return Encoding.UTF8.GetString(ret);
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        } 
    }
}
