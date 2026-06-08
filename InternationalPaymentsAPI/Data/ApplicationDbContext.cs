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
        public DbSet<CurrencyModel> Currencies { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<AuditLogModel> AuditLogs { get; set; }
        public DbSet<MfaChallengeModel> MfaChallenges { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<EmployeeSessionModel> EmployeeSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerModel>(entity =>
            {
                entity.ToTable("tblCustomer");
                entity.HasKey(e => e.customer_Id);

                entity.Property(e => e.first_Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.last_Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.id_Number).IsRequired().HasMaxLength(13);
                entity.Property(e => e.email_Address).HasMaxLength(150);
                entity.Property(e => e.account_Number).IsRequired();
                entity.Property(e => e.username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.password_Hash).IsRequired().HasColumnName("password_hash");
                entity.Property(e => e.CreatedOn).IsRequired().HasColumnName("created_On").HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Currency)
                    .WithMany()
                    .HasForeignKey(e => e.currency_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerSessionModel>(entity =>
            {
                entity.ToTable("tblCustomerSession");
                entity.HasKey(e => e.session_Id);
                entity.Property(e => e.login_Time).IsRequired();
                entity.Property(e => e.is_Active).IsRequired();
                entity.Property(e => e.session_Token_Hash).IsRequired().HasMaxLength(128);
                entity.Property(e => e.expires_On).IsRequired();
                entity.HasIndex(e => e.session_Token_Hash).IsUnique();

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.CustomerSessions)
                    .HasForeignKey(e => e.customer_Id);
            });

            modelBuilder.Entity<MfaChallengeModel>(entity =>
            {
                entity.ToTable("tblMfaChallenge");
                entity.HasKey(e => e.mfa_Challenge_Id);
                entity.Property(e => e.otp_Code_Hash).IsRequired().HasMaxLength(128);
                entity.Property(e => e.created_On).IsRequired();
                entity.Property(e => e.expires_On).IsRequired();
                entity.Property(e => e.attempt_Count).IsRequired();
                entity.HasIndex(e => e.customer_Id);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.MfaChallenges)
                    .HasForeignKey(e => e.customer_Id);
            });

            modelBuilder.Entity<BeneficiaryModel>(entity =>
            {
                entity.ToTable("tblBeneficiary");
                entity.HasKey(e => e.beneficiary_Id);
                entity.Property(e => e.beneficiary_Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.bank_Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.account_Number).IsRequired().HasMaxLength(20);
                entity.Property(e => e.swift_Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.country).HasMaxLength(150);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Beneficiaries)
                    .HasForeignKey(e => e.customer_Id);

                entity.HasOne(e => e.Currency)
                    .WithMany()
                    .HasForeignKey(e => e.currency_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CurrencyModel>(entity =>
            {
                entity.ToTable("tblCurrency");
                entity.HasKey(e => e.currency_Id);
                entity.Property(e => e.currency_Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.currency_Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.exchange_Rate).IsRequired().HasColumnType("decimal(18,4)");
                entity.HasIndex(e => e.currency_Code).IsUnique();
            });

            modelBuilder.Entity<EmployeeModel>(entity =>
            {
                entity.ToTable("tblEmployee");
                entity.HasKey(e => e.employee_Id);
                entity.Property(e => e.username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.password_Hash).IsRequired();
                entity.Property(e => e.full_Name).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.username).IsUnique();
            });

            modelBuilder.Entity<EmployeeSessionModel>(entity =>
            {
                entity.ToTable("tblEmployeeSession");
                entity.HasKey(e => e.employee_Session_Id);
                entity.Property(e => e.session_Token_Hash).IsRequired().HasMaxLength(128);
                entity.HasIndex(e => e.session_Token_Hash).IsUnique();

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.employee_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentModel>(entity =>
            {
                entity.ToTable("tblPayment");
                entity.HasKey(e => e.payment_Id);
                entity.Property(e => e.amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.exchange_Rate_Used).IsRequired().HasColumnType("decimal(18,4)");
                entity.Property(e => e.converted_Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.payment_Provider).IsRequired().HasMaxLength(50).HasDefaultValue("SWIFT");
                entity.Property(e => e.swift_Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.status).IsRequired().HasMaxLength(30).HasDefaultValue("Pending");
                entity.Property(e => e.created_On).IsRequired().HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.customer_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Beneficiary)
                    .WithMany()
                    .HasForeignKey(e => e.beneficiary_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FromCurrency)
                    .WithMany()
                    .HasForeignKey(e => e.from_Currency_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ToCurrency)
                    .WithMany()
                    .HasForeignKey(e => e.to_Currency_Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.VerifiedByEmployee)
                    .WithMany()
                    .HasForeignKey(e => e.verified_By_Employee_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuditLogModel>(entity =>
            {
                entity.ToTable("tblAuditLog");
                entity.HasKey(e => e.audit_Id);
                entity.Property(e => e.action_Type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.table_Name).HasMaxLength(100);
                entity.Property(e => e.details).HasMaxLength(255);
                entity.Property(e => e.created_On).IsRequired().HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.customer_Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
