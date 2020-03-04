using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public sealed class ProductContext : DbContext
    {
        public ProductContext()
        {
        }

        public ProductContext(DbContextOptions<ProductContext> opt) : base(opt)
        {
            if (Database.IsSqlServer())
            {
                Database.Migrate();
            }
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=db;Database=master;User=sa;Password=ADjh3487@^98;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .HasData(
                    new Product { Id = 1, Name = "Lavender heart", Price = 9.25f },
                    new Product { Id = 2, Name = "Personalised cufflinks", Price = 45f },
                    new Product { Id = 3, Name = "Kids T-shirt", Price = 19.95f }
                );
        }
    }
}
