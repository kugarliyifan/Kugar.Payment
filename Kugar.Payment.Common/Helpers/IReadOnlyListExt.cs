using OneOf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kugar.Payment.Common.Helpers
{
    public static class IReadOnlyExt
    {
        public static TValue TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source, TKey key,TValue defaultValue=default)
        {
            if (source.TryGetValue(key,out var tmp))
            {
                return tmp;
            }
            else
            {
                return defaultValue;
            }
        }

        public static Dictionary<TKey, TValue> AddIf<TKey, TValue>(this Dictionary<TKey, TValue> source,bool isAdd, TKey key,
            TValue value)
        {
            if (isAdd)
            {
                if (source.ContainsKey(key))
                {
                    return source;
                }
                else
                {
                    source.Add(key, value);
                    return source;
                }
            }
            else
            {
                return source;
            }
        }

        public static Dictionary<TKey, TValue> AddIf<TKey, TValue>(this Dictionary<TKey, TValue> source, bool isAdd, TKey key,
            Func<TValue> valueFactory)
        {
            if (isAdd)
            {
                if (source.ContainsKey(key))
                {
                    return source;
                }
                else
                {
                    source.Add(key, valueFactory());
                    return source;
                }
            }
            else
            {
                return source;
            }
        }
    }
}
