using budget_tracker;
using budget_tracker.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Prema.ShuleOne.Web.Server.Database;
using Serilog;
using System.Configuration;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:4200", "https://lifeway.prema.co.ke") // Update this with your Angular app's URL
            .AllowAnyHeader()
            .AllowAnyMethod());
});
// Add services to the container.

// Use Serilog - from configuration
builder.Host.UseSerilog(
    (hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// Setup database connection    
var connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");
var shuleOneDbConnectionString = builder.Configuration.GetConnectionString("ShuleOneDbConnectionString");
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddSingleton<Logging>();
builder.Services.AddSingleton<BulkSms>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<MobileSasaBulkSms>();
builder.Services.AddSingleton<PollyPolicy>();
builder.Services.AddHostedService<StartupService>();
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

builder.Services.AddDbContext<ShuleOneDatabaseContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(shuleOneDbConnectionString, serverVersion,
            options => options.EnableRetryOnFailure()) // Enable transient error resiliency
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddDbContext<BudgetTrackerDbContext>(options => options.UseMySQL(connectionString));
builder.Services.AddTransient<ITransactionsService, TransactionsService>();
builder.Services.AddTransient<IFeePaymentService, FeePaymentService>();
builder.Services.AddSingleton<RevenueApiClient>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
//    options.HttpsPort = 5000;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

//app.Urls.Add("http://0.0.0.0:5000");
//app.Urls.Add("https://0.0.0.0:5001");

Console.WriteLine("All resources loaded, application running...");

//Logging logging = new Logging();
//logging.WriteToLog("All resources loaded, application running...", "Information");

app.Run();
