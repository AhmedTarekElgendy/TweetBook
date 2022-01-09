using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;
using TweetBook.Contracts.Models.RequestModels.Queries;

namespace TweetBook.Application.Services
{
    public class URIGeneratorService : IURIGenerator
    {
        private readonly string baseURI;

        public URIGeneratorService(string _baseURI)
        {
            baseURI = _baseURI;
        }
        public Uri GetAllPostsUri(GetAllPostsQuery getAllPostsQuery)
        {
            if (getAllPostsQuery == null || getAllPostsQuery.PageSize < 1 || getAllPostsQuery.PageNumber < 1
    || getAllPostsQuery.PageSize == null || getAllPostsQuery.PageNumber == null)
            {
                return null;
            }

            var queryDictionary = new Dictionary<string, string>
            {
                {"pageNumber",getAllPostsQuery.PageNumber.ToString() },
                {"pageSize",getAllPostsQuery.PageSize.ToString() }
            };
            var modifiedURI = QueryHelpers.AddQueryString(baseURI, queryDictionary);

            return new Uri(modifiedURI);
        }
    }
}
