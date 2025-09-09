using Microsoft.EntityFrameworkCore;
using AuthModule.Data;
using AuthModule.Repositories;
using AuthModule.Services;
using AuthModule.Middleware;

namespace AuthModule
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure pipeline
            ConfigurePipeline(app);

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllersWithViews();

            // Add Entity Framework
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Add Repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Add Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();

            // Add JWT Authentication
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!")),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"] ?? "AuthModule",
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtSettings:Audience"] ?? "AuthModuleUsers",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void ConfigurePipeline(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use custom CORS middleware
            app.UseMiddleware<CorsMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            // Map Account routes first (more specific)
            app.MapControllerRoute(
                name: "account",
                pattern: "Account/{action=Login}",
                defaults: new { controller = "Account" });

            // Map API routes
            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action}/{id?}");

            // Map default MVC routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }
    }
}
