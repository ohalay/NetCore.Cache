using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace NetCore.RedisCache.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        const string defaultKey = "defaultKey";
        const string defaultValue = "defaultValue";
        private readonly DateTimeOffset absoluteExpiration = DateTimeOffset.Now.AddMinutes(1);

        private IDistributedCache distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
        // GET api/values
        [HttpGet]
        public string GetOrCreate()
        {
            return this.distributedCache.GetOrCreate(defaultKey, entry =>
            {
                entry.AbsoluteExpiration = absoluteExpiration;
                return defaultValue;
            });
        }

        // GET api/values/5
        [HttpGet("{key}")]
        public string Get(string key)
        {
            return this.distributedCache.Get<string>(key);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody](string key, string value) pair)
        {
            this.distributedCache.Set(pair.key, pair.value, absoluteExpiration);
        }


        // DELETE api/values/5
        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            this.distributedCache.Remove(key);
        }
    }
}
