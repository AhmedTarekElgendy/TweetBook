using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Application.Data;
using TweetBook.Application.Localization.Resources;
using TweetBook.Application.Options;
using TweetBook.Domain.Models;

namespace TweetBook.Application.Services
{
    public class JwtService : IJwt
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JWTSettings jWTSettings;
        private readonly TokenValidationParameters validationParameters;
        private readonly TweetBookDbContext dbContext;
        private readonly RoleManager<IdentityRole> roleManager;
        private SymmetricSecurityKey key;

        public JwtService(UserManager<IdentityUser> _userManager, JWTSettings _jWTSettings,
            TokenValidationParameters _validationParameters, TweetBookDbContext _dbContext,RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            this.jWTSettings = _jWTSettings;
            validationParameters = _validationParameters;
            dbContext = _dbContext;
            this.roleManager = _roleManager;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSettings.JWTSecret));
        }

        public async Task<JwtResponse> Login(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return new JwtResponse
                {
                    IsSuccess = false,
                    Errors = new List<Error> {
                        new Error { Description =Resources.UserNotExisted }
                    }
                };

            var isSameCredentials = await userManager.CheckPasswordAsync(user, password);

            if (!isSameCredentials)
                return new JwtResponse
                {
                    IsSuccess = false,
                    Errors = new List<Error>
                    {
                        new Error{Description = Resources.WrongCredentials}
                    }
                };


            return await GenerateTokenAsync(user);
        }

        public async Task<JwtResponse> RefreshToken(string token, string refreshToken)
        {
            var claimsPrincipal = GetClaimsPrincipal(token);
            if (claimsPrincipal == null)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.InvalidToken  }
                    }
                };
            }
            var expiryDateUnix = long.Parse(claimsPrincipal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateUTC > DateTime.UtcNow)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.ValidToken  }
                    }
                };
            }
            var jti = claimsPrincipal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = dbContext.RefreshTokens.SingleOrDefault(r => r.Token == refreshToken);

            if (storedRefreshToken == null || storedRefreshToken.JwtId != jti)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.InvalidRefreshToken  }
                    }
                };
            }

            if (storedRefreshToken.ExpirationDate < DateTime.UtcNow)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.ExpiredRefreshToken  }
                    }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.UsedRefreshToken  }
                    }
                };
            }

            if (storedRefreshToken.Used)
            {
                return new JwtResponse
                {
                    Errors = new List<Error>
                    {
                        new Error{ Description = Resources.InvalidatedRefreshToken  }
                    }
                };
            }
            storedRefreshToken.Used = true;
            dbContext.RefreshTokens.Update(storedRefreshToken);
            await dbContext.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(claimsPrincipal.Claims.Single(c => c.Type == "id").Value);
            return await GenerateTokenAsync(user);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            try
            {
                validationParameters.ValidateLifetime = false;
                var principal = jwtHandler.ValidateToken(token, validationParameters, out var securityToken);
                if (!IsTokenHasrightAlgorithm(securityToken as JwtSecurityToken))
                {
                    return null;
                }
                return principal;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        private bool IsTokenHasrightAlgorithm(JwtSecurityToken Token)
        {

            return Token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<JwtResponse> Register(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
                return new JwtResponse
                {
                    IsSuccess = false,
                    Errors = new List<Error> { new Error { Description = Resources.UserExisted } }
                };

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };
            var isCreated = await userManager.CreateAsync(newUser, password);

            if (!isCreated.Succeeded)
                return new JwtResponse
                {
                    IsSuccess = false,
                    Errors = isCreated.Errors.Select(error => new Error { Description = error.Description }).ToList()
                };

            //Add Role to the user
            //var roleResponse= await roleManager.RoleExistsAsync("PostAdmin");

            //if(!roleResponse)
            //    await roleManager.CreateAsync(new IdentityRole { Name = "PostAdmin" });

            //await userManager.AddToRoleAsync(newUser, "PostAdmin");


            //Add claim to the user
            //await userManager.AddClaimAsync(newUser, new Claim("delete", "true"));

            return await GenerateTokenAsync(newUser);
        }

        private async Task<JwtResponse> GenerateTokenAsync(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id",user.Id)
            };
            //To Add Claim to the JWT 
            //var userClaims= await userManager.GetClaimsAsync(user);
            //claims.AddRange(userClaims);

            //To Add the Role and it's claims in the jwt
            //var userRoles = await userManager.GetRolesAsync(user);
            //foreach(var role in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //    var roleStored =await roleManager.FindByNameAsync(role);

            //    if (role == null) continue;
            //    var roleClaims = await roleManager.GetClaimsAsync(roleStored);

            //    foreach (var roleClaim in roleClaims)
            //    {
            //        if (claims.Contains(roleClaim))
            //            continue;

            //        claims.Add(roleClaim);
            //    }
            //}
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenhandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.Add(jWTSettings.TokenLifeTime),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
            };
            var scuritytoken = tokenhandler.CreateToken(descriptor);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                JwtId = scuritytoken.Id
            };

            var storedRefreshToken = await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();

            return new JwtResponse
            {
                IsSuccess = true,
                Token = tokenhandler.WriteToken(scuritytoken),
                RefreshToken = storedRefreshToken.Entity.Token
            };
        }
    }
}
