using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;
using TweetBook.Application.Options;

namespace TweetBook.Application.Cache
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int timeToLive;

        public CachedAttribute(int _timeToLive)
        {
            timeToLive = _timeToLive;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before he enters the controller
            var redisOptions = context.HttpContext.RequestServices.GetRequiredService<RedisOptions>();

            if (!redisOptions.Enabled)
            {
                await next();
                return;
            }
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IRedisCache>();
            var requestKey = GenerateRequestKey(context.HttpContext.Request);

            var response = await cacheService.GetResponseAsync(requestKey);
            if (response != null)
            {
                var content = new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = 200,
                    Content = response
                };
                context.Result = content;
                return;
            }
            
            var endpointResponse= await next();
            //after he enter the controller

            if (endpointResponse.Result is OkObjectResult okObjectResult)//here I check if response of get is 200
            {
                await cacheService.SetResponseAsync(requestKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLive));
            }


        }

        private string GenerateRequestKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);

            foreach (var (key,value) in request.Query.OrderBy(q=>q.Key))
            {
                keyBuilder.Append($"{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
