using System;
using System.Threading.Tasks;
using Kugar.Core.BaseStruct;
using Newtonsoft.Json.Linq;

namespace Kugar.Payment.Common
{
    public abstract class ResultBase
    {

        public virtual bool IsSuccess { set; get; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public virtual string Code { set; get; }

        /// <summary>
        /// 错误提示
        /// </summary>
        public virtual string Message { set; get; }

        public virtual bool IsCanceled { set; get; }
        
        /// <summary>
        /// 原始结果字符串
        /// </summary>
        public virtual string RawResult { set; get; }

        public static implicit operator bool(ResultBase d)
        {
            return d.IsSuccess;
        }
        
    }
    
}
