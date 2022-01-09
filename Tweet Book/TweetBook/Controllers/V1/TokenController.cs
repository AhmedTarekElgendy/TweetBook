using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.RequestModels;
using TweetBook.Application.Contracts;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwt jwt;

        public TokenController(IJwt _jwt)
        {
            jwt = _jwt;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterJwtRequest registerJwtRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(errors => errors.Errors.Select(error => error.ErrorMessage)));


            var registerTokenResponse = await jwt.Register(registerJwtRequest.Email, registerJwtRequest.Password);

            return registerTokenResponse.IsSuccess ?
                Ok(new JwtResponse
                {
                    Token = registerTokenResponse.Token,
                    RefreshToken = registerTokenResponse.RefreshToken,
                    IsSuccess = registerTokenResponse.IsSuccess
                })
                : BadRequest(new JwtResponse { Errors = registerTokenResponse.Errors, IsSuccess = registerTokenResponse.IsSuccess });
        }

        [HttpPut]
        public async Task<IActionResult> Login([FromBody] LoginJwtRequest registerJwtRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(errors => errors.Errors.Select(error => error.ErrorMessage)));

            var loginTokenResponse = await jwt.Login(registerJwtRequest.Email, registerJwtRequest.Password);

            return loginTokenResponse.IsSuccess ?
                Ok(new JwtResponse
                {
                    Token = loginTokenResponse.Token,
                    RefreshToken = loginTokenResponse.RefreshToken,
                    IsSuccess = loginTokenResponse.IsSuccess
                })
                : BadRequest(new JwtResponse { Errors = loginTokenResponse.Errors, IsSuccess = loginTokenResponse.IsSuccess });
        }

        [HttpPut("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshJwtRequest registerJwtRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(errors => errors.Errors.Select(error => error.ErrorMessage)));

            var refreshTokenResponse = await jwt.RefreshToken(registerJwtRequest.Token, registerJwtRequest.RefreshToken);

            return refreshTokenResponse.IsSuccess ?
                            Ok(new JwtResponse
                            {
                                Token = refreshTokenResponse.Token,
                                RefreshToken = refreshTokenResponse.RefreshToken,
                                IsSuccess = refreshTokenResponse.IsSuccess
                            })
                            : BadRequest(new JwtResponse { Errors = refreshTokenResponse.Errors, IsSuccess = refreshTokenResponse.IsSuccess });
        }
    }
}
