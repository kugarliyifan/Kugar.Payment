using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CCB_B2CPay_Util;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Core.Log;
using Kugar.Core.Network;
using Kugar.Payment.DragonPay.Requests;
using Newtonsoft.Json.Linq;
using OneOf;

namespace Kugar.Payment.DragonPay.Services
{
    public abstract class DragonServiceBase
    {
        protected DragonServiceBase(Payment.DragonPay.DragonPay pay, DragonPayConfig config)
        {
            this.Config = config;
            this.Parent = pay;
        }

        protected async Task<ResultReturn<JObject>> PostData(string url, DragonRequestBase request, int retryCount = 5, bool useClientCert = false)
        {
            var tmpStr = $"MERCHANTID={Config.MerchantId}&POSID={Config.PosId}&BRANCHID={Config.BranchId}&{request.ToUrl()}";

            var ccParam = new CCBPayUtil().makeCCBParam(tmpStr, Config.PubKey);
            
            try
            {
                var http = WebHelper.Create(url)
                    .ContentType(WebHelper.ContentTypeEnum.FormUrlencoded)
                    .SetParameter("MERCHANTID", Config.MerchantId)
                    .SetParameter("POSID",Config.PosId)
                    .SetParameter("BRANCHID",Config.BranchId)
                    .SetParameter("ccbParam", ccParam) 
                    .RetryCount(retryCount);

                LoggerManager.Default.Debug("龙支付调用参数:" + tmpStr);

                //if (useClientCert)
                //{
                //    http.AddCertificate(new X509Certificate2(Config.CertData, Config.CertPassword));
                //}

                var response = await http.Post_StringAsync();

                LoggerManager.Default.Debug("龙支付回复参数:" + response);

                if (response.Contains("ccbParam deciphering fail"))
                {
                    return new FailResultReturn<JObject>("请检查传入参数是否正确");
                }

                //var dic = FromXml(response);

                return new SuccessResultReturn<JObject>(JObject.Parse(response));
            }
            catch (Exception e)
            {
                return new FailResultReturn<JObject>(e);
            }
        }



        protected DragonPayConfig Config { get; }

        protected Payment.DragonPay.DragonPay Parent { get; }

    }

    ///// <summary>
    ///// 交易支付的基类
    ///// </summary>
    //public abstract class PayTradeServiceBase  : DragonServiceBase 
    //{

    //    protected string _body = "";
    //    protected decimal _amount = 0.0m;
    //    protected string _tradeno = "";
    //    protected string _attach = "";
    //    protected string _fee_type = "";
    //    //protected string _spbill_create_ip = "";
    //    protected bool _no_credit = false;
    //    protected DateTime? _time_start = null;
    //    protected DateTime? _time_expire = null;
    //    protected bool _profit_sharing = false;

    //    protected PayTradeServiceBase(Wechatpay pay, WechatpayConfig config) : base(pay, config)
    //    {
    //    }
        
    //}
}