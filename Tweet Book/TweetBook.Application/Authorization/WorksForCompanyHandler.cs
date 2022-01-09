using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Authorization
{
    public class WorksForCompanyHandler : AuthorizationHandler<WorkForCompanyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkForCompanyRequirement requirement)
        {
            var email = context.User?.FindFirst(ClaimTypes.Email).Value;

            if (!string.IsNullOrEmpty(email))
            {
                if (email.EndsWith(requirement.comapnyName))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
