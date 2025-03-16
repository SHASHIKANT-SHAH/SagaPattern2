using Microsoft.EntityFrameworkCore;
using SagaPattern2.Models;

namespace SagaPattern2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<OrderSaga> OrderSagas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<OrderSaga>()
            //    .HasNoKey(); // Mark it as a keyless entity

            modelBuilder.Entity<OrderSaga>()
                .HasKey(o=> o.Id); // Mark it as a keyless entity

            modelBuilder.Entity<OrderSaga>()
                .HasIndex(o => o.OrderId)
                .IsUnique(); // Mark the OrderId as uniquemodelBuilder.Entity<OrderSaga>()

            base.OnModelCreating(modelBuilder);
        }

    }
}
