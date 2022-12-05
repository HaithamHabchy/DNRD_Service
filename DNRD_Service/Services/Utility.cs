using System;
using System.Runtime.Caching;
using DNRD_Service.Services.IServices;
using Newtonsoft.Json;

namespace DNRD_Service.Services
{
    public class Utility : IUtility
    {
        public object GetCache(string key)
        {
            ObjectCache cache = MemoryCache.Default;
            dynamic content = cache[key];
            return content;
        }

        public void SetCache(string key, object content, bool neverExpires = false)
        {
            var policy = new CacheItemPolicy();
            if (!neverExpires)
            {
                // Cache expires every 120 minutes
                policy.AbsoluteExpiration = DateTime.UtcNow.AddMinutes(120);
            }
            ObjectCache cache = MemoryCache.Default;
            cache.Set(key, content, policy);
        }

        public string SerializeToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T DeserializeFromJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public object GetKeyValue(dynamic dictionary, string path, object defaultValue = null)
        {
            var arr = path.Split('.');

            foreach (var key in arr)
            {
                if (!dictionary.ContainsKey(key))
                {
                    return defaultValue;
                }
                dictionary = dictionary[key];
            }

            return dictionary;
        }
    }
}
