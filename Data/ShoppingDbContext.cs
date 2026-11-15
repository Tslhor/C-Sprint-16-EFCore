using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Data;

public sealed class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supermarket> Supermarkets => Set<Supermarket>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetails> OrderDetails => Set<OrderDetails>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(customer => customer.FirstName).HasMaxLength(30).IsRequired();
            entity.Property(customer => customer.LastName).HasMaxLength(30).IsRequired();
            entity.Property(customer => customer.Address).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Supermarket>(entity =>
        {
            entity.ToTable("SuperMarkets");
            entity.Property(supermarket => supermarket.Name).HasMaxLength(30).IsRequired();
            entity.Property(supermarket => supermarket.Address).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name).HasMaxLength(50).IsRequired();
            entity.Property(product => product.Price).HasPrecision(10, 2).IsRequired();
            entity.ToTable(table => table.HasCheckConstraint("CK_Products_Price_Positive", "Price > 0"));
        });

        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.Property(details => details.Quantity).HasPrecision(10, 3).IsRequired();
            entity.HasIndex(details => new { details.OrderId, details.ProductId }).IsUnique();
            entity.ToTable(table => table.HasCheckConstraint("CK_OrderDetails_Quantity_Positive", "Quantity > 0"));
        });

        modelBuilder.Entity<Order>()
            .HasOne(order => order.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(order => order.Supermarket)
            .WithMany(supermarket => supermarket.Orders)
            .HasForeignKey(order => order.SupermarketId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetails>()
            .HasOne(details => details.Order)
            .WithMany(order => order.OrderDetails)
            .HasForeignKey(details => details.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetails>()
            .HasOne(details => details.Product)
            .WithMany(product => product.OrderDetails)
            .HasForeignKey(details => details.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

