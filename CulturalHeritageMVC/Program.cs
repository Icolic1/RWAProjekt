using Microsoft.EntityFrameworkCore;
using WebAPI.Data;

namespace CulturalHeritageMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add controllers with views
            builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();

            // Add DbContext
            builder.Services.AddDbContext<CulturalHeritageDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Session support
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout nakon 30 minuta
                options.Cookie.HttpOnly = true; // Smanjuje sigurnosne rizike
                options.Cookie.IsEssential = true; // omogucuje rad bez pristanka kolacica
            });

            // Add Authentication and Authorization
            builder.Services.AddAuthentication("CookieAuth")
     .AddCookie("CookieAuth", options =>
     {
         options.LoginPath = "/Auth/Login"; // Ruta za Login
         options.LogoutPath = "/Auth/Logout"; // Ruta za Logout (koristi POST)
         options.AccessDeniedPath = "/Auth/Login"; // Ruta za neautorizovan pristup
     });


            // Build the app
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session middleware
            app.UseSession();

            // Enable Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Configure routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}
