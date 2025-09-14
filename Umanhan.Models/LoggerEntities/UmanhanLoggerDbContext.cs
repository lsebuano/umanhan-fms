using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Umanhan.Models.LoggerEntities;

public partial class UmanhanLoggerDbContext : DbContext
{
    public UmanhanLoggerDbContext(DbContextOptions<UmanhanLoggerDbContext> options)
        : base(options)
    {
    }

    //public virtual DbSet<ChangeLog> ChangeLogs { get; set; }
    public virtual DbSet<EfQueryLog> EfQueryLogs { get; set; }
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

        modelBuilder.Entity<EfQueryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ef_query_logs_pkey");

            entity.ToTable("ef_query_logs");

            entity.HasIndex(e => e.ApiEndpoint, "idx_ef_query_logs_api_endpoint");

            entity.HasIndex(e => e.CreatedAt, "idx_ef_query_logs_created_at").IsDescending();

            entity.HasIndex(e => e.DurationMs, "idx_ef_query_logs_duration_ms").IsDescending();

            entity.HasIndex(e => e.FarmId, "idx_ef_query_logs_farm_id");

            entity.Property(e => e.Id).HasColumnName("query_id");
            entity.Property(e => e.ApiEndpoint).HasColumnName("api_endpoint");
            entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationMs).HasColumnName("duration_ms");
            entity.Property(e => e.Environment)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Prod'::character varying")
                .HasColumnName("environment");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.HttpMethod)
                .HasMaxLength(10)
                .HasColumnName("http_method");
            entity.Property(e => e.Parameters)
                .HasColumnType("jsonb")
                .HasColumnName("parameters");
            entity.Property(e => e.Query)
                .IsRequired()
                .HasColumnName("query");
            entity.Property(e => e.RowsReturned).HasColumnName("rows_returned");
            entity.Property(e => e.Source)
                .HasDefaultValueSql("'EFCore'::text")
                .HasColumnName("source");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
