using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Umanhan.Models.Entities;

namespace Umanhan.Models.LoggerEntities;

public partial class UmanhanLoggerDbContext : DbContext
{
    public UmanhanLoggerDbContext(DbContextOptions<UmanhanLoggerDbContext> options)
        : base(options)
    {
    }

    //public virtual DbSet<ChangeLog> ChangeLogs { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<ChangeLog>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("change_logs_pkey");

        //    entity.ToTable("change_logs");

        //    entity.Property(e => e.Id)
        //        .HasDefaultValueSql("gen_random_uuid()")
        //        .HasColumnName("change_id");
        //    entity.Property(e => e.Entity)
        //        .IsRequired()
        //        .HasColumnName("entity");
        //    entity.Property(e => e.FarmId).HasColumnName("farm_id");
        //    entity.Property(e => e.Field)
        //        .IsRequired()
        //        .HasColumnName("field");
        //    entity.Property(e => e.NewValue).HasColumnName("new_value");
        //    entity.Property(e => e.OldValue).HasColumnName("old_value");
        //    entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        //});

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("logs_pkey");

            entity.ToTable("logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("log_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Exception).HasColumnName("exception");
            entity.Property(e => e.Level)
                .IsRequired()
                .HasMaxLength(10)
                .HasComment("INFO, DEBUG, ERROR")
                .HasColumnName("level");
            entity.Property(e => e.Message)
                .IsRequired()
                .HasColumnName("message");
            entity.Property(e => e.Properties)
                .HasColumnType("jsonb")
                .HasColumnName("properties");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_activities_pkey");

            entity.ToTable("user_activities");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("user_activity_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .HasColumnName("module_name");
            entity.Property(e => e.Path).HasColumnName("path");
            entity.Property(e => e.Properties)
                .HasColumnType("jsonb")
                .HasColumnName("properties");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Username)
                .HasMaxLength(200)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
