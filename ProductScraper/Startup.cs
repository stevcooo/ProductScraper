using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductScraper.Common.Naming;
using ProductScraper.Data;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Implementations;
using ProductScraper.Services.Interfaces;
using System;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;

namespace ProductScraper
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            Configuration = configuration;
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
#if AZURE || RELEASE
            .AddJsonFile($"appsettings.azure.json", optional: true)
#else
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
#endif
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })   
            //ElCamino configuration
            .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
            {
                IdentityConfiguration idconfig = new IdentityConfiguration();
                idconfig.TablePrefix = Configuration.GetSection("AzureTable:IdentityConfiguration:TablePrefix").Value;
                idconfig.StorageConnectionString = Configuration.GetSection("AzureTable:StorageConnectionString").Value;
                idconfig.LocationMode = Configuration.GetSection("AzureTable:IdentityConfiguration:LocationMode").Value;
                idconfig.IndexTableName = TableName.IdentityIndex; // default: AspNetIndex
                idconfig.RoleTableName = TableName.IdentityRoles;   // default: AspNetRoles
                idconfig.UserTableName = TableName.IdentityUsers;   // default: AspNetUsers
                return idconfig;
            }))
            .AddDefaultTokenProviders()
            .AddDefaultUI()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>(); //can remove after first run;
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddScoped<IAzureTableStorage<ProductInfo>>(factory =>
            {
                return new AzureTableStorage<ProductInfo>(
                    new AzureTableSettings(
                        storageConnectionString: Configuration.GetSection("AzureTable:StorageConnectionString").Value,
                        tableName: TableName.ProductInfo));
            });

            services.AddScoped<IProductInfoService, ProductInfoService>();

            services.AddScoped<IAzureTableStorage<ScrapeConfig>>(factory =>
            {
                return new AzureTableStorage<ScrapeConfig>(
                    new AzureTableSettings(
                        storageConnectionString: Configuration.GetSection("AzureTable:StorageConnectionString").Value,
                        tableName: TableName.ScrapeConfig));
            });
            services.AddScoped<IScrapeConfigService, ScrapeConfigService>();

            services.AddScoped<IAzureTableStorage<UserProfile>>(factory =>
            {
                return new AzureTableStorage<UserProfile>(
                    new AzureTableSettings(
                        storageConnectionString: Configuration.GetSection("AzureTable:StorageConnectionString").Value,
                        tableName: TableName.UserProfile));
            });
            services.AddScoped<IUserProfileService, UserProfileService>();
            
            services.AddScoped<IHttpHandlerService, HttpHandlerService>();
            services.AddScoped<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
