using budget_tracker;
using budget_tracker.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Configuration;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Use Serilog - from configuration
builder.Host.UseSerilog(
    (hostingContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// Setup database connection    
var connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));
builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddSingleton<Logging>();
builder.Services.AddSingleton<BulkSms>();
builder.Services.AddSingleton<MobileSasaBulkSms>();
builder.Services.AddSingleton<PollyPolicy>();
builder.Services.AddHostedService<StartupService>();
builder.Services.AddDbContextPool<BudgetTrackerDbContext>(options => options.UseMySQL(connectionString));
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IFeePaymentService, FeePaymentService>();
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

app.UseAuthorization();

app.MapControllers();

//app.Urls.Add("http://0.0.0.0:5000");
//app.Urls.Add("https://0.0.0.0:5001");

Console.WriteLine("All resources loaded, application running...");

//Logging logging = new Logging();
//logging.WriteToLog("All resources loaded, application running...", "Information");

app.Run();
