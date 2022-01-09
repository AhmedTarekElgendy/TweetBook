using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.SDK
{
    public interface IJWT
    {
        [Put("/api/token")]
        Task<ApiResponse<JwtResponse>> LoginAsync([Body]LoginJwtRequest loginJwtRequest);
    }
}
