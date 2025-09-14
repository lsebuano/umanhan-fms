using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmGeneralExpenseReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmGeneralExpenseReceiptService> _logger;

        private static List<FarmGeneralExpenseReceiptDto> ToFarmGeneralExpenseReceiptDto(IEnumerable<FarmGeneralExpenseReceipt> farmGeneralExpenseReceipts)
        {
            return [.. farmGeneralExpenseReceipts.Select(x => new FarmGeneralExpenseReceiptDto
            {
                ReceiptId = x.Id,
                GeneralExpenseId = x.GeneralExpenseId,
                MimeType = x.MimeType,
                Notes = x.Notes,
                ReceiptUrlFull = x.ReceiptUrlFull,
                ReceiptUrlThumbnail = x.ReceiptUrlThumbnail,
                Timestamp = x.Timestamp
            })];
        }

        private static FarmGeneralExpenseReceiptDto ToFarmGeneralExpenseReceiptDto(FarmGeneralExpenseReceipt farmGeneralExpenseReceipt)
        {
            return new FarmGeneralExpenseReceiptDto
            {
                ReceiptId = farmGeneralExpenseReceipt.Id,
                GeneralExpenseId = farmGeneralExpenseReceipt.GeneralExpenseId,
                MimeType = farmGeneralExpenseReceipt.MimeType,
                Notes = farmGeneralExpenseReceipt.Notes,
                ReceiptUrlFull = farmGeneralExpenseReceipt.ReceiptUrlFull,
                ReceiptUrlThumbnail = farmGeneralExpenseReceipt.ReceiptUrlThumbnail,
                Timestamp = farmGeneralExpenseReceipt.Timestamp
            };
        }

        public FarmGeneralExpenseReceiptService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmGeneralExpenseReceiptService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<FarmGeneralExpenseReceiptDto>> GetFarmGeneralExpenseReceiptsByGeneralExpenseAsync(Guid generalExpenseId, params string[] includeEntities)
        {
            var list = await _unitOfWork.FarmGeneralExpenseReceipts.GetFarmGeneralExpenseReceiptsByGeneralExpenseAsync(generalExpenseId, includeEntities).ConfigureAwait(false);
            return ToFarmGeneralExpenseReceiptDto(list);
        }

        public async Task<FarmGeneralExpenseReceiptDto> GetFarmGeneralExpenseReceiptByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.FarmGeneralExpenseReceipts.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToFarmGeneralExpenseReceiptDto(obj);
        }

        public async Task<FarmGeneralExpenseReceiptDto> CreateFarmGeneralExpenseReceiptAsync(FarmGeneralExpenseReceiptDto farmGeneralExpenseReceipt)
        {
            var newFarmGeneralExpenseReceipt = new FarmGeneralExpenseReceipt
            {
                GeneralExpenseId = farmGeneralExpenseReceipt.GeneralExpenseId,
                MimeType = farmGeneralExpenseReceipt.MimeType,
                Notes = farmGeneralExpenseReceipt.Notes,
                ReceiptUrlFull = farmGeneralExpenseReceipt.ReceiptUrlFull,
                ReceiptUrlThumbnail = farmGeneralExpenseReceipt.ReceiptUrlThumbnail,
                Timestamp = DateTime.Now
            };

            try
            {
                // Create the farm activity usage
                var createdFarmGeneralExpenseReceipt = await _unitOfWork.FarmGeneralExpenseReceipts.AddAsync(newFarmGeneralExpenseReceipt).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseReceiptDto(createdFarmGeneralExpenseReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Farm General Expense Receipt: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmGeneralExpenseReceiptDto> UpdateFarmGeneralExpenseReceiptAsync(FarmGeneralExpenseReceiptDto farmGeneralExpenseReceipt)
        {
            var farmGeneralExpenseReceiptEntity = await _unitOfWork.FarmGeneralExpenseReceipts.GetByIdAsync(farmGeneralExpenseReceipt.ReceiptId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Farm General Expense Receipt not found.");
            farmGeneralExpenseReceiptEntity.GeneralExpenseId = farmGeneralExpenseReceipt.GeneralExpenseId;
            farmGeneralExpenseReceiptEntity.MimeType = farmGeneralExpenseReceipt.MimeType;
            farmGeneralExpenseReceiptEntity.Notes = farmGeneralExpenseReceipt.Notes;
            farmGeneralExpenseReceiptEntity.ReceiptUrlFull = farmGeneralExpenseReceipt.ReceiptUrlFull;
            farmGeneralExpenseReceiptEntity.ReceiptUrlThumbnail = farmGeneralExpenseReceipt.ReceiptUrlThumbnail;
            farmGeneralExpenseReceiptEntity.Timestamp = DateTime.Now;

            try
            {
                var updatedFarmGeneralExpenseReceipt = await _unitOfWork.FarmGeneralExpenseReceipts.UpdateAsync(farmGeneralExpenseReceiptEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseReceiptDto(updatedFarmGeneralExpenseReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Farm General Expense Receipt: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FarmGeneralExpenseReceiptDto> DeleteFarmGeneralExpenseReceiptAsync(Guid id)
        {
            var farmGeneralExpenseReceiptEntity = await _unitOfWork.FarmGeneralExpenseReceipts.GetByIdAsync(id).ConfigureAwait(false);
            if (farmGeneralExpenseReceiptEntity == null)
                return null;

            try
            {
                var deletedFarmGeneralExpenseReceipt = await _unitOfWork.FarmGeneralExpenseReceipts.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToFarmGeneralExpenseReceiptDto(new FarmGeneralExpenseReceipt());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Farm General Expense Receipt: {Message}", ex.Message);
                throw;
            }
        }
    }
}
