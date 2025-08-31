using Microsoft.EntityFrameworkCore;
using Umanhan.Models.Entities;
using Module = Umanhan.Models.Entities.Module;
using Task = Umanhan.Models.Entities.Task;

namespace Umanhan.Models;

public partial class UmanhanDbContext : DbContext
{
    public UmanhanDbContext()
    {
    }

    public UmanhanDbContext(DbContextOptions<UmanhanDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChangeLog> ChangeLogs { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerType> CustomerTypes { get; set; }

    public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }

    public virtual DbSet<Farm> Farms { get; set; }

    public virtual DbSet<FarmActivity> FarmActivities { get; set; }

    public virtual DbSet<FarmActivityExpense> FarmActivityExpenses { get; set; }

    public virtual DbSet<FarmActivityLaborer> FarmActivityLaborers { get; set; }

    public virtual DbSet<FarmActivityUsage> FarmActivityUsages { get; set; }

    public virtual DbSet<FarmContract> FarmContracts { get; set; }

    public virtual DbSet<FarmContractDetail> FarmContractDetails { get; set; }

    public virtual DbSet<FarmContractSale> FarmContractSales { get; set; }

    public virtual DbSet<FarmCrop> FarmCrops { get; set; }

    public virtual DbSet<FarmGeneralExpense> FarmGeneralExpenses { get; set; }

    public virtual DbSet<FarmInventory> FarmInventories { get; set; }

    public virtual DbSet<FarmLivestock> FarmLivestocks { get; set; }

    public virtual DbSet<FarmProduceInventory> FarmProduceInventories { get; set; }

    public virtual DbSet<FarmTransaction> FarmTransactions { get; set; }

    public virtual DbSet<FarmZone> FarmZones { get; set; }

    public virtual DbSet<FarmZoneYield> FarmZoneYields { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Laborer> Laborers { get; set; }

    public virtual DbSet<Livestock> Livestocks { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<SoilType> SoilTypes { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<PricingCondition> PricingConditions { get; set; }

    public virtual DbSet<PricingConditionType> PricingConditionTypes { get; set; }

    public virtual DbSet<VwProductLookup> VwProductLookups { get; set; }

    public virtual DbSet<PricingProfile> PricingProfiles { get; set; }

    public virtual DbSet<Quotation> Quotations { get; set; }

    public virtual DbSet<QuotationDetail> QuotationDetails { get; set; }

    public virtual DbSet<FarmActivityPhoto> FarmActivityPhotos { get; set; }

    public virtual DbSet<QuotationProduct> QuotationProducts { get; set; }

    public virtual DbSet<FarmContractPayment> FarmContractPayments { get; set; }
    public virtual DbSet<FarmContractPaymentDetail> FarmContractPaymentDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "categories_category_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasColumnName("category_name");
            entity.Property(e => e.Group).HasColumnName("group");
            entity.Property(e => e.Group2).HasColumnName("group_2");
            entity.Property(e => e.ConsumptionBehavior).HasColumnName("consumption_behavior");
        });

        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("change_logs_pkey");

            entity.ToTable("change_logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("change_id");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasColumnName("username");
            entity.Property(e => e.Entity)
                .IsRequired()
                .HasColumnName("entity");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.Field)
                .IsRequired()
                .HasColumnName("field");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("crops_pkey");

            entity.ToTable("crops");

            entity.HasIndex(e => e.CropName, "crops_crop_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("crop_id");
            entity.Property(e => e.CropName)
                .IsRequired()
                .HasColumnName("crop_name");
            entity.Property(e => e.CropVariety).HasColumnName("crop_variety");
            entity.Property(e => e.DefaultRatePerUnit)
                .HasPrecision(10, 2)
                .HasColumnName("default_rate_per_unit");
            entity.Property(e => e.DefaultUnitId).HasColumnName("default_unit_id");
            entity.Property(e => e.Notes).HasColumnName("notes");

            entity.HasOne(d => d.DefaultUnit).WithMany(p => p.Crops)
                .HasForeignKey(d => d.DefaultUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("crops_default_unit_id_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.CustomerName, "customers_customer_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("customer_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactInfo).HasColumnName("contact_info");
            entity.Property(e => e.EmailAddress).HasColumnName("email_address");
            entity.Property(e => e.Tin).HasColumnName("tin");
            entity.Property(e => e.ContractEligible)
                .HasColumnName("contract_eligible")
                .HasDefaultValue(false);
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerTypeId).HasColumnName("customer_type_id");

            entity.HasOne(d => d.CustomerType).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CustomerTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_customer_type_id_fkey");
        });

        modelBuilder.Entity<CustomerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_types_pkey");

            entity.ToTable("customer_types");

            entity.HasIndex(e => e.CustomerTypeName, "customer_types_customer_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("type_id");
            entity.Property(e => e.CustomerTypeName)
                .IsRequired()
                .HasColumnName("customer_type_name");
        });

        modelBuilder.Entity<ExpenseType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expense_types_pkey");

            entity.ToTable("expense_types");

            entity.HasIndex(e => e.ExpenseTypeName, "expense_types_expense_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("type_id");

            entity.Property(e => e.ExpenseTypeName)
                .IsRequired()
                .HasColumnName("expense_type_name");
        });

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farms_pkey");

            entity.ToTable("farms");

            entity.HasIndex(e => e.FarmName, "farms_farm_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("farm_id");
            entity.Property(e => e.BoundaryJson).HasColumnName("boundary_json");
            entity.Property(e => e.FarmName)
                .IsRequired()
                .HasColumnName("farm_name");
            entity.Property(e => e.Location)
                .IsRequired()
                .HasColumnName("location");
            entity.Property(e => e.FullAddress).HasColumnName("full_address");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OwnerName).HasColumnName("owner_name");
            entity.Property(e => e.SizeInHectares)
                .HasPrecision(10, 2)
                .HasColumnName("size_in_hectares");
            entity.Property(e => e.SizeInSqm)
                .HasPrecision(10, 2)
                .HasColumnName("size_in_sqm");
            entity.Property(e => e.Lat)
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasColumnName("lng");
            entity.Property(e => e.SetupComplete)
                .HasColumnName("setup_complete");
            entity.Property(e => e.SetupStarted)
                .HasColumnName("setup_started");
            entity.Property(e => e.StaticMapUrl)
                .HasColumnName("static_map_url");
            entity.Property(e => e.Tin)
                .HasColumnName("tin");
            entity.Property(e => e.ContactPhone)
                .HasColumnName("contact_phone");
            entity.Property(e => e.ContactEmail)
                .HasColumnName("contact_email");
        });

        modelBuilder.Entity<FarmActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_activities_pkey");

            entity.ToTable("farm_activities");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("activity_id");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId)
                .HasComment("id from either crops or livestocks tables, no FK")
                .HasColumnName("product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SupervisorId).HasColumnName("supervisor_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Contract).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("farm_activities_contract_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_activities_farm_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activities_product_type_id_fkey");

            entity.HasOne(d => d.Supervisor).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activities_supervisor_id_fkey");

            entity.HasOne(d => d.Task).WithMany(p => p.FarmActivities)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activities_task_id_fkey");
        });

        modelBuilder.Entity<FarmActivityExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_activity_expenses_pkey");

            entity.ToTable("farm_activity_expenses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("date");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("description");
            entity.Property(e => e.ExpenseTypeId).HasColumnName("expense_type_id");

            //entity.HasOne(d => d.Activity).WithMany(p => p.FarmActivityExpenses)
            //    .HasForeignKey(d => d.ActivityId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("farm_activity_expenses_activity_id_fkey");

            //entity.HasOne(d => d.ExpenseType).WithOne(p => p.FarmActivityExpense)
            //    .HasForeignKey<FarmActivityExpense>(d => d.ExpenseTypeId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("farm_activity_expenses_expense_type_id_fkey");

            entity.HasOne(d => d.Activity).WithMany(p => p.FarmActivityExpenses)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_expenses_activity_id_fkey");

            entity.HasOne(d => d.ExpenseType).WithMany(p => p.FarmActivityExpenses)
                .HasForeignKey(d => d.ExpenseTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_expenses_expense_type_id_fkey");
        });

        modelBuilder.Entity<FarmActivityLaborer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_activity_laborers_pkey");

            entity.ToTable("farm_activity_laborers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("labor_activity_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.LaborerId).HasColumnName("laborer_id");
            entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("now()")
                .HasColumnName("timestamp");
            entity.Property(e => e.QuantityWorked)
                .HasColumnName("quantity_worked");
            entity.Property(e => e.TotalPayment)
                .HasPrecision(10, 2)
                .HasColumnName("total_payment");

            entity.HasOne(d => d.Activity).WithMany(p => p.FarmActivityLaborers)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_laborers_activity_id_fkey");

            entity.HasOne(d => d.Laborer).WithMany(p => p.FarmActivityLaborers)
                .HasForeignKey(d => d.LaborerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_laborers_laborer_id_fkey");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.FarmActivityLaborers)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_laborers_payment_type_id_fkey");
        });

        modelBuilder.Entity<FarmActivityUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_activity_usages_pkey");

            entity.ToTable("farm_activity_usages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("usage_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("now()")
                .HasColumnName("timestamp");
            entity.Property(e => e.Rate)
                .HasPrecision(10, 2)
                .HasColumnName("rate");
            entity.Property(e => e.TotalCost)
                .HasPrecision(10, 2)
                .HasColumnName("total_cost");
            entity.Property(e => e.UsageHours)
                .HasPrecision(5, 2)
                .HasColumnName("usage_hours");

            entity.HasOne(d => d.Activity).WithMany(p => p.FarmActivityUsages)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_usages_activity_id_fkey");

            entity.HasOne(d => d.Inventory).WithMany(p => p.FarmActivityUsages)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_usages_inventory_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmActivityUsages)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_usages_unit_id_fkey");
        });

        modelBuilder.Entity<FarmActivityPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_activity_photos_pkey");

            entity.ToTable("farm_activity_photos");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("photo_id");
            entity.Property(e => e.ActivityId).HasColumnName("activity_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("now()")
                .HasColumnName("timestamp");
            entity.Property(e => e.PhotoUrlThumbnail).IsRequired().HasColumnName("photo_url_thumbnail");
            entity.Property(e => e.PhotoUrlFull).IsRequired().HasColumnName("photo_url_full");
            entity.Property(e => e.MimeType).IsRequired().HasColumnName("mime_type");
            entity.Property(e => e.Notes).HasColumnName("notes");

            entity.HasOne(d => d.Activity).WithMany(p => p.FarmActivityPhotos)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_activity_photos_activity_id_fkey");
        });

        modelBuilder.Entity<FarmContract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_contracts_pkey");

            entity.ToTable("farm_contracts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_id");
            entity.Property(e => e.ContractDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("contract_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasComment("Pending, Completed, Cancelled")
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.FarmContracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contracts_customer_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmContracts)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_contracts_farm_id_fkey");
        });

        modelBuilder.Entity<FarmContractDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_contract_details_pkey");

            entity.ToTable("farm_contract_details");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_detail_id");
            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.ContractedQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("contracted_quantity");
            entity.Property(e => e.DeliveredQuantity)
                .HasPrecision(10, 2)
                .HasDefaultValue(0)
                .HasColumnName("delivered_quantity");
            entity.Property(e => e.ContractedUnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("contracted_unit_price");
            entity.Property(e => e.HarvestDate).HasColumnName("harvest_date");
            entity.Property(e => e.PickupConfirmed)
                .HasDefaultValue(false)
                .HasColumnName("pickup_confirmed");
            entity.Property(e => e.PickupDate).HasColumnName("pickup_date");
            entity.Property(e => e.PaidDate).HasColumnName("paid_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.PricingProfileId).HasColumnName("pricing_profile_id");
            entity.Property(e => e.IsRecovered)
                .HasDefaultValue(false)
                .HasColumnName("is_recovered");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Contract).WithMany(p => p.FarmContractDetails)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("farm_contract_details_contract_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.FarmContractDetails)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contract_details_product_type_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmContractDetails)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contract_details_unit_id_fkey");

            entity.HasOne(d => d.PricingProfile).WithMany(p => p.FarmContractDetails)
                .HasForeignKey(d => d.PricingProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contract_details_pricing_profile_id_fkey");
        });

        modelBuilder.Entity<FarmContractSale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_contract_sales_pkey");

            entity.ToTable("farm_contract_sales");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_sale_id");
            entity.Property(e => e.ContractDetailId).HasColumnName("contract_detail_id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnName("date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.ProductVariety).HasColumnName("product_variety");
            entity.Property(e => e.ProductTypeName).HasColumnName("product_type_name");
            entity.Property(e => e.CustomerName).HasColumnName("customer_name");
            entity.Property(e => e.UnitName).HasColumnName("unit_name");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.ContractDetail).WithMany(p => p.FarmContractSales)
                .HasForeignKey(d => d.ContractDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contract_sales_contract_detail_id_fkey");
        });

        modelBuilder.Entity<FarmCrop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_crops_pkey");

            entity.ToTable("farm_crops");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("farm_crop_id");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.DefaultRate)
                .HasPrecision(10, 2)
                .HasColumnName("default_rate")
                .HasDefaultValue(0);
            entity.Property(e => e.EstimatedHarvestDate).HasColumnName("estimated_harvest_date");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.PlantingDate).HasColumnName("planting_date");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Crop).WithMany(p => p.FarmCrops)
                .HasForeignKey(d => d.CropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_crops_crop_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmCrops)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_crops_farm_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmCrops)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_crops_unit_id_fkey");

            entity.HasOne(d => d.Zone).WithMany(p => p.FarmCrops)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_crops_zone_id_fkey");
        });

        modelBuilder.Entity<FarmInventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("farm_inventories_pkey");

            entity.ToTable("farm_inventories", tb => tb.HasComment("For farm equipments, materials, tools, etc"));

            entity.Property(e => e.InventoryId)
                .ValueGeneratedNever()
                .HasColumnName("inventory_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("farm_inventory_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ItemImageS3KeyThumbnail).HasColumnName("item_image_s3_key_thumbnail");
            entity.Property(e => e.ItemImageS3ContentType).HasColumnName("item_image_s3_content_type");
            entity.Property(e => e.ItemImageS3KeyFull).HasColumnName("item_image_s3_key_full");
            entity.Property(e => e.ItemImageS3UrlThumbnail).HasColumnName("item_image_s3_url_thumbnail");
            entity.Property(e => e.ItemImageS3UrlFull).HasColumnName("item_image_s3_url_full");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmInventories)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_inventories_farm_id_fkey");

            entity.HasOne(d => d.Inventory).WithOne(p => p.FarmInventory)
                .HasForeignKey<FarmInventory>(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_inventories_inventory_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmInventories)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_inventories_unit_id_fkey");
        });

        modelBuilder.Entity<FarmLivestock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_livestocks_pkey");

            entity.ToTable("farm_livestocks");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("farm_livestock_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Breed).HasColumnName("breed");
            entity.Property(e => e.DefaultRate)
                .HasPrecision(10, 2)
                .HasColumnName("default_rate");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.LivestockId).HasColumnName("livestock_id");
            entity.Property(e => e.PurchaseCost)
                .HasPrecision(10, 2)
                .HasColumnName("purchase_cost");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.EstimatedHarvestDate).HasColumnName("estimated_harvest_date");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmLivestocks)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_livestocks_farm_id_fkey");

            entity.HasOne(d => d.Livestock).WithMany(p => p.FarmLivestocks)
                .HasForeignKey(d => d.LivestockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_livestocks_livestock_id_fkey");

            entity.HasOne(d => d.Zone).WithMany(p => p.FarmLivestocks)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_livestocks_zone_id_fkey");
        });

        modelBuilder.Entity<FarmProduceInventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_produce_inventories_pkey");

            entity.ToTable("farm_produce_inventories", tb => tb.HasComment("For long-storage farm produce. Entries here are also for sale or for surplus donation."));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("inventory_id");
            entity.Property(e => e.CurrentQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("current_quantity");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnName("date");
            entity.Property(e => e.InitialQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("initial_quantity");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId)
                .HasComment("Either crop_id or livestock_id. no FK")
                .HasColumnName("product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmProduceInventories)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_produce_inventories_unit_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.FarmProduceInventories)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_produce_inventories_product_type_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmProduceInventories)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_produce_inventories_farm_id_fkey");
        });

        modelBuilder.Entity<FarmTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_transactions_pkey");

            entity.ToTable("farm_transactions", tb => tb.HasComment("Transaction records from the produce inventory"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("transaction_id");
            entity.Property(e => e.BuyerId).HasColumnName("buyer_id");
            entity.Property(e => e.BuyerName).HasColumnName("buyer_name");
            entity.Property(e => e.PaymentType)
                .HasDefaultValue("CASH")
                .HasColumnName("payment_type");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProduceInventoryId)
                .HasComment("produce inventory id")
                .HasColumnName("produce_inventory_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Buyer).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.BuyerId)
                .HasConstraintName("farm_transactions_buyer_id_fkey");

            entity.HasOne(d => d.ProduceInventory).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.ProduceInventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_transactions_produce_inventory_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_transactions_product_type_id_fkey");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_transactions_transaction_type_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_transactions_unit_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmTransactions)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_transactions_farm_id_fkey");
        });

        modelBuilder.Entity<FarmZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_zones_pkey");

            entity.ToTable("farm_zones");

            entity.HasIndex(e => e.ZoneName, "farm_zones_zone_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("zone_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.IrrigationType).HasColumnName("irrigation_type");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.SizeInHectares)
                .HasPrecision(10, 2)
                .HasColumnName("size_in_hectares");
            entity.Property(e => e.SizeInSqm)
                .HasPrecision(10, 2)
                .HasColumnName("size_in_sqm");
            entity.Property(e => e.SoilId).HasColumnName("soil_id");
            entity.Property(e => e.ZoneName)
                .IsRequired()
                .HasColumnName("zone_name");
            entity.Property(e => e.ZoneDescription)
                .HasColumnName("zone_description");
            entity.Property(e => e.BoundaryJson)
                .HasColumnName("boundary_json");
            entity.HasOne(d => d.Farm).WithMany(p => p.FarmZones)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("farm_zones_farm_id_fkey");
            entity.HasOne(d => d.Soil).WithMany(p => p.FarmZones)
                .HasForeignKey(d => d.SoilId)
                .HasConstraintName("farm_zones_soil_id_fkey");
            entity.Property(e => e.Lat)
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasColumnName("lng");
            entity.Property(e => e.ZoneColor)
                .HasColumnName("zone_color");
        });

        modelBuilder.Entity<FarmZoneYield>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_zone_yields_pkey");

            entity.ToTable("farm_zone_yields");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("yield_id");
            entity.Property(e => e.ActualYield)
                .HasPrecision(10, 2)
                .HasColumnName("actual_yield");
            entity.Property(e => e.ProductId).HasColumnName("crop_id");
            entity.Property(e => e.ExpectedYield)
                .HasPrecision(10, 2)
                .HasColumnName("expected_yield");
            entity.Property(e => e.ForecastedYield)
                .HasPrecision(10, 2)
                .HasColumnName("forecasted_yield");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.Property(e => e.HarvestDate).HasColumnName("harvest_date");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmZoneYields)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_zone_yields_farm_id_fkey");

            entity.HasOne(d => d.Zone).WithMany(p => p.FarmZoneYields)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_zone_yields_zone_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.FarmZoneYields)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("farm_zone_yields_unit_id_fkey");

            entity.HasOne(d => d.FarmContractDetail).WithMany(p => p.FarmZoneYields)
                .HasForeignKey(d => d.ContractDetailId)
                .HasConstraintName("farm_zone_yields_contract_detail_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.FarmZoneYields)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("farm_zone_yields_product_type_id_fkey");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventories_pkey");

            entity.ToTable("inventories");

            entity.HasIndex(e => e.ItemName, "inventories_item_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("inventory_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ItemName)
                .IsRequired()
                .HasColumnName("item_name");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventories_category_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("inventories_unit_id_fkey");
        });

        modelBuilder.Entity<Laborer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("laborers_pkey");

            entity.ToTable("laborers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("laborer_id");
            entity.Property(e => e.ContactInfo).HasColumnName("contact_info");
            entity.Property(e => e.ContractedRate)
                .HasPrecision(10, 2)
                .HasColumnName("contracted_rate");
            entity.Property(e => e.DailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("daily_rate");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Skillset).HasColumnName("skillset");
        });

        modelBuilder.Entity<Livestock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("livestocks_pkey");

            entity.ToTable("livestocks");

            entity.HasIndex(e => e.AnimalType, "livestocks_animal_type_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("livestock_id");
            entity.Property(e => e.AnimalType)
                .IsRequired()
                .HasColumnName("animal_type");
            entity.Property(e => e.Breed).HasColumnName("breed");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payment_types_pkey");

            entity.ToTable("payment_types");

            entity.HasIndex(e => e.PaymentTypeName, "payment_types_payment_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("payment_type_id");
            entity.Property(e => e.PaymentTypeName)
                .IsRequired()
                .HasColumnName("payment_type_name");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_types_pkey");

            entity.ToTable("product_types");

            entity.HasIndex(e => e.ProductTypeName, "product_types_product_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("type_id");
            entity.Property(e => e.ProductTypeName)
                .IsRequired()
                .HasColumnName("product_type_name");
            entity.Property(e => e.TableName)
                .IsRequired()
                .HasColumnName("table_name");
        });

        modelBuilder.Entity<SoilType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("soil_types_pkey");

            entity.ToTable("soil_types");

            entity.HasIndex(e => e.SoilName, "soil_types_soil_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("soil_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.SoilFertility)
                .HasComment("Low, Medium, High")
                .HasColumnName("soil_fertility");
            entity.Property(e => e.SoilMoisture)
                .HasComment("Dry, Moist, Wet")
                .HasColumnName("soil_moisture");
            entity.Property(e => e.SoilName)
                .IsRequired()
                .HasColumnName("soil_name");
            entity.Property(e => e.SoilOrganicCarbon)
                .HasComment("Low, Medium, High")
                .HasColumnName("soil_organic_carbon");
            entity.Property(e => e.SoilPh)
                .HasComment("Acidic, Neutral, Alkaline")
                .HasColumnName("soil_ph");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staffs_pkey");

            entity.ToTable("staffs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("staff_id");
            entity.Property(e => e.ContactInfo).HasColumnName("contact_info");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Farm).WithMany(p => p.Staffs)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("staffs_farm_id_fkey");
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("system_settings_pkey");

            entity.ToTable("system_settings");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("setting_id");
            entity.Property(e => e.SettingName)
                .IsRequired()
                .HasColumnName("setting_name");
            entity.Property(e => e.SettingValue)
                .HasColumnName("setting_value");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.HasIndex(e => e.TaskName, "tasks_task_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("task_id");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");

            entity.Property(e => e.TaskName).HasColumnName("task_name");

            entity.HasOne(d => d.Category).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasks_category_id_fkey");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_types_pkey");

            entity.ToTable("transaction_types");

            entity.HasIndex(e => e.TransactionTypeName, "transaction_types_transaction_type_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("type_id");

            entity.Property(e => e.TransactionTypeName)
                .IsRequired()
                .HasColumnName("transaction_type_name");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("units_pkey");

            entity.ToTable("units");

            entity.HasIndex(e => e.UnitName, "units_unit_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("unit_id");

            entity.Property(e => e.UnitName)
                .IsRequired()
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.TemplateId).HasColumnName("template_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("role_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(false)
                .HasColumnName("is_active");

            entity.HasOne(d => d.DashboardTemplate).WithMany(p => p.Roles)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("roles_template_id_fkey");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modules_pkey");

            entity.ToTable("modules");

            entity.HasIndex(e => e.Name, "modules_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("module_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Name, "permissions_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("permission_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions");

            entity.HasIndex(e => new { e.RoleId, e.ModuleId, e.PermissionId }, "role_permissions_role_id_module_id_permission_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("role_permission_id");

            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Module).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("role_permissions_module_id_fkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("role_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("role_permissions_role_id_fkey");
        });

        modelBuilder.Entity<PricingCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pricing_conditions_pkey");

            entity.ToTable("pricing_conditions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("pricing_id");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnName("value");

            entity.Property(e => e.ApplyType)
                .IsRequired()
                .HasColumnName("apply_type");

            entity.Property(e => e.Sequence)
                .IsRequired()
                .HasColumnName("sequence");

            entity.Property(e => e.ProfileId)
                .IsRequired()
                .HasColumnName("profile_id");

            entity.Property(e => e.ConditionTypeId)
                .IsRequired()
                .HasColumnName("condition_type_id");

            entity.HasOne(d => d.PricingProfile).WithMany(p => p.PricingConditions)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("pricing_conditions_profile_id_fkey");

            entity.HasOne(d => d.ConditionType).WithMany(p => p.PricingConditions)
                .HasForeignKey(d => d.ConditionTypeId)
                .HasConstraintName("pricing_conditions_condition_type_id_fkey");
        });

        modelBuilder.Entity<PricingConditionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pricing_condition_types_pkey");

            entity.ToTable("pricing_condition_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("condition_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");

            entity.Property(e => e.IsDeduction)
                .IsRequired()
                .HasColumnName("is_deduction");
        });

        modelBuilder.Entity<PricingProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pricing_profile_pkey");

            entity.ToTable("pricing_profiles");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("profile_id");

            entity.Property(e => e.FarmId).HasColumnName("farm_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");

            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FinalPrice)
                .HasPrecision(10, 2)
                .HasDefaultValue(0)
                .HasColumnName("final_price");

            entity.HasIndex(e => new { e.Name, e.FarmId }, "pricing_profiles_name_farm_id_key").IsUnique();

            entity.HasOne(d => d.Farm).WithMany(p => p.PricingProfiles)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("pricing_profile_farm_id_fkey");
        });

        modelBuilder.Entity<VwProductLookup>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_product_lookup");

            entity.Property(e => e.DefaultRate)
                .HasPrecision(10, 2)
                .HasColumnName("default_rate");
            entity.Property(e => e.EstimatedHarvestDate).HasColumnName("estimated_harvest_date");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.FarmName).HasColumnName("farm_name");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.Variety).HasColumnName("variety");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.ProductTypeName).HasColumnName("product_type_name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UnitName).HasColumnName("unit_name");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.ZoneName).HasColumnName("zone_name");
        }); 
        
        modelBuilder.Entity<Quotation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotations_pkey");

            entity.ToTable("quotations");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("quotation_id");
            entity.Property(e => e.AwsSesMessageId).HasColumnName("aws_ses_message_id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(12, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.DateSent)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_sent");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.FinalPrice)
                .HasPrecision(12, 2)
                .HasColumnName("final_price");
            entity.Property(e => e.PricingProfileId).HasColumnName("pricing_profile_id");
            entity.Property(e => e.QuotationNumber)
                .IsRequired()
                .HasColumnName("quotation_number");
            entity.Property(e => e.RecipientEmail)
                .IsRequired()
                .HasColumnName("recipient_email");
            entity.Property(e => e.RecipientName)
                .IsRequired()
                .HasColumnName("recipient_name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'SENT'::text")
                .HasColumnName("status");
            entity.Property(e => e.ValidUntil).HasColumnName("valid_until");

            entity.HasOne(d => d.Farm).WithMany(p => p.Quotations)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotations_farm_id_fkey");

            entity.HasOne(d => d.PricingProfile).WithMany(p => p.Quotations)
                .HasForeignKey(d => d.PricingProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotations_pricing_profile_id_fkey");
        });

        modelBuilder.Entity<QuotationProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotation_products_pkey");

            entity.ToTable("quotation_products");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("quotation_product_id");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QuotationId).HasColumnName("quotation_id");

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Quotation).WithMany(p => p.QuotationProducts)
                .HasForeignKey(d => d.QuotationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotation_products_quotation_id_fkey");

            entity.HasOne(d => d.ProductType).WithMany(p => p.QuotationProducts)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotation_products_product_type_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.QuotationProducts)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotation_products_unit_id_fkey");
        });

        modelBuilder.Entity<QuotationDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("quotation_details_pkey");

            entity.ToTable("quotation_details");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("detail_id");
            entity.Property(e => e.After)
                .HasPrecision(12, 2)
                .HasColumnName("after");
            entity.Property(e => e.ApplyType)
                .IsRequired()
                .HasColumnName("apply_type");
            entity.Property(e => e.Before)
                .HasPrecision(12, 2)
                .HasColumnName("before");
            entity.Property(e => e.ConditionType)
                .IsRequired()
                .HasColumnName("condition_type");
            entity.Property(e => e.Delta)
                .HasPrecision(12, 2)
                .HasColumnName("delta");
            entity.Property(e => e.IsDeduction).HasColumnName("is_deduction");
            entity.Property(e => e.QuotationId).HasColumnName("quotation_id");
            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.Value)
                .HasPrecision(10, 2)
                .HasColumnName("value");

            entity.HasOne(d => d.Quotation).WithMany(p => p.QuotationDetails)
                .HasForeignKey(d => d.QuotationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("quotation_details_quotation_id_fkey");
        });

        modelBuilder.Entity<DashboardTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dashboard_template_pkey");

            entity.ToTable("dashboard_templates");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("template_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name");

            entity.HasIndex(e => e.Name, "dashboard_templates_name_key").IsUnique();

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.DashboardComponentName)
                .IsRequired()
                .HasColumnName("dashboard_component_name");
        });

        modelBuilder.Entity<FarmGeneralExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_general_expenses_pkey");

            entity.ToTable("farm_general_expenses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("expense_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ExpenseTypeId).HasColumnName("expense_type_id");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Payee).HasColumnName("payee");

            entity.HasOne(d => d.ExpenseType).WithMany(p => p.FarmGeneralExpenses)
                .HasForeignKey(d => d.ExpenseTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_general_expenses_expense_type_id_fkey");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmGeneralExpenses)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_general_expenses_farm_id_fkey");
        });

        modelBuilder.Entity<FarmKpiSummary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_kpi_summaries_pkey");

            entity.ToTable("farm_kpi_summaries");

            entity.HasIndex(e => new { e.FarmId, e.PeriodType, e.PeriodDate }, "idx_farm_kpi_period");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.KpiName)
                .IsRequired()
                .HasColumnName("kpi_name");
            entity.Property(e => e.KpiValue)
                .HasPrecision(18, 2)
                .HasColumnName("kpi_value");
            entity.Property(e => e.PeriodDate).HasColumnName("period_date");
            entity.Property(e => e.PeriodType)
                .IsRequired()
                .HasColumnName("period_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.SourceRun).HasColumnName("source_run");
        }); 
        
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.ToTable("weather_forecasts");

            entity.HasKey(e => e.Id)
                  .HasName("weather_forecasts_pkey");

            entity.Property(e => e.Id)
                  .HasColumnName("forecast_id")
                  .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Lat)
                  .HasColumnName("lat");

            entity.Property(e => e.Lng)
                  .HasColumnName("lng");

            entity.Property(e => e.Date)
                  .HasColumnName("date")
                  .IsRequired();

            entity.Property(e => e.TemperatureMin)
                  .HasColumnName("temperature_min");

            entity.Property(e => e.TemperatureMax)
                  .HasColumnName("temperature_max");

            entity.Property(e => e.TemperatureAverage)
                  .HasColumnName("temperature_ave");

            entity.Property(e => e.Humidity)
                  .HasColumnName("humidity");

            entity.Property(e => e.WindSpeed)
                  .HasColumnName("wind_speed");

            entity.Property(e => e.WindAngleDegree)
                  .HasColumnName("wind_angle_degree");

            entity.Property(e => e.WindGust)
                  .HasColumnName("wind_gust");

            entity.Property(e => e.Cloudiness)
                  .HasColumnName("cloudiness");
        }); 
        
        modelBuilder.Entity<FarmContractPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_contract_payments_pkey");

            entity.ToTable("farm_contract_payments");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_payment_id");
            entity.Property(e => e.AmountInWords)
                .IsRequired()
                .HasColumnName("amount_in_words");
            entity.Property(e => e.BuyerAddress).HasColumnName("buyer_address");
            entity.Property(e => e.BuyerContactNo).HasColumnName("buyer_contact_no");
            entity.Property(e => e.BuyerName)
                .IsRequired()
                .HasColumnName("buyer_name");
            entity.Property(e => e.BuyerTin).HasColumnName("buyer_tin");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DocumentType)
                .IsRequired()
                .HasColumnName("document_type");
            entity.Property(e => e.FarmAddress)
                .IsRequired()
                .HasColumnName("farm_address");
            entity.Property(e => e.FarmContactEmail).HasColumnName("farm_contact_email");
            entity.Property(e => e.FarmContactPhone)
                .IsRequired()
                .HasColumnName("farm_contact_phone");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.FarmLogo).HasColumnName("farm_logo");
            entity.Property(e => e.FarmName)
                .IsRequired()
                .HasColumnName("farm_name");
            entity.Property(e => e.FarmTin)
                .IsRequired()
                .HasColumnName("farm_tin");
            entity.Property(e => e.InvoiceNo)
                .IsRequired()
                .HasColumnName("invoice_no");
            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentRef).HasColumnName("payment_ref");
            entity.Property(e => e.PrintTimestamp).HasColumnName("print_timestamp");
            entity.Property(e => e.PrintedBy)
                .IsRequired()
                .HasColumnName("printed_by");
            entity.Property(e => e.ReceivedBy)
                .IsRequired()
                .HasColumnName("received_by");
            entity.Property(e => e.SystemRefNo)
                .IsRequired()
                .HasColumnName("system_ref_no");
            entity.Property(e => e.TotalAdjustments)
                .HasPrecision(10, 2)
                .HasColumnName("total_adjustments");
            entity.Property(e => e.TotalAmountReceived)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount_received");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
        });

        modelBuilder.Entity<FarmContractPaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_contract_payment_details_pkey");

            entity.ToTable("farm_contract_payment_details");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contract_payment_detail_id");
            entity.Property(e => e.ContractPaymentId).HasColumnName("contract_payment_id");
            entity.Property(e => e.Item)
                .IsRequired()
                .HasColumnName("item");
            entity.Property(e => e.Quantity)
                .HasPrecision(10, 2)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.Unit)
                .IsRequired()
                .HasColumnName("unit");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.ContractPayment).WithMany(p => p.FarmContractPaymentDetails)
                .HasForeignKey(d => d.ContractPaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_contract_payment_details_contract_payment_id_fkey");
        });

        modelBuilder.Entity<FarmNumberSery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farm_number_series_pkey");

            entity.ToTable("farm_number_series");

            entity.HasIndex(e => new { e.FarmId, e.Type }, "farm_number_series_farm_id_type_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("number_sery_id");
            entity.Property(e => e.CurrentSery).HasColumnName("current_sery");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.Format)
                .IsRequired()
                .HasColumnName("format");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type");

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmNumberSeries)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("farm_number_series_farm_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
