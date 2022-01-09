using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.ResponseModels;

namespace TweetBook.Application.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Where(v => v.Value.Errors.Count > 0)

                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(msg => msg.ErrorMessage));

                var errModel = new CustomErrorModel();
                foreach (var error in errors)
                {
                    foreach (var subError in error.Value)
                    {
                        errModel.Errors.Add(new ErrorModel
                        {
                            Key = error.Key,
                            Value = subError
                        });
                    }
                }

                context.Result = new BadRequestObjectResult(errModel);
                return;
            }

            await next();


        }
    }
}
