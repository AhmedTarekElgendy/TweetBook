using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Application.Data;

namespace TweetBook.Tests
{
    public class IntegrationTest : IDisposable
    {
        protected HttpClient httpClient;
        private IServiceProvider serviceProvider;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
            //this is used to use a memory DB for testing
                .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(TweetBookDbContext));
                    services.AddDbContext<TweetBookDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("testdb");
                    });
                });
            });
            serviceProvider = appFactory.Services;
            httpClient = appFactory.CreateClient();
        }
        protected async Task AuthenticateAsync()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJWTTokenAsync());
        }

        private async Task<string> GetJWTTokenAsync()
        {
            var response = await httpClient.PutAsJsonAsync(GetAuthenticationURL(), new RegisterJwtRequest
            {
                Email = "user@example.com",
                Password = "P@ssw0rd"
            });
            var maapedResponse = await response.Content.ReadAsAsync<JwtResponse>();
            return maapedResponse.Token;
        }

        protected string GetAppHost() => "https://localhost:44386";
        protected string GetAppVersion() => "/api/v1";
        private string GetAuthenticationURL() => $"{GetAppHost()}/api/token";

        public void Dispose()
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
