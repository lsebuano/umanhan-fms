using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Umanhan.Models;
using Umanhan.Models.Entities;
using Umanhan.Models.LoggerEntities;
using Umanhan.Repositories.Interfaces;

namespace Umanhan.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UmanhanDbContext _context;
        public ICategoryRepository Categories { get; private set; }
        public IChangeLogRepository ChangeLogs { get; private set; }
        public ICropRepository Crops { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public ICustomerTypeRepository CustomerTypes { get; private set; }
        public IExpenseTypeRepository ExpenseTypes { get; private set; }
        public IFarmActivityRepository FarmActivities { get; private set; }
        public IFarmActivityExpenseRepository FarmActivityExpenses { get; private set; }
        public IFarmActivityLaborerRepository FarmActivityLaborers { get; private set; }
        public IFarmActivityUsageRepository FarmActivityUsages { get; private set; }
        public IFarmContractRepository FarmContracts { get; private set; }
        public IFarmContractDetailRepository FarmContractDetails { get; private set; }
        public IFarmContractSaleRepository FarmContractSales { get; private set; }
        public IFarmCropRepository FarmCrops { get; private set; }
        public IFarmInventoryRepository FarmInventories { get; private set; }
        public IFarmLivestockRepository FarmLivestocks { get; private set; }
        public IFarmProduceInventoryRepository FarmProduceInventories { get; private set; }
        public IFarmRepository Farms { get; private set; }
        public IFarmTransactionRepository FarmTransactions { get; private set; }
        public IFarmZoneRepository FarmZones { get; private set; }
        public IFarmZoneYieldRepository FarmZoneYields { get; private set; }
        public IInventoryRepository Inventories { get; private set; }
        public ILaborerRepository Laborers { get; private set; }
        public ILivestockRepository Livestocks { get; private set; }
        public IPaymentTypeRepository PaymentTypes { get; private set; }
        public IProductTypeRepository ProductTypes { get; private set; }
        public ISoilTypeRepository SoilTypes { get; private set; }
        public IStaffRepository Staffs { get; private set; }
        public ISystemSettingRepository SystemSettings { get; private set; }
        public ITaskRepository Tasks { get; private set; }
        public ITransactionTypeRepository TransactionTypes { get; private set; }
        public IUnitRepository Units { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IModuleRepository Modules { get; private set; }
        public IPermissionRepository Permissions { get; private set; }
        public IRolePermissionRepository RolePermissions { get; private set; }
        public IReportRepository Reports { get; private set; }

        public IPricingConditionRepository PricingConditions { get; private set; }
        public IPricingConditionTypeRepository PricingConditionTypes { get; private set; }

        public IProductRepository ProductLookup { get; private set; }

        public IPricingProfileRepository PricingProfiles { get; private set; }

        public IQuotationRepository Quotations { get; private set; }
        public IQuotationDetailRepository QuotationDetails { get; private set; }

        public IDashboardTemplateRepository DashboardTemplates { get; private set; }

        public IFarmActivityPhotoRepository FarmActivityPhotos { get; private set; }

        public IFarmGeneralExpenseRepository FarmGeneralExpenses { get; private set; }

        public IFarmKpiSummaryRepository FarmKpiSummaries { get; private set; }

        public IWeatherForecastRepository WeatherForecasts { get; private set; }

        public IFarmContractPaymentRepository FarmContractPayments { get; private set; }
        public IFarmNumberSeryRepository FarmNumberSeries { get; private set; }

        public UnitOfWork(UmanhanDbContext context)
        {
            _context = context;

            Categories = new CategoryRepository(_context);
            ChangeLogs = new ChangeLogRepository(_context);
            Crops = new CropRepository(_context);
            Customers = new CustomerRepository(_context);
            CustomerTypes = new CustomerTypeRepository(_context);
            ExpenseTypes = new ExpenseTypeRepository(_context);
            FarmActivities = new FarmActivityRepository(_context);
            FarmActivityExpenses = new FarmActivityExpenseRepository(_context);
            FarmActivityLaborers = new FarmActivityLaborerRepository(_context);
            FarmActivityUsages = new FarmActivityUsageRepository(_context);
            FarmContracts = new FarmContractRepository(_context);
            FarmContractDetails = new FarmContractDetailRepository(_context);
            FarmContractSales = new FarmContractSaleRepository(_context);
            FarmCrops = new FarmCropRepository(_context);
            FarmInventories = new FarmInventoryRepository(_context);
            FarmLivestocks = new FarmLivestockRepository(_context);
            FarmProduceInventories = new FarmProduceInventoryRepository(_context);
            Farms = new FarmRepository(_context);
            FarmTransactions = new FarmTransactionRepository(_context);
            FarmZones = new FarmZoneRepository(_context);
            FarmZoneYields = new FarmZoneYieldRepository(_context);
            Inventories = new InventoryRepository(_context);
            Laborers = new LaborerRepository(_context);
            Livestocks = new LivestockRepository(_context);
            PaymentTypes = new PaymentTypeRepository(_context);
            ProductTypes = new ProductTypeRepository(_context);
            SoilTypes = new SoilTypeRepository(_context);
            Staffs = new StaffRepository(_context);
            SystemSettings = new SystemSettingRepository(_context);
            Tasks = new TaskRepository(_context);
            TransactionTypes = new TransactionTypeRepository(_context);
            Units = new UnitRepository(_context);

            Roles = new RoleRepository(_context);
            Modules = new ModuleRepository(_context);
            Permissions = new PermissionRepository(_context);
            RolePermissions = new RolePermissionRepository(_context);

            Reports = new ReportRepository(_context);
            PricingConditions = new PricingConditionRepository(_context);
            PricingConditionTypes = new PricingConditionTypeRepository(_context);

            ProductLookup = new ProductRepository(_context);

            PricingProfiles = new PricingProfileRepository(_context);

            Quotations = new QuotationRepository(_context);
            QuotationDetails = new QuotationDetailRepository(_context);

            DashboardTemplates = new DashboardTemplateRepository(_context);

            FarmActivityPhotos = new FarmActivityPhotoRepository(_context);

            FarmGeneralExpenses = new FarmGeneralExpenseRepository(_context);
            FarmKpiSummaries = new FarmKpiSummaryRepository(_context);
            FarmKpiSummaries = new FarmKpiSummaryRepository(_context);
            WeatherForecasts = new WeatherForecastRepository(_context);

            FarmContractPayments = new FarmContractPaymentRepository(_context);
            FarmNumberSeries = new FarmNumberSeryRepository(_context);
        }

        public async Task<int> CommitAsync()
        {
            return await CommitAsync(null).ConfigureAwait(false);
        }

        public async Task<int> CommitAsync(string username)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                    LogChanges(username);

                return await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.InnerException as PostgresException;
                if (sqlException != null)
                {
                    switch (sqlException.SqlState)
                    {
                        case "23505":
                            throw new DbUpdateException("Record already exists.");

                        case "42703":
                        case "42P18":
                            throw new DbUpdateException("Invalid column reference.");

                        case "23503":
                            throw new DbUpdateException("Foreign key constraint violated.");

                        case "42P01":
                            throw new DbUpdateException("Table not found.");

                        case "23514":
                            throw new DbUpdateException("Check constraint violation.");

                        default:
                            throw new DbUpdateException("Unknown database error.");
                    }
                }
                throw new DbUpdateException("Unexpected error. Please try again later.");
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong.");
            }
        }

        private void LogChanges(string username)
        {
            // 1. Grab all pending changes
            var now = DateTime.UtcNow;
            var entries = _context.ChangeTracker
                                  .Entries()
                                  .Where(e => e.State == EntityState.Added ||
                                              e.State == EntityState.Modified ||
                                              e.State == EntityState.Deleted);

            var logs = new List<ChangeLog>();

            foreach (var entry in entries)
            {
                var pkProp = entry.Metadata.FindPrimaryKey()
                                    .Properties
                                    .FirstOrDefault();
                string keyName = pkProp?.Name;
                var keyProperty = entry.Property(keyName);
                string entityId = entry.State == EntityState.Deleted
                                        ? keyProperty.OriginalValue?.ToString()
                                        : keyProperty.CurrentValue?.ToString();

                string entityName = entry.Entity.GetType().Name;

                // exclude navigation properties
                var simpleSnapshot = entry.Properties
                    .Where(p => !(p.Metadata is INavigationBase))
                    .ToDictionary(
                        p => p.Metadata.Name,
                        p => p.CurrentValue
                    );

                if (entry.State == EntityState.Added)
                {
                    string json = JsonSerializer.Serialize(simpleSnapshot, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                    logs.Add(new ChangeLog
                    {
                        Timestamp = now,
                        Username = username,
                        Type = "ADDED",
                        Entity = entityName,
                        EntityId = entityId,
                        Field = "__EntireObject",
                        OldValue = null,
                        NewValue = json
                    });
                    continue;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    string json = JsonSerializer.Serialize(simpleSnapshot, new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                    // On delete: oldVal = actual, newVal = null
                    logs.Add(new ChangeLog
                    {

                        Timestamp = now,
                        Username = username,
                        Type = "DELETED",
                        Entity = entityName,
                        EntityId = entityId,
                        Field = "__EntireObject",
                        OldValue = json,
                        NewValue = null
                    });
                    continue;
                }

                foreach (var prop in entry.Properties)
                {
                    string fieldName = prop.Metadata.Name;
                    string oldVal = prop.OriginalValue?.ToString();
                    string newVal = prop.CurrentValue?.ToString();

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // On update: skip unchanged props and null↔empty noise
                            if (!prop.IsModified || oldVal == newVal)
                                continue;

                            logs.Add(new ChangeLog
                            {
                                Timestamp = now,
                                Username = username,
                                Type = "MODIFIED",
                                Entity = entityName,
                                EntityId = entityId,
                                Field = fieldName,
                                OldValue = oldVal,
                                NewValue = newVal
                            });
                            break;
                    }
                }
            }

            // 2. Add the logs to the context
            if (logs.Count != 0)
                _context.ChangeLogs.AddRange(logs);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
