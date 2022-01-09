using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Contracts.Models.ResponseModels
{
    public class CustomErrorModel
    {
        public CustomErrorModel()
        {
            Errors = new List<ErrorModel>();
        }
        public List<ErrorModel> Errors { get; set; }
    }
}
