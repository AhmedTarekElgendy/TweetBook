using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;

namespace TweetBook.Application.Examples.Requests
{
    public class PostRequestExample : IExamplesProvider<PostRequest>
    {
        public PostRequest GetExamples()
        {
            return new PostRequest
            {
                Name = "example name"
            };
        }
    }
}
