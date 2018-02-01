using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace NetCore.MemoryCache.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        const string defaultKey = "defaultKey";
        const string defaultValue = "defaultValue";
        private readonly DateTimeOffset absoluteExpiration = DateTimeOffset.Now.AddMinutes(1);

        private IMemoryCache memoryCache;

        public ValuesController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        // GET api/values
        [HttpGet]
        public string GetOrCreate()
        {
            return this.memoryCache.GetOrCreate(defaultKey, entry => 
            {
                entry.AbsoluteExpiration = absoluteExpiration;
                return defaultValue;
            } );
        }

        // GET api/values/5
        [HttpGet("{key}")]
        public string Get(string key)
        {
            return this.memoryCache.Get<string>(key);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody](string key, string value) pair)
        {
            this.memoryCache.Set(pair.key, pair.value, absoluteExpiration);
        }


        // DELETE api/values/5
        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            this.memoryCache.Remove(key);
        }
    }
}
