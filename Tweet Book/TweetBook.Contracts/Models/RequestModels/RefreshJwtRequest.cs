using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Contracts.Models.RequestModels
{
    public class RefreshJwtRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
