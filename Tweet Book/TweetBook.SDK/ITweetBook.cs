using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.SDK
{
    [Headers("Authorization: Bearer")]
    public interface ITweetBook
    {
        [Get("/api/v1/posts")]
        Task<ApiResponse<List<PostResponse>>> GetAllPostsAsync();

        [Post("/api/v1/posts")]
        Task<ApiResponse<PostResponse>> CreatePostAsync([Body] PostRequest postRequest);

        [Get("/api/v1/posts/{ID}")]
        Task<ApiResponse<PostResponse>> GetAsync(int ID);
    }
}
