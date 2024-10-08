using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using F24_Assignment3_mwebster.Models;

namespace F24_Assignment3_mwebster.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<F24_Assignment3_mwebster.Models.Movie> Movie { get; set; } = default!;
        public DbSet<F24_Assignment3_mwebster.Models.Actor> Actor { get; set; } = default!;
        public DbSet<F24_Assignment3_mwebster.Models.ActorMovie> ActorMovie { get; set; } = default!;
    }
}
