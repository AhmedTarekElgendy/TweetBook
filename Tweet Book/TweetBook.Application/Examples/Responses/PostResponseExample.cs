using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.Application.Examples.Responses
{
    public class PostResponseExample : IExamplesProvider<PostResponse>
    {
        public PostResponse GetExamples()
        {
            return new PostResponse
            {
                Name = "post name",
                UserId = "User ID post"
            };
        }
    }
}
