using InternationalPaymentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InternationalPaymentsAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CustomerModel> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerModel>()
                .HasIndex(c => c.email_Address)
                .IsUnique()
                .HasFilter("[email_Address] IS NOT NULL");

            modelBuilder.Entity<CustomerModel>()
                .HasIndex(c => c.username)
                .IsUnique();

            modelBuilder.Entity<CustomerModel>()
                .HasIndex(c => c.id_Number)
                .IsUnique();

            modelBuilder.Entity<CustomerModel>()
                .HasIndex(c => c.account_Number)
                .IsUnique();
        }
    }
}
