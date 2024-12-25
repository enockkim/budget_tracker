
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
//using Prema.ShuleOne.Web.Server.Database.LocationData;
using Prema.ShuleOne.Web.Server.Models.Location;
using Prema.ShuleOne.Web.Server.Models;
using budget_tracker.Models;


namespace Prema.ShuleOne.Web.Server.Database
{
    public class ShuleOneDatabaseContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ShuleOneDatabaseContext(DbContextOptions<ShuleOneDatabaseContext> options) : base(options)
        {
            Console.WriteLine("ShuleOneDatabaseContext created");
        }

        public override void Dispose()
        {
            Console.WriteLine("ShuleOneDatabaseContext disposed");
            base.Dispose();
        }
        public DbSet<County> County { get; set; }
        public DbSet<Subcounty> Subcounty { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeBankDetails> EmployeeBankDetails { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentContact> StudentContact { get; set; }
        //public DbSet<Subject> Subject { get; set; }
        //public DbSet<Teacher> Teacher { get; set; }
        //public DbSet<SMSRecord> SMSRecord { get; set; }
        //public DbSet<SMSFailure> SMSFailure { get; set; }
        public DbSet<Grade> Grade { get; set; }
        //public DbSet<TutionFeesStructure> TutionFeesStructure { get; set; }
        //public DbSet<FeeType> FeeType { get; set; }
        //public DbSet<Document> Document { get; set; }
    }

}
