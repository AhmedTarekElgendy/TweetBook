using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.Models.RequestModels.Queries;

namespace TweetBook.Application.Contracts
{
    public interface IURIGenerator
    {
        Uri GetAllPostsUri(GetAllPostsQuery getAllPostsQuery );
    }
}
