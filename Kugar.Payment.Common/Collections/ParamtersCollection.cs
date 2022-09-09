using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Kugar.Core.ExtMethod;
using Newtonsoft.Json.Linq;
using OneOf;

namespace Kugar.Payment.Common.Collections
{
    public class ParamtersCollection
    {
        private SortedDictionary<string, OneOf<string, int,decimal>> _args = null;
        private int _digits = 2;

        public ParamtersCollection(IComparer<string> comparer = null,int digits=2)
        {
            _args = new SortedDictionary<string, OneOf<string, int, decimal>>(comparer);
        }

        public ParamtersCollection Set(string key, string value, bool escap = false)
        {
            _args.Add(key,escap? HttpUtility.JavaScriptStringEncode(value) :value);

            return this;
        }

        public ParamtersCollection Set(string key, int value)
        {
            
            _args.Add(key,value);

            return this;
        }

        public ParamtersCollection Set(string key, decimal value)
        {
            _args.Add(key,value);

            return this;
        }

        public ParamtersCollection SetIf(bool checker, string key, string value, bool escap = false)
        {
            if (!checker)
            {
                return this;
            }

            return Set(key, value, escap);
        }

        public ParamtersCollection SetIf(bool checker, string key, int value)
        {
            if (!checker)
            {
                return this;
            }

            return Set(key, value);
        }

        public ParamtersCollection SetIf(bool checker, string key, decimal value)
        {
            if (!checker)
            {
                return this;
            }

            return Set(key, value);
        }

        public ParamtersCollection Remove(string key)
        {
            _args.Remove(key);

            return this;
        }

        public JObject GetJson(bool isIncludeNullOrEmpty = false)
        {
            var json = new JObject();

            foreach (var item in _args)
            {
                if (item.Value.IsT0)
                {
                    if (!isIncludeNullOrEmpty && string.IsNullOrWhiteSpace(item.Value.AsT0))
                    {
                        continue;
                    }
                    json.Add(item.Key,item.Value.AsT0);
                }
                else if (item.Value.IsT1)
                {
                    json.Add(item.Key,item.Value.AsT1);
                }
                else if (item.Value.IsT2)
                {
                    json.Add(item.Key,item.Value.AsT2.ToString("F" + _digits));
                }
            }

            return json;
        }

        public string GetUrl(ValueHandler valueHandler=null, char splitChar = '&', bool isIncludeNullOrEmpty = false)
        {
            var s = $"D{_digits}";
            
            var sb = new StringBuilder(256);

            foreach (var item in _args)
            {
                var v = valueHandler?.Invoke(item.Key, item.Value) ?? item.Value;
                
                if (v.IsT0)
                {
                    if (!isIncludeNullOrEmpty && string.IsNullOrWhiteSpace(item.Value.AsT0))
                    {
                        continue;
                    }

                    sb.AppendFormat("{0}={1}{2}", item.Key, item.Value.AsT0, splitChar); 
                }
                else if (v.IsT1)
                {
                    sb.AppendFormat("{0}={1}{2}", item.Key, item.Value.AsT1, splitChar);
                }
                else if (v.IsT2)
                {
                    sb.AppendFormat("{0}={1}{2}", item.Key, item.Value.AsT2.ToString(s), splitChar);
                }
            }

            if (sb[-1]==splitChar)
            {
                return sb.ToString(0, sb.Length - 1);
            }
            else
            {
                return sb.ToStringEx();
            }
        }
    }

    public delegate OneOf<string, int, decimal> ValueHandler(string key, OneOf<string, int, decimal> value);
}
