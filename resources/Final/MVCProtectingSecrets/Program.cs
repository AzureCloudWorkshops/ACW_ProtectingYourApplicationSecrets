using Azure.Core;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCProtectingSecrets.Data;
using MVCProtectingSecrets.Initializers;

namespace MVCProtectingSecrets;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var settings = config.Build();

            ////by default, use the default credential:
            TokenCredential credential = new DefaultAzureCredential();
            var env = settings["Application:Environment"];

            //without keyvault:
            if (env == null || !env.Trim().Equals("develop", StringComparison.OrdinalIgnoreCase))
            {
                //if at azure, use ManagedIdentityCredential and AppConfiguration URI endpoint
                credential = new ManagedIdentityCredential();

                config.AddAzureAppConfiguration(options =>
                    options.Connect(new Uri(settings["AzureAppConfigConnection"]), credential)
                        .ConfigureKeyVault(kv => { kv.SetCredential(credential); }));
            }
            else
            {
                //from local app, use connection string, not the URI
                config.AddAzureAppConfiguration(options =>
                        options.Connect(settings["AzureAppConfigConnection"])
                                .ConfigureKeyVault(kv => { kv.SetCredential(credential); }));
            }
        });

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}