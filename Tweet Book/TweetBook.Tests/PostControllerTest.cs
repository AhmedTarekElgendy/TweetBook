using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Domain.Models;
using Xunit;

namespace TweetBook.Tests
{
    public class PostControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAllPosts_ReturnEmptyList_TakeNoParams()
        {
            //Arrange
            await AuthenticateAsync();
            //Act
            var response = await httpClient.GetAsync(GetPostControllerURL());


            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var mappedResponse =await response.Content.ReadAsAsync<List<Posts>>();
            mappedResponse.Should().NotBeEmpty();

        }
        [Fact]
        public async Task CreatePost_ReturnNewPost_WithPostObjectInParam()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response= await httpClient.PostAsJsonAsync(GetPostControllerURL(), new PostRequest
            {
                Name = "New Post Added"
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        private string GetPostControllerURL() => $"{GetAppHost()}{GetAppVersion()}/posts";

    }
}
