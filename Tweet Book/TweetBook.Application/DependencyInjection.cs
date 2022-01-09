using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;
using TweetBook.Application.Data;
using TweetBook.Application.Filters;
using TweetBook.Application.Options;
using TweetBook.Application.Services;
using TweetBook.Application.Validators;

namespace TweetBook.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TweetBookDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITweetBook, TweetBookService>();

            services.AddScoped<IJwt, JwtService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddFluentValidation(f => f.RegisterValidatorsFromAssemblyContaining<PostRequestValidator>());

            var redisOptions = new RedisOptions();
            configuration.Bind(nameof(RedisOptions), redisOptions);
            if (redisOptions.Enabled)
                services.AddStackExchangeRedisCache(options => options.Configuration = redisOptions.ConnectionString);

            services.AddSingleton(redisOptions);
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            });
            services.AddSingleton<IRedisCache, RedisCacheService>();

            services.AddSingleton<IURIGenerator>(provider =>
           {
               var accessor = provider.GetRequiredService<IHttpContextAccessor>();
               var request = accessor.HttpContext.Request;
               var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
               return new URIGeneratorService(uri);
           });
            return services;
        }
    }
}
