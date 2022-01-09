using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Contracts.Models.RequestModels.Queries;

namespace TweetBook.Application.Contracts
{
    public interface ITweetBook
    {
        Task<List<PostResponse>> GetAllPostsAsync(GetAllPostsQuery getAllPostsQuery);

        Task<PostResponse> GetPostAsync(int ID);

        Task<PostResponse> CreatePostAsync(PostRequest postRequest, string UserID);

        Task<PostResponse> UpdatePostAsync(int ID, UpdatePostRequest updatePost);

        Task<bool> DeletePostAsync(int ID);

        Task<bool> IsOwnerForPostAsync(string UserID, int PostID);
    }
}
