using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Application.Cache;
using TweetBook.Application.Contracts;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Application.Examples.Requests;
using TweetBook.Application.Extentions;
using TweetBook.Contracts.Models.RequestModels.Queries;
using TweetBook.Contracts.Models.ResponseModels;

namespace TweetBook.Controllers.V1
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiVersion("1")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ITweetBook tweetBook;
        private readonly IURIGenerator uriGenerator;

        public PostsController(ITweetBook _tweetBook, IURIGenerator _uriGenerator)
        {
            this.tweetBook = _tweetBook;
            uriGenerator = _uriGenerator;
        }

        /// <summary>
        /// Returns All the posts
        /// </summary>
        [HttpGet]
        //[Cached(600)]
        public async Task<IActionResult> GetAllPosts([FromQuery] GetAllPostsQuery getAllPostsQuery = null)
        {

            var response = await tweetBook.GetAllPostsAsync(getAllPostsQuery);
            var mappedResponse = new Response<PostResponse> { Data = response };
            if (getAllPostsQuery == null || getAllPostsQuery.PageSize < 1 || getAllPostsQuery.PageNumber < 1
                || getAllPostsQuery.PageSize == null || getAllPostsQuery.PageNumber == null)
            {
                return Ok(mappedResponse);
            }
            return Ok(GenrateResponseURI(getAllPostsQuery, response, mappedResponse));
        }

        public Response<T> GenrateResponseURI<T>(GetAllPostsQuery getAllPostsQuery, List<PostResponse> response, Response<T> mappedResponse)
        {
            var nextPage = uriGenerator.GetAllPostsUri(new GetAllPostsQuery
            {
                PageNumber = getAllPostsQuery.PageNumber + 1,
                PageSize = getAllPostsQuery.PageNumber
            });
            var perviousPage = uriGenerator.GetAllPostsUri(new GetAllPostsQuery
            {
                PageNumber = getAllPostsQuery.PageNumber - 1,
                PageSize = getAllPostsQuery.PageNumber
            });

            return new Response<T>
            {
                Data = (IEnumerable<T>)response,
                PageNumber = getAllPostsQuery.PageNumber,
                PageSize = getAllPostsQuery.PageSize,
                NextPage = response.Any() ?
             nextPage?.ToString() : null,
                PreviousPage = response.Any() ?
             perviousPage?.ToString() : null
            };
        }

        /// <summary>
        /// Create new post
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PostResponse), 201)]
        [ProducesResponseType(typeof(PostResponse), 400)]
        public async Task<IActionResult> CreatePost([FromBody] PostRequest postRequest)
        {
            if (postRequest == null)
                return BadRequest("Please enter a valid request");

            var response = await tweetBook.CreatePostAsync(postRequest, HttpContext.GetUserID());

            var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationURL = $"{baseURL}{HttpContext.Request.Path}/{response.ID}";

            return Created(locationURL, response);
        }


        [HttpGet("{ID}")]
        //[Authorize(Roles = "PostAdmin")]
        public async Task<IActionResult> GetPost([FromRoute] int ID)
        {
            var response = await tweetBook.GetPostAsync(ID);

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdatePost([FromRoute] int ID, [FromBody] UpdatePostRequest updatePost)
        {
            var isOwner = await tweetBook.IsOwnerForPostAsync(HttpContext.GetUserID(), ID);

            if (!isOwner)
                return Forbid();

            if (updatePost == null)
                return BadRequest("Please enter a valid request");

            var response = await tweetBook.UpdatePostAsync(ID, updatePost);

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpDelete("{ID}")]
        //[Authorize(policy: "postowner")]
        [Authorize(policy: "MicrosoftPolicy")]
        public async Task<IActionResult> DeletePost([FromRoute] int ID)
        {
            var isOwner = await tweetBook.IsOwnerForPostAsync(HttpContext.GetUserID(), ID);

            if (!isOwner)
                return Forbid();

            var response = await tweetBook.DeletePostAsync(ID);

            if (!response)
                return NotFound();

            return Ok();
        }
    }
}

