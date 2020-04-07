using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductScraper.Data;
using ProductScraper.Models.EntityModels;
using ProductScraper.Models.ViewModels;
using ProductScraper.Services.Implementations.AzureTableStorage;
using ProductScraper.Services.Interfaces;
using System;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;

namespace samplemvccore4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
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
                idconfig.StorageConnectionString = Configuration.GetSection("AzureTable:IdentityConfiguration:StorageConnectionString").Value;
                idconfig.LocationMode = Configuration.GetSection("AzureTable:IdentityConfiguration:LocationMode").Value;
                idconfig.IndexTableName = Configuration.GetSection("AzureTable:IdentityConfiguration:IndexTableName").Value; // default: AspNetIndex
                idconfig.RoleTableName = Configuration.GetSection("AzureTable:IdentityConfiguration:RoleTableName").Value;   // default: AspNetRoles
                idconfig.UserTableName = Configuration.GetSection("AzureTable:IdentityConfiguration:UserTableName").Value;   // default: AspNetUsers
                return idconfig;
            }))
            .AddDefaultTokenProviders()
            .AddDefaultUI()
            .CreateAzureTablesIfNotExists<ApplicationDbContext>(); //can remove after first run;
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<IAzureTableStorage<ProductInfo>>(factory =>
            {
                return new AzureTableStorage<ProductInfo>(
                    new AzureTableSettings(
                        storageConnectionString: Configuration.GetSection("AzureTable:StorageConnectionString").Value,
                        tableName: Configuration.GetSection("AzureTable:ProductInfoTableName").Value));
            });

            services.AddScoped<IProductInfoService, ProductInfoService>();
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
