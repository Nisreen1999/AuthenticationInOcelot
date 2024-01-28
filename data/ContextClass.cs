using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthenticationAutherization.data
{
    public class ContextClass:IdentityDbContext
    {
        public DbSet<Course> course { get; set; }
        public DbSet<student> student { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=;Initial Catalog=Auth;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}