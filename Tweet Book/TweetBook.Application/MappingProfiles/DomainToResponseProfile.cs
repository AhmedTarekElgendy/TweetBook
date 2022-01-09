using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Application.Contracts.Models.ResponseModels;
using TweetBook.Domain.Models;

namespace TweetBook.Application.MappingProfiles
{
    public class DomainToResponseProfile :Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Posts, PostResponse>();
        }
    }
}
