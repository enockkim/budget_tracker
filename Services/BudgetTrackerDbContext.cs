using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using budget_tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace budget_tracker.Services
{
    public class BudgetTrackerDbContext : DbContext
    {
        public DbSet<Users> Users { set; get; }
        public DbSet<general_ledger> general_ledger { set; get; }
        public DbSet<fee_payment> fee_payment { set; get; }
        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options)
        {
            Console.WriteLine("BudgetTrackerDbContext created");
        }

        public override void Dispose()
        {
            Console.WriteLine("BudgetTrackerDbContext disposed");
            base.Dispose();
        }
    }
}