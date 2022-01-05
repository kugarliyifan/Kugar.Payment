﻿using System;
using System.Collections.Generic;
using System.Text;
using Kugar.Core.ExtMethod;
using Kugar.Payment.Common;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.DragonPay.Results
{
    /// <summary>
    /// 龙支付结果基类
    /// </summary>
    public abstract class DragonResultBase
    {
        public DragonResultBase(JObject json)
        {
            Code=json.GetString("ERRCODE");
            Msg = json.GetString("ERRMSG");

            IsSuccess = json.GetString("RESULT") == "Y";
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string Code { set; get; }
        
        /// <summary>
        /// 错误提示
        /// </summary>
        public string Msg { protected set; get; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { set; get; }
    }

    /// <summary>
    /// 龙支付交易类结果基类
    /// </summary>
    public abstract class DragonPaymentResultBase:DragonResultBase,ICommonPaymentResult
    {
        /// <summary>
        /// 交易单号
        /// </summary>
        public string TransactionId { get;   set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string OutTradeNo { get;  set; }
        

        protected DragonPaymentResultBase(JObject json) : base(json)
        {
            OutTradeNo = json.GetString("ORDERID");

            if (IsSuccess)
            {
                TransactionId = json.GetString("TRACEID");
            }
        }
    }
}