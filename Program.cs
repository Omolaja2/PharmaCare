using Microsoft.EntityFrameworkCore;
using PharmacyApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using PharmacyApp.Models;
using PharmacyApp.Services;
using Serilog;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
// the logger for seriilogg
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration) 
    // .WriteTo.Console() 
    .CreateLogger();

try
{
    Log.Information("starting the server");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();


    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<PharmacyDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 36))
        )
    );

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/Login";
        });

    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddTransient<EmailService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
