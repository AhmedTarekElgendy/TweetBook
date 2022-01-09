using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.Application.Contracts
{
    public interface IJwt
    {
        Task<JwtResponse> Register(string email , string password);
        Task<JwtResponse> Login(string email, string password);
        Task<JwtResponse> RefreshToken(string token, string refreshToken);
    }
}
