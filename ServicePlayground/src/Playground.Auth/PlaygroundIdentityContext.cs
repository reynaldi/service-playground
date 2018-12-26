using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Playground.Auth.Domain;

namespace Playground.Auth
{
    public class PlaygroundIdentityContext : IdentityDbContext<AuthUser>
    {
        public PlaygroundIdentityContext(DbContextOptions<PlaygroundIdentityContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> AuthUsers { get; set; }
    }
}
