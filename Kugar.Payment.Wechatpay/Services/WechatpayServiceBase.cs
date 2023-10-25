using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Core.Network;
using Kugar.Payment.Wechatpay.Enums;
using Kugar.Payment.Wechatpay.Requests;
using Newtonsoft.Json;
using OneOf;
using static Kugar.Payment.Wechatpay.Services.MicropayService;

namespace Kugar.Payment.Wechatpay.Services
{
    public abstract class WechatpayServiceBase
    {
        protected WechatpayServiceBase(Wechatpay pay, WechatpayConfig config)
        {
            this.Config = config;
            this.Parent = pay;
        }

        protected async Task<ResultReturn<IReadOnlyDictionary<string, string>>> PostData(string url, WechatRequestBase request, int retryCount = 5, bool useClientCert = false)
        {
            var vr = request.Validate();

            if (!vr)
            {
                return vr.Cast((IReadOnlyDictionary<string, string>)null);
            }

            var inputData = request.ToData();

            return await PostData(url, inputData, retryCount, useClientCert);
        }

        protected async Task<ResultReturn<IReadOnlyDictionary<string, string>>> PostData(string url, Dictionary<string, OneOf<int, string>> inputData, int retryCount = 5, bool useClientCert = false,bool needTimestamp=true)
        {
            inputData.AddOrUpdate("appid", Config.AppId);//公众账号ID
            inputData.AddOrUpdate("mch_id", Config.MchId);//商户号
            inputData.AddOrUpdate("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputData.AddOrUpdate("sign_type", Config.SignType == SignType.MD5 ? "MD5" : "HMAC-SHA256");//签名类型

            if (needTimestamp)
            {
                inputData.AddOrUpdate("time_stamp", GenerateTimeStamp());//时间戳    
            }
            
            var ret = ToUrl(inputData);

            if (!ret.IsSuccess)
            {
                return ret.Cast<ResultReturn<IReadOnlyDictionary<string, string>>>();
            }

            inputData.AddOrUpdate("sign", MakeSign(ret.ReturnData));//签名

            var ret1 = ToXml(inputData);

            string xml = "";

            if (!ret1)
            {
                return ret1.Cast<ResultReturn<IReadOnlyDictionary<string, string>>>();
            }

            xml = ret1.ReturnData;

            try
            {
                var http = WebHelper.Create(url)
                    .SetContent(xml)
                    .RetryCount(retryCount);

                if (useClientCert)
                {
                    http.AddCertificate(new X509Certificate2(Config.CertData, Config.CertPassword));
                }

                var response = await http.Post_StringAsync();

                var dic = FromXml(response);

                return dic;
            }
            catch (Exception e)
            {
                LoggerManager.Default.Debug($"微信支付提交错误:{JsonConvert.SerializeObject(e)}");
                return new FailResultReturn<IReadOnlyDictionary<string, string>>(e);
            }
        }


        protected string MakeSign(string inputData)
        {
            inputData += ("&key=" + Config.PayKey);

            if (Config.SignType == SignType.MD5)
            {
                //return inputData.MD5_32(true);

                using var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(inputData));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }
            else if (Config.SignType == SignType.SHA256)
            {
                return CalcHMACSHA256Hash(inputData, Config.PayKey);
            }
            else
            {
                throw new Exception("sign_type 不合法");
            }
        }

        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        protected ResultReturn<IReadOnlyDictionary<string, string>> FromXml(string xml,bool checkSign=true)
        {
            if (string.IsNullOrEmpty(xml))
            {
                //Log.Error(this.GetType().ToString(), "将空的xml串转换为WxPayData不合法!");
                //throw new WxPayException("将空的xml串转换为WxPayData不合法!");

                return new FailResultReturn<IReadOnlyDictionary<string, string>>("将空的xml串转换为WxPayData不合法!");
            }


            XmlDocument xmlDoc = new XmlDocument()
            {
                XmlResolver = null
            };

            try
            {
                xmlDoc.LoadXml(xml);
                XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
                XmlNodeList nodes = xmlNode.ChildNodes;

                var dic = new Dictionary<string, string>();

                foreach (XmlNode xn in nodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    dic.Add(xe.Name, xe.InnerText);//获取xml的键值对到WxPayData内部的数据中
                }

                try
                {
                    if (dic["return_code"] != "SUCCESS")
                    {
                        return new ResultReturn<IReadOnlyDictionary<string, string>>(false, dic, message: "请求失败");
                    }

                    var inputData = ToUrl(dic);

                    if (!inputData.IsSuccess)
                    {
                        return new FailResultReturn<IReadOnlyDictionary<string, string>>(inputData.Message);
                    }

                    if (checkSign)
                    {
                        var sign = MakeSign(inputData.ReturnData);

                        if (sign == dic["sign"])
                        {
                            return new SuccessResultReturn<IReadOnlyDictionary<string, string>>(dic);
                        }
                        else
                        {
                            return new FailResultReturn<IReadOnlyDictionary<string, string>>("签名错误");
                        }
                    }
                    else
                    {
                        return new SuccessResultReturn<IReadOnlyDictionary<string, string>>(dic);

                    }
                    
                }
                catch (Exception ex)
                {
                    return new FailResultReturn<IReadOnlyDictionary<string, string>>(ex);
                }
            }
            catch (Exception e)
            {
                return new FailResultReturn<IReadOnlyDictionary<string, string>>(e);
            }


        }

        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        protected ResultReturn<string> ToUrl(IReadOnlyDictionary<string, OneOf<int, string>> inputData)
        {
            string buff = "";

            var keys = inputData.Keys.OrderBy(x => x).ToArrayEx();

            foreach (var key in keys)
            {
                var value = inputData[key];

                if (value.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");

                    return new FailResultReturn<string>($"{key}值为null的字段!");
                }

                var v =value.Match(
                    i=>i.ToStringEx(),
                    j=>j.ToStringEx()
                    );

                if (key != "sign" && v != "")
                {
                    buff += key + "=" + v + "&";
                }
            }
            buff = buff.Trim('&');
            return new SuccessResultReturn<string>(buff);
        }

