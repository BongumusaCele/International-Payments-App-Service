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
        public DbSet<BeneficiaryModel> Beneficiaries { get; set; }
        public DbSet<CustomerSessionModel> CustomerSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.ToTable("tblCustomer");
                entity.HasKey(e => e.customer_Id);

                entity.Property(e => e.first_Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.last_Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.id_Number)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.email_Address)
                    .HasMaxLength(150);

                entity.Property(e => e.account_Number)
                    .IsRequired();

                entity.Property(e => e.preferred_Currency)
                    .HasMaxLength(50);

                entity.Property(e => e.username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.password_Hash)
                    .IsRequired()
                    .HasColumnName("password_hash");

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnName("created_On")
                    .HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<CustomerSessionModel>(entity =>
            {
                entity.ToTable("tblCustomerSession");

                entity.HasKey(e => e.session_Id);

                entity.Property(e => e.login_Time)
                    .IsRequired();

                entity.Property(e => e.logout_Time);

                entity.Property(e => e.is_Active)
                    .IsRequired();

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.CustomerSessions)
                    .HasForeignKey(e => e.customer_Id);
            });

            modelBuilder.Entity<BeneficiaryModel>(entity =>
            {
                entity.ToTable("tblBeneficiary");

                entity.HasKey(e => e.beneficiary_Id);

                entity.Property(e => e.beneficiary_Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.bank_Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.account_Number)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.swift_Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.country)
                    .HasMaxLength(150);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Beneficiaries)
                    .HasForeignKey(e => e.customer_Id);
            });
        }
    }
}
