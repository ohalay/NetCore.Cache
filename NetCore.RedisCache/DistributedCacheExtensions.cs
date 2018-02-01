using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace NetCore.RedisCache
{
    public static class DistributedCacheExtensions
    {
        public static T GetOrCreate<T>(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, T> func)
        {
            T objResult = default(T);

            var stringValue = cache.GetString(key);
            if (stringValue == null)
            {
                var options = new DistributedCacheEntryOptions();
                objResult = func(options);

                stringValue = JsonConvert.SerializeObject(objResult);
                cache.SetString(key, stringValue, options);
            }
            else
            {
                objResult = JsonConvert.DeserializeObject<T>(stringValue);
            }

            return objResult;
        }

        public static void Set<T>(this IDistributedCache cache, string key, T Value, DateTimeOffset absoluteExpiration)
        {
            var stringValue = JsonConvert.SerializeObject(Value);
            cache.SetString(key, stringValue, new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration });
        }

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            T objResult = default(T);

            var stringValue = cache.GetString(key);
            if (stringValue != null)
            {
                objResult = JsonConvert.DeserializeObject <T>(stringValue);
            }

            return objResult;
        }
    }
}
