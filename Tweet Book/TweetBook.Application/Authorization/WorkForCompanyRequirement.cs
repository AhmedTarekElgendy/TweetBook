using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Authorization
{
    public class WorkForCompanyRequirement : IAuthorizationRequirement
    {
        public readonly string comapnyName;

        public WorkForCompanyRequirement(string _comapnyName)
        {
            comapnyName = _comapnyName;
        }
    }
}
