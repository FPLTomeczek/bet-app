using BetApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BetApp.Api.Data;

public class BetAppContext : DbContext
{
    public BetAppContext(DbContextOptions<BetAppContext> options) : base(options) { }

    public DbSet<SportCategory> SportCategories => Set<SportCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}