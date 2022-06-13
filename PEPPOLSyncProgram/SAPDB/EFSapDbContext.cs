using PEPPOLSyncProgram.SAPDB.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEPPOLSyncProgram.SAPDB
{
    class EFSapDbContext : DbContext
    {
        // DB First Approach 
        public EFSapDbContext() : base("SAPDB") // EFSapDbContext - specified here is a connection string name
        {
            // DB First Approach mentioned this line
            Database.SetInitializer<EFSapDbContext>(null);
        }
        // DB First Approach 
        public DbSet<OINV> InvoiceHeaders { get; set; }
        public DbSet<INV1> InvoiceLines { get; set; }

        public DbSet<ORIN> CreditNoteHeaders { get; set; }
        public DbSet<RIN1> CreditNoteLines { get; set; }

        public DbSet<OCRD> Customers { get; set; }
        public DbSet<OADM> CompanyDetails { get; set; }

        public DbSet<ADM1> CompanyInfo { get; set; }
        public DbSet<OCTG> PaymentTerms { get; set; }
        public DbSet<OCPR> ContactPerson { get; set; }
        public DbSet<OACT> GLCodes { get; set; }
        public DbSet<ODRF> DraftDocs { get; set; }
        public DbSet<TaxMapping> TaxMappings { get; set; }
        public DbSet<UOMMapping> UomMappings { get; set; }
        protected override void OnModelCreating(DbModelBuilder model_builder)
        {
            base.OnModelCreating(model_builder);

            // Fluent API

        }
    }
}
