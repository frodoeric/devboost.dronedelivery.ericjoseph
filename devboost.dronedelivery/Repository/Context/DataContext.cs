using devboost.dronedelivery.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Repository.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Drone> Drone { get; set; }
        public DbSet<Pedido> Pedido { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //           @"my connection string",
        //           x => x.UseNetTopologySuite());
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Drone>().HasKey(x => x.Id);
            builder.Entity<Drone>().Property(x => x.StatusDrone).HasColumnName("Status");

            builder.Entity<Pedido>().HasKey(x => x.Id);

            builder.Entity<Pedido>().HasOne(x => x.Drone)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.DroneId);
        }
    }
}
