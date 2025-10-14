using Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Persistence
{
    public class PaymentContext(DbContextOptions<PaymentContext> opts) : DbContext(opts)
    {
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.Token).IsUnique();
                b.Property(x => x.TerminalNo).IsRequired();
                b.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
                b.Property(x => x.RedirectUrl).IsRequired();
                b.Property(x => x.ReservationNumber).IsRequired();
                b.Property(x => x.PhoneNumber).IsRequired();
                b.Property(x => x.Token).IsRequired();
                b.Property(x => x.Status).HasConversion<int>();
                b.Property(x => x.CreatedAt).IsRequired();
                b.Property(x => x.UpdatedAt).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}