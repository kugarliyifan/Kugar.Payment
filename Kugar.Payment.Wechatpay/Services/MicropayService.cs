using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fasterflect;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Wechatpay.Services;
using OneOf;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Results;

namespace Kugar.Payment.Wechatpay.Services
{
    /// <summary>
    /// 刷卡支付模式,商家扫用户二维码模式
    /// </summary>
    public class MicropayService : PayTradeServiceBase 
    {
        private string _authCode = "";
        //private string _body = "";
        //private decimal _amount = 0.0m;
        //private string _tradeno = "";
        //private string _attach = "";
        //private string _fee_type = "";
        //private string _spbill_create_ip = "";
        //private bool _no_credit = false;
        //private DateTime? _time_start = null;
        //private DateTime? _time_expire = null;
        //private bool _profit_sharing = false;


        internal MicropayService(Wechatpay pay, WechatpayConfig config) : base(pay, config)
        {

        }

        /// <summary>
        /// 扫码支付付款码，设备读取用户微信中的条码或者二维码信息,前缀 10、11、12、13、14、15开头
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        public MicropayService AuthCode(string authCode)
        {
            _authCode = authCode;
            return this;
        }

        /// <summary>
        /// 商品简单描述，该字段须严格按照规范传递
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual MicropayService Body(string body)
        {
            _body = body;

            return this;
        }

        /// <summary>
        /// 支付金额,单位为元
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual MicropayService Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内（最少6个字符）
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public virtual MicropayService OutTradeNo(string orderNo)
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
        public virtual MicropayService LimitTime(DateTime? startDt, DateTime? endDt)
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
        public virtual MicropayService NoCredit(bool enabled)
        {
            _no_credit = enabled;

            return this;
        }

        /// <summary>
        /// 是否使用分账功能
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public virtual MicropayService ProfitSharing(bool enabled)
        {
            _profit_sharing = enabled;

            return this;
        }

        public virtual MicropayService Attach(string attach)
        {
            _attach = attach;
            return this;
        }

        /// <summary>
        /// 开始支付,如返回值中的returnCode==10003,,表示等待用户支付,请稍后重新查询
        /// </summary>
        /// <returns></returns>
        public async Task<ResultReturn<MicropayResult>> ExecuteAsync()
        {
            var data = new Dictionary<string, OneOf<int, string>>();

            if (string.IsNullOrWhiteSpace(_authCode))
            {
                return new FailResultReturn<MicropayResult>("AuthCode不能为空");
            }

            if (string.IsNullOrWhiteSpace(_body))
            {
                return new FailResultReturn<MicropayResult>("body不能为空");
            }

            if (_amount <= 0)
            {
                return new FailResultReturn<MicropayResult>("amount必须大于0");
            }

            if (string.IsNullOrWhiteSpace(_tradeno))
            {
                return new FailResultReturn<MicropayResult>("out_trade_no不能为空");
            }

            data.AddOrUpdate("auth_code", _authCode); //授权码
            data.AddOrUpdate("body", _body); //商品描述
            data.AddOrUpdate("total_fee", (int)(_amount * 100)); //总金额
            data.AddOrUpdate("out_trade_no", _tradeno); //产生随机的商户订单号
            data.AddIf(!string.IsNullOrWhiteSpace(_attach), "attach", _attach);
            data.AddIf(!string.IsNullOrWhiteSpace(_fee_type), "fee_type", _fee_type)
                //.AddIf(!string.IsNullOrWhiteSpace(_spbill_create_ip), "spbill_create_ip", _spbill_create_ip)
                .AddIf(_time_start.HasValue, "time_start", _time_start.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_time_expire.HasValue, "time_expire", _time_expire.Value.ToString("yyyyMMddHHmmss"))
                .AddIf(_no_credit, "limit_pay", "no_credit")
                .AddIf(_profit_sharing, "profit_sharing","Y")
                ;


            string url = $"{Config.GatewayHost}/pay/micropay";

            var dic = await base.PostData(url, data, 1);

            var result = new MicropayResult(dic.ReturnData);

            if (dic.IsSuccess && result.IsSubscribe)
            {
                if (dic.ReturnData.TryGetValue("transaction_id", out var t))
                {
                    return new SuccessResultReturn<MicropayResult>();
                }
                else
                {
                    return new FailResultReturn<MicropayResult>("交易单号不存在");
                }
            }
            else if (result.Err_Code == "USERPAYING")
            {
                return new FailResultReturn<MicropayResult>("等待用户确认", 10003);
            }
            else if (!dic.ReturnData.ContainsKey("return_code") || dic.ReturnData.TryGetValue("return_code") == "FAIL")
            {
                string returnMsg = dic.ReturnData.TryGetValue("return_msg").ToStringEx();

                return new FailResultReturn<MicropayResult>(returnMsg);

            }
            else
            {
                await Parent.Common().CancelOrder(_tradeno);

                return new FailResultReturn<MicropayResult>(dic.ReturnData.TryGetValue("err_code").ToStringEx());
            }
        }



        /// <summary>
        /// 付款码扫码支付调用结果
        /// </summary>
        public class MicropayResult : WechatPayResultBase
        {
            public MicropayResult(IReadOnlyDictionary<string, string> source) : base(source)
            {
                if (IsSuccess)
                {
                    CashFee = source.TryGetValue("cash_fee").ToDecimal() / 100;
                    SettlementTtotalFee= source.TryGetValue("settlement_total_fee	").ToDecimal() / 100;
                    BankType = source.TryGetValue("bank_type");
                    CashFeeType= source.TryGetValue("cash_fee_type");
                    IsSubscribe = source.TryGetValue("is_subscribe") == "Y";
                    OpenId = source.TryGetValue("openid");
                }

                
            }

            //public string Trade_Type { set; get; }

            /// <summary>
            /// 付款银行
            /// </summary>
            public string BankType { set; get; }

            /// <summary>
            /// 当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额。
            /// </summary>
            public decimal SettlementTtotalFee { set; get; }

            /// <summary>
            /// 现金支付货币类型
            /// </summary>
            public string CashFeeType { set; get; }

            public decimal CashFee { set; get; }

            /// <summary>
            /// 用户是否关注公众账号，仅在公众账号类型支付有效 
            /// </summary>
            public bool IsSubscribe { set; get; }

            /// <summary>
            /// 用户在商户appid 下的唯一标识
            /// </summary>
            public string OpenId { set; get; }
        }

    }
}