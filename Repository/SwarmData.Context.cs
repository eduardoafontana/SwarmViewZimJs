﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Repository
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_defaultConnection"))
        {

        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Breakpoints> Breakpoints { get; set; }
        public virtual DbSet<CodeFiles> CodeFiles { get; set; }
        public virtual DbSet<CodeMetrics> CodeMetrics { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<PathNodeParameters> PathNodeParameters { get; set; }
        public virtual DbSet<PathNodes> PathNodes { get; set; }
        public virtual DbSet<PharoSessions> PharoSessions { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
    }
}
