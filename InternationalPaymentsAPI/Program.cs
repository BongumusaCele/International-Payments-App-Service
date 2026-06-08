using Scalar.AspNetCore;
using InternationalPaymentsAPI.Data;
using InternationalPaymentsAPI.Auth;
using InternationalPaymentsAPI.Helpers;
using InternationalPaymentsAPI.Models;
using InternationalPaymentsAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
bool useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
bool applyDatabaseMigrations = builder.Configuration.GetValue<bool>("ApplyDatabaseMigrations");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:4173",
                "http://127.0.0.1:4173",
                "https://international-payments-app-react-bmfcfsatc2fhb3aa.southafricanorth-01.azurewebsites.net",
                "https://internationa-payments-app-react-bmfcfsatc2fhb3aa.southafricanorth-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("InternationalPaymentsLocalDev");
        return;
    }

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ISwiftService, SwiftService>();

builder.Services
    .AddAuthentication(SessionAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
        SessionAuthenticationHandler.SchemeName,
        options => { })
    .AddScheme<AuthenticationSchemeOptions, EmployeeAuthenticationHandler>(
        EmployeeAuthenticationHandler.SchemeName,
        options => { });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("Auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (useInMemoryDatabase)
{
    SeedLocalDevelopmentData(app);
}
else if (applyDatabaseMigrations)
{
    ApplyDatabaseMigrations(app);
}

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseHsts();

app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    context.Response.Headers.TryAdd("Content-Security-Policy", "frame-ancestors 'none'");
    await next();
});

app.UseCors("AllowReactApp");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "International Payments API is running");

app.Run();

static void SeedLocalDevelopmentData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureCreated();

    if (!context.Currencies.Any())
    {
        context.Currencies.AddRange(
            new CurrencyModel
            {
                currency_Id = 1,
                currency_Name = "South African Rand",
                currency_Code = "ZAR",
                exchange_Rate = 1m
            },
            new CurrencyModel
            {
                currency_Id = 2,
                currency_Name = "Euro",
                currency_Code = "EUR",
                exchange_Rate = 20m
            });
    }

    if (!context.Customers.Any())
    {
        context.Customers.Add(new CustomerModel
        {
            customer_Id = 1,
            first_Name = "Demo",
            last_Name = "Customer",
            id_Number = "9001015009087",
            email_Address = "demo.customer@example.com",
            account_Number = 123456789,
            currency_Id = 1,
            username = "demo_customer",
            password_Hash = PasswordHelper.HashPassword("Password123!"),
            CreatedOn = DateTime.UtcNow
        });
    }

    if (!context.Employees.Any())
    {
        context.Employees.AddRange(
            new EmployeeModel
            {
                employee_Id = 1,
                username = "employee1",
                password_Hash = PasswordHelper.HashPassword("Password123!"),
                full_Name = "Andile Bolo",
                is_Active = true
            },
            new EmployeeModel
            {
                employee_Id = 2,
                username = "employee2",
                password_Hash = PasswordHelper.HashPassword("Password123!"),
                full_Name = "Themba Msomi",
                is_Active = true
            });
    }

    if (!context.Beneficiaries.Any())
    {
        context.Beneficiaries.Add(new BeneficiaryModel
        {
            beneficiary_Id = 1,
            customer_Id = 1,
            currency_Id = 2,
            beneficiary_Name = "Anna Schmidt",
            bank_Name = "Deutsche Bank",
            account_Number = "DE89370400440532013000",
            swift_Code = "DEUTDEFF",
            country = "Germany"
        });
    }

    if (!context.Payments.Any())
    {
        context.Payments.Add(new PaymentModel
        {
            payment_Id = 1,
            customer_Id = 1,
            beneficiary_Id = 1,
            from_Currency_Id = 1,
            to_Currency_Id = 2,
            amount = 15000m,
            exchange_Rate_Used = 20m,
            converted_Amount = 750m,
            payment_Reference = "PAY-DEMO-001",
            payment_Reason = "Supplier payment",
            payment_Provider = "SWIFT",
            swift_Code = "DEUTDEFF",
            status = "Pending",
            created_On = DateTime.UtcNow
        });
    }

    context.SaveChanges();
}

static void ApplyDatabaseMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();
    EnsureEmployeeDemoAccounts(context);
}

static void EnsureEmployeeDemoAccounts(ApplicationDbContext context)
{
    UpsertEmployee(context, "employee1", "Andile Bolo");
    UpsertEmployee(context, "employee2", "Themba Msomi");

    context.SaveChanges();
}

static void UpsertEmployee(ApplicationDbContext context, string username, string fullName)
{
    var employee = context.Employees.FirstOrDefault(item => item.username == username);
    string passwordHash = PasswordHelper.HashPassword("Password123!");

    if (employee == null)
    {
        context.Employees.Add(new EmployeeModel
        {
            username = username,
            password_Hash = passwordHash,
            full_Name = fullName,
            is_Active = true
        });
        return;
    }

    employee.full_Name = fullName;
    employee.password_Hash = passwordHash;
    employee.is_Active = true;
}
