using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmActivityService> _logger;

        private static List<FarmActivityDto> ToFarmActivityDto(IEnumerable<FarmActivity> farmActivities)
        {
            return [.. farmActivities.Select(x => new FarmActivityDto
            {
                FarmId = x.FarmId,
                FarmName = x.Farm?.FarmName,
                ActivityId = x.Id,
                ContractId = x.ContractId,
                EndDateTime = x.EndDate,
                Notes = x.Notes,
                ProductId = x.ProductId,
                ProductType = x.ProductType?.ProductTypeName,
                ProductTypeId = x.ProductTypeId,
                StartDateTime = x.StartDate,
                Status = x.Status,
                Supervisor = x.Supervisor?.Name,
                SupervisorId = x.SupervisorId,
                Task = x.Task?.TaskName,
                TaskId = x.TaskId
            })];
        }

        private static FarmActivityDto ToFarmActivityDto(FarmActivity farmActivity)
        {
            return new FarmActivityDto
            {
                FarmId = farmActivity.FarmId,
                FarmName = farmActivity.Farm?.FarmName,
                ActivityId = farmActivity.Id,
                ContractId = farmActivity.ContractId,
                EndDateTime = farmActivity.EndDate,
                Notes = farmActivity.Notes,
                ProductId = farmActivity.ProductId,                
                ProductType = farmActivity.ProductType?.ProductTypeName,
                ProductTypeId = farmActivity.ProductTypeId,
                StartDateTime = farmActivity.StartDate,
                Status = farmActivity.Status,
                Supervisor = farmActivity.Supervisor?.Name,
                SupervisorId = farmActivity.SupervisorId,
                Task = farmActivity.Task?.TaskName,
                TaskId = farmActivity.TaskId,
            };
        }

        public FarmActivityService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmActivityService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmActivityDto>> GetFarmActivitiesAsync(Guid farmId)
        {
            var list = await _unitOfWork.FarmActivities.GetFarmActivitiesAsync(farmId).ConfigureAwait(false);
            return ToFarmActivityDto(list);
        }

        public async Task<IEnumerable<FarmActivityDto>> GetFarmActivitiesAsync(Guid farmId, DateTime date, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivities.GetFarmActivitiesAsync(farmId, date.Date, includeEntities).ConfigureAwait(false);
            return ToFarmActivityDto(list);
        }

        public async Task<IEnumerable<FarmActivityExpenseDto>> GetFarmActivityExpensesAsync(Guid activityId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmActivities.GetFarmActivityExpensesAsync(activityId, includeEntities).ConfigureAwait(false);
            return list.Select(x => new FarmActivityExpenseDto
            {
                ActivityId = x.ActivityId,
                Amount = x.Amount,
                DateUtc = x.Date,
                Description = x.Description,
                ExpenseId = x.Id,
                ExpenseTypeId = x.ExpenseTypeId,
                ExpenseTypeName = x.ExpenseType?.ExpenseTypeName,
                Task = x.Activity?.Task.TaskName,
                ProductType = x.Activity?.ProductType?.ProductTypeName,
                Supervisor = x.Activity?.Supervisor?.Name,
                ActivityStartDateTime = x.Activity?.StartDate ?? DateTime.MinValue,
                ActivityEndDateTime = x.Activity?.EndDate ?? DateTime.MinValue,
                CustomerAddress = x.Activity?.Contract?.Customer?.Address,
                CustomerContactInfo = x.Activity?.Contract?.Customer?.ContactInfo,
                CustomerName = x.Activity?.Contract?.Customer?.CustomerName,
            });
        }

        public async Task<FarmActivityDto> GetFarmActivityByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmActivities.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmActivityDto(obj);
        }

        public async Task<FarmActivityDto> CreateFarmActivityAsync(FarmActivityDto farmActivity)
        {
            var newFarmActivity = new FarmActivity
            {
                ContractId = farmActivity.ContractId,
                EndDate = farmActivity.EndDateTime == null ? null : DateTime.SpecifyKind(farmActivity.EndDateTime.Value, DateTimeKind.Utc),
                Notes = farmActivity.Notes,
                ProductId = farmActivity.ProductId,
                ProductTypeId = farmActivity.ProductTypeId,
                StartDate = DateTime.SpecifyKind(farmActivity.StartDateTime, DateTimeKind.Utc),
                Status = "NEW", //farmActivity.Status,
                SupervisorId = farmActivity.SupervisorId,
                TaskId = farmActivity.TaskId,
                FarmId = farmActivity.FarmId
            };

            try
            {
                var createdFarmActivity = await _unitOfWork.FarmActivities.AddAsync(newFarmActivity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityDto(createdFarmActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm activity");
                throw;
            }
        }

        public async Task<FarmActivityDto> UpdateFarmActivityAsync(FarmActivityDto farmActivity)
        {
            var farmActivityEntity = await _unitOfWork.FarmActivities.GetByIdAsync(farmActivity.ActivityId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm Activity not found.");
            farmActivityEntity.ContractId = farmActivity.ContractId;
            farmActivityEntity.StartDate = DateTime.SpecifyKind(farmActivity.StartDateTime, DateTimeKind.Utc);
            farmActivityEntity.EndDate = farmActivity.EndDateTime == null ? null : DateTime.SpecifyKind(farmActivity.EndDateTime.Value, DateTimeKind.Utc);
            farmActivityEntity.Notes = farmActivity.Notes;
            farmActivityEntity.ProductId = farmActivity.ProductId;
            farmActivityEntity.ProductTypeId = farmActivity.ProductTypeId;
            farmActivityEntity.Status = farmActivity.Status;
            farmActivityEntity.SupervisorId = farmActivity.SupervisorId;
            farmActivityEntity.TaskId = farmActivity.TaskId;

            try
            {
                var updatedFarmActivity = await _unitOfWork.FarmActivities.UpdateAsync(farmActivityEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityDto(updatedFarmActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm activity: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmActivityDto> DeleteFarmActivityAsync(Guid id)
        {
            var farmActivityEntity = await _unitOfWork.FarmActivities.GetByIdAsync(id).ConfigureAwait(false);
            if (farmActivityEntity == null)
                return null;

            try
            {
                var deletedFarmActivity = await _unitOfWork.FarmActivities.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmActivityDto(new FarmActivity());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm activity: {Message}", ex.Message);
                throw;
            }
        }
    }
}
