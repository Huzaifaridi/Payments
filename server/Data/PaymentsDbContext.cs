using Microsoft.EntityFrameworkCore;
using PaymentsApi.Models;

namespace PaymentsApi.Data
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(eb =>
            {
                eb.HasKey(p => p.Id);
                eb.HasIndex(p => p.ClientRequestId).IsUnique();
                eb.HasIndex(p => p.Reference).IsUnique();
                eb.Property(p => p.Currency).HasMaxLength(3);
            });
        }
    }
}
