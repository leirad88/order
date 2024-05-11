using System;
using acme.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace acme.Data
{
    public class AppDbContext : DbContext, IDataProtectionKeyContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<acme.Models.Order> Orders { get; set; } = default!;
        public DbSet<acme.Models.Product> Product { get; set; } = default!;
        public DbSet<acme.Models.ImportedOrder> ImportedOrder { get; set; } = default!;
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}

