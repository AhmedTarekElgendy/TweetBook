using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Contracts.Models.RequestModels
{
   public class GetAllPostsRequst
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
