using MB_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace MB_Project.Data;

public class MB_ProjectContext : IdentityDbContext<IdentityUser>
{
    public MB_ProjectContext(DbContextOptions<MB_ProjectContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users {  get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PostImage> PostsImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PostFeature> PostFeatures { get; set; }
    public DbSet<Review> Reviews { get; set; }
    //public DbSet<OrderrrrStatus> OrderStatuses { get; set; }
    public DbSet<PostCategory> PostCategories { get; set; }
    public DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);



        builder.Entity<Post>(e=>e.Property(e=>e.Id).ValueGeneratedOnAdd());
        builder.Entity<Category>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<Review>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<Order>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<PostFeature>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<PostImage>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<UserRefreshTokens>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        builder.Entity<Post>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());


        builder.Entity<IdentityUser>(b =>
        {
            b.ToTable("Users");
        });

        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("UserTokens");
        });

        builder.Entity<IdentityRole>(b =>
        {
            b.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("UserRoles");
        });


        builder.Entity<Post>()
        .HasMany(p => p.SecondaryImages)
        .WithOne(pi => pi.Post)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    Name = "CLIENT",
                    NormalizedName = "CLIENT".ToUpper()
                },
                new IdentityRole
                {
                    Id = "ae2626ab-cea5-458f-82f5-2dbad5009e29",
                    Name = "ADMIN",
                    NormalizedName = "ADMIN".ToUpper()
                },
                new IdentityRole
                {
                    Id = "9b3e174e-10e6-446f-86af-483d56fd7210",
                    Name = "SELLER",
                    NormalizedName = "SELLER".ToUpper()
                }
            );
        /*
        builder.Entity<OrderrrrStatus>()
            .HasData(  
                new OrderrrrStatus
                {
                    Id = 1,
                    Name = "PENDING"
                },
                new OrderrrrStatus
                {
                    Id = 2,
                    Name = "ACCEPTED"
                },
                new OrderrrrStatus
                {
                    Id = 3,
                    Name = "APPROVED"
                },
                new OrderrrrStatus
                {
                    Id = 4,
                    Name = "COMPLETED"
                },
                new OrderrrrStatus
                {
                    Id = 5,
                    Name = "DELIVERED"
                },
                new OrderrrrStatus
                {
                    Id = 6,
                    Name = "CANCELLED"
                }
            );
        */

        builder.Entity<Post>()
        .HasMany(e => e.Categories)
        .WithMany(e => e.Posts)
        .UsingEntity<PostCategory>();


        builder.Entity<Review>()
            .Property(r => r.Rating)
            .HasColumnName("Rating")
            .HasColumnType("int")
            .HasDefaultValue(null); // Optional if you want to set a default value (null in this case)

        builder.Entity<Review>()
            .Property(r => r.Rating)
            .HasAnnotation("Range", new int?[] { 1, 2, 3, 4, 5 }); // Ensures the rating is within the specified range
    }
}
