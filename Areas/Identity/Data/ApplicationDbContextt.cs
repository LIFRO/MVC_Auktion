using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MVC_Auktioner.Areas.Identity.Data;

public class ApplicationDbContextt : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContextt(DbContextOptions<ApplicationDbContextt> options)
        : base(options)
    {
    }

    public DbSet<auction> Auctions {get; set; } = default;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
