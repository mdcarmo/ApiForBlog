using ApiForBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiForBlog.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiForBlogContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApiForBlogContext(DbContextOptions options) :
            base(options)
        { }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(p => p.Id);
            modelBuilder.Entity<Post>().HasKey(p => p.Id);
        }
    }
}