        protected ResultReturn<string> ToUrl(IReadOnlyDictionary<string, string> inputData)
        {
            string buff = "";

            var keys = inputData.Keys.OrderBy(x => x).ToArrayEx();

            foreach (var key in keys)
            {
                var value = inputData[key];

                if (value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");

                    return new FailResultReturn<string>($"{key}值为null的字段!");
                }

                var v = value.ToStringEx();

                if (key != "sign" && value.ToStringEx() != "")
                {
                    buff += key + "=" + v + "&";
                }
            }
            buff = buff.Trim('&');
            return new SuccessResultReturn<string>(buff);
        }

        /**
        * @将Dictionary转成xml
        * @return 经转换得到的xml串
        * @throws WxPayException
        **/
        protected ResultReturn<string> ToXml(IReadOnlyDictionary<string, OneOf<int, string>> inputData)
        {
            //数据为空时不能转化为xml格式
            if (0 == inputData.Count)
            {
                //Log.Error(this.GetType().ToString(), "WxPayData数据为空!");
                //throw new WxPayException("WxPayData数据为空!");
                return new FailResultReturn<string>("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, OneOf<int, string>> pair in inputData)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value.Value == null)
                {
                    return new FailResultReturn<string>($"{pair.Key}值为null");
                }

                pair.Value.Match(
                    i => xml += "<" + pair.Key + ">" + pair.Value.AsT0 + "</" + pair.Key + ">",
                    s => xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value.AsT1 + "]]></" + pair.Key + ">"
                );
            }
            xml += "</xml>";

            return new SuccessResultReturn<string>(xml);
        }

        protected bool CheckIsSuccess(IReadOnlyDictionary<string, string> dic)
        {
            if (dic == null || !dic.ContainsKey("return_code") || !dic.ContainsKey("result_code"))
            {
                return false;
            }

            if (dic["return_code"].ToStringEx() == "SUCCESS" &&
                dic["result_code"].ToStringEx() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        protected WechatpayConfig Config { get; }

        protected Wechatpay Parent { get; }

        private string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
                baText2BeHashed = enc.GetBytes(plaintext),
                baSalt = enc.GetBytes(salt);
            using System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }

        protected string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        protected string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    /// <summary>
    /// 交易支付的基类
    /// </summary>
    public abstract class PayTradeServiceBase  : WechatpayServiceBase 
    {

        protected string _body = "";
        protected decimal _amount = 0.0m;
        protected string _tradeno = "";
        protected string _attach = "";
        protected string _fee_type = "";
        //protected string _spbill_create_ip = "";
        protected bool _no_credit = false;
        protected DateTime? _time_start = null;
        protected DateTime? _time_expire = null;
        protected bool _profit_sharing = false;

        protected PayTradeServiceBase(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {
        }
        
    }
}