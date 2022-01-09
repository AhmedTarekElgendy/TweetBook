using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Contracts.Models.ResponseModels
{
    public class Response<T>
    {
        public Response()
        {

        }
        public Response(IEnumerable<T> response)
        {
            Data = response;
        }
        public IEnumerable<T> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? NextPage { get; set; }
        public string? PreviousPage { get; set; }
    }
}
