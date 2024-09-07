using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ehrlich.Pizza.API.Models;

public partial class PizzaPlaceDbContext : DbContext
{
    public PizzaPlaceDbContext()
    {
    }

    public PizzaPlaceDbContext(DbContextOptions<PizzaPlaceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Pizza> Pizzas { get; set; }

    public virtual DbSet<PizzaType> PizzaTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-1URGSGQ7;Initial Catalog=PizzaPlaceDb;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Time)
                .HasPrecision(0)
                .HasColumnName("time");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailsId);

            entity.ToTable("order_details");

            entity.Property(e => e.OrderDetailsId)
                .ValueGeneratedNever()
                .HasColumnName("order_details_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PizzaId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pizza_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_order_details_orders");

            entity.HasOne(d => d.Pizza).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.PizzaId)
                .HasConstraintName("FK_order_details_pizzas");
        });

        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.ToTable("pizzas");

            entity.Property(e => e.PizzaId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pizza_id");
            entity.Property(e => e.PizzaTypeId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pizza_type_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("size");

            entity.HasOne(d => d.PizzaType).WithMany(p => p.Pizzas)
                .HasForeignKey(d => d.PizzaTypeId)
                .HasConstraintName("FK_pizzas_pizza_types");
        });

        modelBuilder.Entity<PizzaType>(entity =>
        {
            entity.ToTable("pizza_types");

            entity.Property(e => e.PizzaTypeId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pizza_type_id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.Ingredients)
                .IsUnicode(false)
                .HasColumnName("ingredients");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
