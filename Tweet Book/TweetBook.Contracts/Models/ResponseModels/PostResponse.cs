using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Application.Contracts.Models.ResponseModels
{
    public class PostResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }


    }
}
