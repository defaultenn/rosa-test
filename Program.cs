using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args) {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDBContext>(options =>
            options.UseSqlite("Data Source=app.db")
        );

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "AuthCookie";
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

        builder.Services.AddAuthorization();
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            dbContext.Database.EnsureCreated();
        }
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=User}/{action=Index}/{id?}"
        );
        app.MapRazorPages();
        app.Run();
    }
}
