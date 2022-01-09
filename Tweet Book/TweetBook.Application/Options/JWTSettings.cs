using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Application.Options
{
    public class JWTSettings
    {
        public string JWTSecret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}
