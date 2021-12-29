using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aop.Api;
using Kugar.Core.BaseStruct;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Alipay.Enums;
using Kugar.Payment.Alipay.Results;
using Kugar.Payment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Alipay.PaymentArguments
{ 
    public abstract class AlipayRequestBase
    {
        public string NotifyUrl { set; get; }
        
        public abstract string Method { get; }

        public abstract IReadOnlyDictionary<string, object> GetParameters();

        public abstract ResultReturn CheckParameter();
    }

    public abstract class AlipayRequestBase<TResult> : AlipayRequestBase where TResult : AlipayResultBase
    {

    }

    public class F2FRequest:AlipayRequestBase<F2FPaymentResult>
    {
        /// <summary>
        /// 商户单号
        /// </summary>
        public string OutTradeNo { set; get; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { set; get; }

        /// <summary>
        /// 订单标题。不可使用特殊字符，如 /，=，& 等(可选)
        /// </summary>
        public string Subject { set; get; }

        /// <summary>
        /// 支付授权码25~30开头的长度为16~24位的数字或刷脸付fp开头的35位字符串
        /// </summary>
        public string AuthCode { set; get; }

        /// <summary>
        /// 产品码。商家和支付宝签约的产品码。
        /// </summary>
        public string ProductCode { set; get; } = "FACE_TO_FACE_PAYMENT";

        /// <summary>
        /// 卖家支付宝用户ID (可选)
        /// </summary>
        public string SellerId { set; get; }

        /// <summary>
        /// 商户门店编号
        /// </summary>
        public string StoreId { set; get; }

        /// <summary>
        /// 商户操作员编号
        /// </summary>
        public string OperatorId { set; get; }

        /// <summary>
        /// 商户机具终端编号
        /// </summary>
        public string TerminalId { set; get; }

        /// <summary>
        /// 返回参数选项。商户通过传递该参数来定制同步需要额外返回的信息字段
        /// </summary>
        public QueryOption[] QueryOptions { set; get; }

        /// <summary>
        /// 订单包含的商品列表信息，(可选)
        /// </summary>
        public GoodDetail[] GoodsDetail { set; get; }
        


        /// <summary>
        /// 商品信息
        /// </summary>
        public class GoodDetail: IExtentData
        {
            /// <summary>
            /// 商品的编号	
            /// </summary>
            public string GoodsId { set; get; }

            /// <summary>
            /// 商品名称	
            /// </summary>
            public string GoodsName { set; get; }

            /// <summary>
            /// 商品数量	
            /// </summary>
            public decimal Quantity { set; get; }

            /// <summary>
            /// 商品单价，单位为元	
            /// </summary>
            public decimal Price { set; get; }

            /// <summary>
            /// 商品类目	
            /// </summary>
            public string GoodsCategory { set; get; }

            /// <summary>
            /// 商品类目树，从商品类目根节点到叶子节点的类目id组成，类目id值使用|分割
            /// </summary>
            public string CategoriesTree { set; get; }

            /// <summary>
            /// 商品的展示地址	
            /// </summary>
            public string ShowUrl { set; get; }

            /// <summary>
            /// 检查参数是否合规
            /// </summary>
            /// <returns></returns>
            public ResultReturn Check()
            {
                if (string.IsNullOrWhiteSpace(GoodsId))
                {
                    return new FailResultReturn("商品编号不能为空")
                    {
                        Error = new ArgumentNullException(nameof(GoodsId), "商品编号不能为空")
                    };
                }

                if (string.IsNullOrWhiteSpace(GoodsName))
                {
                    return new FailResultReturn("商品名称不能为空")
                    {
                        Error = new ArgumentNullException(nameof(GoodsName), "商品名称不能为空")
                    };
                }

                if (Quantity<=0)
                {
                    return new FailResultReturn("商品数量必须大于0")
                    {
                        Error = new ArgumentNullException(nameof(Quantity), "商品数量必须大于0")
                    };
                }

                return SuccessResultReturn.Default;
            }

            /// <summary>
            /// 输出格式化后的json
            /// </summary>
            /// <returns></returns>
            public JObject ToJson()
            {
                var ret = Check();

                if (!ret)
                {
                    throw new ArgumentException(ret.Message);
                }

                var json = new JObject()
                {
                    ["goods_id"]=GoodsId,
                    ["goods_name"]=GoodsName,
                    ["quantity"]=Quantity,
                    ["price"]=Price
                };
                
                json
                    .AddPropertyIf(!string.IsNullOrWhiteSpace(GoodsCategory), "goods_category",GoodsCategory)
                    .AddPropertyIf(!string.IsNullOrWhiteSpace(CategoriesTree), "categories_tree",CategoriesTree)
                    .AddPropertyIf(!string.IsNullOrWhiteSpace(ShowUrl), "show_url", ShowUrl);

                return json;
            }
        }

        public override string Method => "alipay.trade.pay";

        public override IReadOnlyDictionary<string, object> GetParameters()
        {
            var dic = new Dictionary<string, object>()
            {
                ["notify_url"] = NotifyUrl,
                ["out_trade_no"] = OutTradeNo,
                ["total_amount"] = Amount,
                ["subject"] = Subject,
                ["scene"] = AuthCode.StartsWith("fp", true, null) ? "security_code" : "bar_code",
                ["auth_code"] = AuthCode,
                ["product_code"]=ProductCode,
                ["seller_id"]=SellerId,
                ["goods_detail"]=GoodsDetail,
                ["store_id"]=StoreId,
                ["operator_id"]=OperatorId,
                ["terminal_id"]= TerminalId,
                ["query_options"]= QueryOptions
                    ?.Select(x=>x.Switch("")
                        .Case(QueryOption.DiscountGoodsDetail, "discount_goods_detail")
                        .Case(QueryOption.FundBillList, "fund_bill_list")
                        .Case(QueryOption.TradeSettleInfo, "trade_settle_info")
                        .Case(QueryOption.VoucherDetailList, "voucher_detail_list")
                    ).ToArrayEx(),

            };

            return dic;
        }

        public override ResultReturn CheckParameter()
        {
            if (string.IsNullOrWhiteSpace(OutTradeNo))
            {
                return new FailResultReturn("OutTradeNo不能为空")
                {
                    Error = new ArgumentNullException(nameof(OutTradeNo), "OutTradeNo不能为空")
                };
            }

            if (string.IsNullOrWhiteSpace(AuthCode))
            {
                return new FailResultReturn("AuthCode付款码不能为空")
                {
                    Error = new ArgumentNullException(nameof(AuthCode), "AuthCode付款码不能为空")
                };
            }

            if (Amount<=0)
            {
                return new FailResultReturn("Amount付款金额必须大于0")
                {
                    Error = new ArgumentOutOfRangeException(nameof(Amount), "Amount付款金额必须大于0")
                };
            }

            if (GoodsDetail.HasData())
            {
                foreach (var detail in GoodsDetail)
                {
                    var ret = detail.Check();

                    if (!ret)
                    {
                        return ret;
                    }
                }
            }

            return SuccessResultReturn.Default;
        }
    }
 

    public interface IExtentData
    {
        ResultReturn Check();

        JObject ToJson();
    }
}
