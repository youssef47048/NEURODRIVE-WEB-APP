using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NueroDrive.Data;
using NueroDrive.Models;
using NueroDrive.Services;
using System.IO;

namespace NueroDrive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            
            // Configure data protection to persist keys
            var keyPath = builder.Configuration["DataProtection:KeyPath"] ?? "DataProtectionKeys";
            var keysDirectory = Path.Combine(builder.Environment.ContentRootPath, keyPath);
            Directory.CreateDirectory(keysDirectory); // Ensure directory exists
            
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysDirectory))
                .SetApplicationName("NueroDrive");
                
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add database context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add ASP.NET Core Identity with authentication configuration
            builder.Services.AddIdentity<User, IdentityRole>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity Cookie options
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // Register application services
            builder.Services.AddScoped<FaceRecognitionService>();
            builder.Services.AddScoped<EmailService>();

            var app = builder.Build();

            // DEBUG: Log configuration values at startup
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application Environment: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation("FaceRecognitionAPI:Url = {ApiUrl}", app.Configuration["FaceRecognitionAPI:Url"] ?? "Not configured");
            logger.LogInformation("ContentRootPath: {ContentRootPath}", app.Environment.ContentRootPath);

            // Configure the HTTP request pipeline
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
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
