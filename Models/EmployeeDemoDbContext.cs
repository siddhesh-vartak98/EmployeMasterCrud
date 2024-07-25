using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmployeMasterCrud.Models;

public partial class EmployeeDemoDbContext : DbContext
{
    public EmployeeDemoDbContext()
    {
    }

    public EmployeeDemoDbContext(DbContextOptions<EmployeeDemoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AddressMaster> AddressMasters { get; set; }

    public virtual DbSet<CityMaster> CityMasters { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<EmployeeFile> EmployeeFiles { get; set; }

    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }

    public virtual DbSet<SalaryPackage> SalaryPackages { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DESKTOP-5ORVV5G\\SQLEXPRESS;Database=EmployeeDemoDB;Integrated Security=True;Encrypt=false;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressMaster>(entity =>
        {
            entity.HasKey(e => e.AddressId);

            entity.ToTable("AddressMaster");

            entity.Property(e => e.AddressId).HasColumnName("addressID");
            entity.Property(e => e.AddressLineOne)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("addressLineOne");
            entity.Property(e => e.AddressLineTwo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("addressLineTwo");
            entity.Property(e => e.CityId).HasColumnName("cityID");
            entity.Property(e => e.CountryId).HasColumnName("countryID");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.IsPrimary).HasColumnName("isPrimary");
            entity.Property(e => e.StateId).HasColumnName("stateID");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
            entity.Property(e => e.WhenModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenModified");
        });

        modelBuilder.Entity<CityMaster>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("CityMaster");

            entity.Property(e => e.CityId).HasColumnName("cityID");
            entity.Property(e => e.CityName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("cityName");
            entity.Property(e => e.CountryId).HasColumnName("countryID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.StateId).HasColumnName("stateID");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
            entity.Property(e => e.WhenModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenModified");
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId);

            entity.ToTable("CountryMaster");

            entity.Property(e => e.CountryId).HasColumnName("countryID");
            entity.Property(e => e.CountryName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("countryName");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
            entity.Property(e => e.WhenModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenModified");
        });

        modelBuilder.Entity<EmployeeFile>(entity =>
        {
            entity.HasKey(e => e.EmployeeFilesId);

            entity.Property(e => e.EmployeeFilesId).HasColumnName("employeeFilesID");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
        });

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.ToTable("EmployeeMaster");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("countryCode");
            entity.Property(e => e.CountryFlag)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("countryFlag");
            entity.Property(e => e.EmailId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("emailID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("mobileNo");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Otp)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("otp");
            entity.Property(e => e.ThumbnailImage)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("thumbnailImage");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
            entity.Property(e => e.WhenModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenModified");
        });

        modelBuilder.Entity<SalaryPackage>(entity =>
        {
            entity.ToTable("SalaryPackage");

            entity.Property(e => e.SalaryPackageId).HasColumnName("salaryPackageID");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.PackageFile)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("packageFile");
            entity.Property(e => e.PackageName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("packageName");
            entity.Property(e => e.PackageValue)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("packageValue");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.StateId);

            entity.ToTable("StateMaster");

            entity.Property(e => e.StateId).HasColumnName("stateID");
            entity.Property(e => e.CountryId).HasColumnName("countryID");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.StateName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("stateName");
            entity.Property(e => e.WhenEntered)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenEntered");
            entity.Property(e => e.WhenModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("whenModified");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
