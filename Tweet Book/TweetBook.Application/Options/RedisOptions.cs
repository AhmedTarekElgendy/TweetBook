using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetBook.Application.Options
{
    public class RedisOptions
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
    }
}
