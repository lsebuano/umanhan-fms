using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Entities;

namespace Umanhan.Repositories.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        ICategoryRepository Categories { get; }
        IChangeLogRepository ChangeLogs { get; }
        ICropRepository Crops { get; }
        ICustomerRepository Customers { get; }
        ICustomerTypeRepository CustomerTypes { get; }
        IExpenseTypeRepository ExpenseTypes { get; }
        IFarmRepository Farms { get; }
        IFarmActivityRepository FarmActivities { get; }
        IFarmActivityExpenseRepository FarmActivityExpenses { get; }
        IFarmActivityLaborerRepository FarmActivityLaborers { get; }
        IFarmActivityUsageRepository FarmActivityUsages { get;}
        IFarmContractRepository FarmContracts { get; }
        IFarmContractDetailRepository FarmContractDetails { get; }
        IFarmContractSaleRepository FarmContractSales { get; }
        IFarmCropRepository FarmCrops { get; }
        IFarmInventoryRepository FarmInventories { get; }
        IFarmLivestockRepository FarmLivestocks { get; }
        IFarmProduceInventoryRepository FarmProduceInventories { get; }
        IFarmTransactionRepository FarmTransactions { get; }
        IFarmZoneRepository FarmZones { get; }
        IFarmZoneYieldRepository FarmZoneYields { get; }
        IInventoryRepository Inventories { get; }
        ILaborerRepository Laborers { get; }
        ILivestockRepository Livestocks { get; }
        IPaymentTypeRepository PaymentTypes { get; }
        IProductRepository ProductLookup { get; }
        IProductTypeRepository ProductTypes { get; }
        ISoilTypeRepository SoilTypes { get; }
        IStaffRepository Staffs { get; }
        ISystemSettingRepository SystemSettings { get; }
        ITaskRepository Tasks { get; }
        ITransactionTypeRepository TransactionTypes { get; }
        IUnitRepository Units { get; }

        IRoleRepository Roles { get; }
        IModuleRepository Modules { get; }
        IPermissionRepository Permissions { get; }
        IRolePermissionRepository RolePermissions { get; }

        IReportRepository Reports { get; }
        IPricingConditionRepository PricingConditions { get; }
        IPricingConditionTypeRepository PricingConditionTypes { get; }
        IPricingProfileRepository PricingProfiles { get; }

        IQuotationRepository Quotations { get; }
        IQuotationDetailRepository QuotationDetails { get; }

        IDashboardTemplateRepository DashboardTemplates { get; }

        IFarmActivityPhotoRepository FarmActivityPhotos { get; }

        IFarmGeneralExpenseRepository FarmGeneralExpenses { get; }
        IFarmKpiSummaryRepository FarmKpiSummaries { get; }
        IWeatherForecastRepository WeatherForecasts { get; }

        IFarmContractPaymentRepository FarmContractPayments { get; }
        IFarmNumberSeryRepository FarmNumberSeries { get; }

        Task<int> CommitAsync();
        Task<int> CommitAsync(string username);
    }
}
