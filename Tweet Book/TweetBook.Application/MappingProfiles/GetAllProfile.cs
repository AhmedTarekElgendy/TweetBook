using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.Models.RequestModels;
using TweetBook.Contracts.Models.RequestModels.Queries;

namespace TweetBook.Application.MappingProfiles
{
    public class GetAllProfile : Profile
    {
        public GetAllProfile()
        {
            CreateMap<GetAllPostsQuery, GetAllPostsRequst>();
        }
    }
}
