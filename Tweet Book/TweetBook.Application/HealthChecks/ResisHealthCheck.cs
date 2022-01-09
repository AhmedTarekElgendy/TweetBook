using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TweetBook.Application.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer multiplexer;

        public RedisHealthCheck(IConnectionMultiplexer _multiplexer)
        {
            multiplexer = _multiplexer;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var db= multiplexer.GetDatabase();
                db.StringGet("Health");
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch(Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
            }
        }
    }
}
