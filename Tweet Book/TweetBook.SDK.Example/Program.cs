using Refit;
using System;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;

namespace TweetBook.SDK.Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var token = "";
            var host = "https://localhost:44386/";
            var JWTService = RestService.For<IJWT>(host);
            var TweetBookService = RestService.For<ITweetBook>(host, new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(token)
            });

            var loginResponse = await JWTService.LoginAsync(new LoginJwtRequest
            {
                Email = "user@example.com",
                Password = "P@ssw0rd"
            });
            token = loginResponse.Content.Token;
            var getAllResponse = await TweetBookService.GetAllPostsAsync();

            var createResponse = await TweetBookService.CreatePostAsync(new PostRequest
            {
                Name = "Created via SDK"
            });

            var getResponse = await TweetBookService.GetAsync(createResponse.Content.ID);

        }
    }
}
