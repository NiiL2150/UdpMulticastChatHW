using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdpMulticastChatHW.DBModel;

namespace UdpMulticastChatHW
{
    public class ChatDBContext : DbContext
    {
        public DbSet<DBUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=ChatDB; Integrated Security=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBUser>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<DBUser>()
                .Property(u => u.Name)
                .IsRequired();
            modelBuilder.Entity<DBUser>()
                .Property(u => u.Password)
                .IsRequired();
        }
    }
}
