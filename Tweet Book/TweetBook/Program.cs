using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Application.Data;

namespace TweetBook
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //this code to update DB with new changes from the newly created migrations
            var host =  CreateHostBuilder(args).Build();

            using(var services = host.Services.CreateScope())
            {
                var dbContext = services.ServiceProvider.GetRequiredService<TweetBookDbContext>();

                await dbContext.Database.MigrateAsync();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
