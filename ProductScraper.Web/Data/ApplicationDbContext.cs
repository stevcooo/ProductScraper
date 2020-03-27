﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductScraper.Models.EntityModels;
using ProductScraper.Services.Interfaces;

namespace ProductScraper.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
