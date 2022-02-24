using System;
using System.Collections.Generic;
using System.Linq;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common.Helpers;
using Kugar.Payment.Wechatpay.Enums;

namespace Kugar.Payment.Wechatpay.Results
{
    public class NotifyPaymentResult : WechatPayResultBase
    {
        public NotifyPaymentResult(IReadOnlyDictionary<string, string> source) : base(source)
        {
            if (IsSuccess)
            {
                DeviceInfo = source.TryGetValue("device_info");
                IsSubscribe = source.TryGetValue("is_subscribe") == "Y";
                TradeType = source.TryGetValue("trade_type")
                    .Switch(TradeType.JSAPI)
                    .Case("JSAPI", TradeType.JSAPI)
                    .Case("NATIVE", TradeType.NATIVE)
                    .Case("APP", TradeType.APP)
                    .Result;

                BankType = source.TryGetValue("bank_type");
                TotalFee = source.TryGetValue("total_fee").ToDecimal() / 100;
                TotalAmount = TotalFee;
                SettlementTotalFee = source.TryGetValue("settlement_total_fee").ToDecimal() / 100;
                FeeType = source.TryGetValue("fee_type");
                CashFee = source.TryGetValue("cash_fee").ToDecimal() / 100;
                CashFeeType = source.TryGetValue("cash_fee_type");
                OpenId = source.TryGetValue("openid");
                CouponFee = source.TryGetValue("coupon_fee").ToDecimal() / 100;
                var couponCount = source.TryGetValue("coupon_count").ToInt();

                if (couponCount > 0)
                {
                    CouponInfoList = Enumerable.Range(0, couponCount).Select(x => new CouponInfo()
                    {
                        CouponFee = source.TryGetValue($"coupon_fee_{x}").ToDecimal() / 100,
                        CouponId = source.TryGetValue($"coupon_id_{x}"),
                        Coupontype = source.TryGetValue($"coupon_type_{x}")
                            .Switch(CouponType.Cash)
                            .Case("CASH", CouponType.Cash)
                            .Case("NO_CASH", CouponType.NoCash)
                            .Result

                    }).ToArrayEx();
                }
                else
                {
                    CouponInfoList = Array.Empty<CouponInfo>();
                }

            }
        }

        public string DeviceInfo { set; get; }

        public bool IsSubscribe { get; }

        public string OpenId { get; }

        /// <summary>
        /// 交易类型	
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// 付款银行	
        /// </summary>
        public string BankType { get; }

        /// <summary>
        /// 货币种类	
        /// </summary>
        public string FeeType { get; }

        /// <summary>
        /// 订单总金额,单位为元
        /// </summary>
        public decimal TotalFee { get; }

        /// <summary>
        /// 现金支付金额,单位为元
        /// </summary>
        public decimal CashFee { get; }

        /// <summary>
        /// 应结订单金额	应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额 
        /// </summary>
        public decimal SettlementTotalFee { get; }

        /// <summary>
        /// 现金支付货币类型
        /// </summary>
        public string CashFeeType { get; }

        /// <summary>
        /// 总代金券金额	,单位为元
        /// </summary>
        public decimal CouponFee { get; }

        /// <summary>
        /// 本次交易所使用的优惠券信息
        /// </summary>
        public IReadOnlyList<CouponInfo> CouponInfoList { get; }

        /// <summary>
        /// 现金券信息
        /// </summary>
        public class CouponInfo
        {
            /// <summary>
            /// 代金券类型	
            /// </summary>
            public CouponType Coupontype { set; get; }

            /// <summary>
            /// 代金券ID	
            /// </summary>
            public string CouponId { set; get; }

            /// <summary>
            /// 单个代金券支付金额	,单位为元
            /// </summary>
            public decimal CouponFee { set; get; }
        }
    }
}