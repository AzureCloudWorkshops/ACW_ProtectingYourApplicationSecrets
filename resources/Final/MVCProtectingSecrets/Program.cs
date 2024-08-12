using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCProtectingSecrets.Data;
using MVCProtectingSecrets.Initializers;

namespace MVCProtectingSecrets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpClient();

            /********************************************************************************************************
            *  * Add Azure App Configuration
            *  * use the connection string to connect to Azure App Configuration
            *  * requires all app service and identity information to be authorized on app config [app data reader role, etc]
            *  * for key vault => requires the user, app config, and app service to be authorized on the key vault
            ********************************************************************************************************/
            var appConfigConnection = builder.Configuration.GetConnectionString("AzureAppConfigConnection");

            //WITHOUT KEY VAULT:
            //builder.Configuration.AddAzureAppConfiguration(appConfigConnection);

            //WITH KEY VAULT:
            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(appConfigConnection);
                    options.ConfigureKeyVault(options =>
                    {
                        options.SetCredential(new DefaultAzureCredential());
                    });
                });
            });

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //optional, turn on to auto-migrate on startup [Warning: prevents any possibility of migration rollback]
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connectionString).Options;
            using (var context = new ApplicationDbContext(contextOptions))
            {
                context.Database.Migrate();
            }

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddSingleton<ITelemetryInitializer, LogSanitizerInsightsInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
