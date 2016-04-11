namespace Financial_Management_Application.Identity
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// A basic implementation for an application database context compatible with ASP.NET Identity 2 using
    /// <see cref="long"/> as the key-column-type for all entities.
    /// </summary>
    /// <remarks>
    /// This type depends on some other types out of this assembly.
    /// </remarks>
    public class ApplicationDbContext : IdentityDbContext<AccountUser, AccountRole, long, AccountLogin, AccountUserRole, AccountClaim>
    {
        public ApplicationDbContext()
            : base("FM_Datastore_Entities")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Entities to their tables.
            modelBuilder.Entity<AccountUser>().ToTable("User");
            modelBuilder.Entity<AccountRole>().ToTable("Role");
            modelBuilder.Entity<AccountClaim>().ToTable("UserClaim");
            modelBuilder.Entity<AccountLogin>().ToTable("UserLogin");
            modelBuilder.Entity<AccountUserRole>().ToTable("UserRole");

            // Set AutoIncrement-Properties
            modelBuilder.Entity<AccountUser>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<AccountClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<AccountRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Override some column mappings that do not match our default
            modelBuilder.Entity<AccountUser>().Property(r => r.UserName).HasColumnName("Login");
            modelBuilder.Entity<AccountUser>().Property(r => r.PasswordHash).HasColumnName("Password");
        }
    }
}