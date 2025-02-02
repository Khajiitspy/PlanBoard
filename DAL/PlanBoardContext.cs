using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PlanBoardContext:DbContext // Standard Context
    {
        public PlanBoardContext(DbContextOptions<PlanBoardContext> options):base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BoardEntity> Boards { get; set; }
    }
}
