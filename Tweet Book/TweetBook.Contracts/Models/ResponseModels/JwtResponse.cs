using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Contracts.Models.ResponseModels
{
    public class JwtResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<Error> Errors { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
