using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TweetBook.Domain.Models;

namespace TweetBook.Application.Data
{
    public class TweetBookDbContext : IdentityDbContext
    {
        public TweetBookDbContext(DbContextOptions<TweetBookDbContext> options)
            : base(options)
        {
        }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
