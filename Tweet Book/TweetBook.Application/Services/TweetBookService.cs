using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Application.Data;
using TweetBook.Domain.Models;
using TweetBook.Application.Extentions;
using AutoMapper;
using TweetBook.Contracts.Models.RequestModels.Queries;
using TweetBook.Contracts.Models.RequestModels;
using TweetBook.Contracts.Models.ResponseModels;

namespace TweetBook.Application.Services
{
    public class TweetBookService : ITweetBook
    {
        private readonly TweetBookDbContext dbContext;
        private readonly IMapper mapper;

        public TweetBookService(TweetBookDbContext _dbContext, IMapper _mapper)
        {
            dbContext = _dbContext;
            mapper = _mapper;
        }
        public async Task<PostResponse> CreatePostAsync(PostRequest postRequest, string UserID)
        {
            var post = await dbContext.Posts.AddAsync(new Posts { Name = postRequest.Name, UserId = UserID });

            var created = await dbContext.SaveChangesAsync();

            return created > 0 ? mapper.Map<PostResponse>(post.Entity) : null;
        }

        public async Task<bool> DeletePostAsync(int ID)
        {
            var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.ID == ID);

            if (post == null)
                return false;

            dbContext.Posts.Remove(post);

            var deleted = await dbContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<List<PostResponse>> GetAllPostsAsync(GetAllPostsQuery getAllPostsQuery)
        {
            var Query = mapper.Map<GetAllPostsRequst>(getAllPostsQuery);

            if (Query == null || Query.PageSize < 1 || Query.PageNumber < 1)
            {
                var resp = await dbContext.Posts.ToListAsync();
                return mapper.Map<List<PostResponse>>(resp);
            }

            var skip = (Query.PageNumber - 1) * Query.PageSize;

            var response = await dbContext.Posts.Skip(skip).Take(Query.PageSize).ToListAsync();
            return mapper.Map<List<PostResponse>>(response);
        }


        public async Task<PostResponse> GetPostAsync(int ID)
        {
            var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.ID == ID);

            if (post == null)
                return null;

            return mapper.Map<PostResponse>(post);
        }

        public async Task<bool> IsOwnerForPostAsync(string UserID, int PostID)
        {
            var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.ID == PostID);
            if (post == null)
                return false;

            if (post.UserId != UserID)
                return false;

            return true;
        }

        public async Task<PostResponse> UpdatePostAsync(int ID, UpdatePostRequest updatePost)
        {
            var post = await dbContext.Posts.SingleOrDefaultAsync(p => p.ID == ID);

            if (post == null)
                return null;

            post.Name = updatePost.Name;
            var updatedPost = dbContext.Posts.Update(post);
            var updated = await dbContext.SaveChangesAsync();
            return updated > 0 ? mapper.Map<PostResponse>(updatedPost.Entity) : null;
        }
    }
}
