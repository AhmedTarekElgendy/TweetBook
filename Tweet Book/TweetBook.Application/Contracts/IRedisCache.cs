using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Contracts
{
    public interface IRedisCache
    {
        Task SetResponseAsync(string CacheKey, object Response, TimeSpan TimeToLive);
        Task<string> GetResponseAsync(string CacheKey);
    }
}
