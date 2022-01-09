using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;

namespace TweetBook.Application.Services
{
    public class RedisCacheService : IRedisCache
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
        public async Task<string> GetResponseAsync(string CacheKey)
        {
            var Response = await distributedCache.GetStringAsync(CacheKey);
            return string.IsNullOrEmpty(Response) ? null : Response;
        }

        public async Task SetResponseAsync(string CacheKey, object Response, TimeSpan TimeToLive)
        {
            if (Response == null)
                return;

            var serializedResponse = JsonConvert.SerializeObject(Response);
            await distributedCache.SetStringAsync(CacheKey, serializedResponse,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeToLive });
        }
    }
}
