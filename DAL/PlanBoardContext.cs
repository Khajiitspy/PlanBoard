using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PlanBoardContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=ep-young-fire-a81ppksr-pooler.eastus2.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_gmcuiq1ov0Qr;Database=neondb;SSL Mode=Require;Trust Server Certificate=true");
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BoardEntity> Boards { get; set; }
    }
}
