using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.Entities;

namespace MyDotNet9Api;

public class ApplicationDbContext: DbContext
{
     public ApplicationDbContext(DbContextOptions options) : base(options)
     {
          
     }
     public DbSet<Genre> Genres { get; set; }
     public DbSet<Actor> Actors { get; set; }
}