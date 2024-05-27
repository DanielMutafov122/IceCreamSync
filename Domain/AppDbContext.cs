using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class AppDbContext : DbContext
{
    public DbSet<Workflow> Workflows { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public AppDbContext()
    {
    }
}
