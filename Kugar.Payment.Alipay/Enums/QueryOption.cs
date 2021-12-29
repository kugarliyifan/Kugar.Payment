using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Alipay.Enums
{
    public enum QueryOption
    {
        /// <summary>
        /// 返回的交易结算信息，包含分账、补差等信息
        /// </summary>
        TradeSettleInfo,

        /// <summary>
        /// 交易支付使用的资金渠道
        /// </summary>
        FundBillList,

        /// <summary>
        /// 交易支付时使用的所有优惠券信息
        /// </summary>
        VoucherDetailList,

        /// <summary>
        /// 交易支付所使用的单品券优惠的商品优惠信息
        /// </summary>
        DiscountGoodsDetail
    }
}
