using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class PricingConditionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<PricingConditionTypeService> _logger;

        private static List<PricingConditionTypeDto> ToPricingConditionTypeDto(IEnumerable<PricingConditionType> pricingConditionTypes)
        {
            return [.. pricingConditionTypes.Select(x => new PricingConditionTypeDto
            {
                ConditionId = x.Id,
                Name = x.Name,
                IsDeduction = x.IsDeduction,
            })
            .OrderBy(x => x.Name)];
        }

        private static PricingConditionTypeDto ToPricingConditionTypeDto(PricingConditionType pricingConditionType)
        {
            return new PricingConditionTypeDto
            {
                ConditionId = pricingConditionType.Id,
                Name = pricingConditionType.Name,
                IsDeduction = pricingConditionType.IsDeduction,
            };
        }

        public PricingConditionTypeService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<PricingConditionTypeService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<PricingConditionTypeDto>> GetAllPricingConditionTypesAsync()
        {
            var list = await _unitOfWork.PricingConditionTypes.GetAllAsync().ConfigureAwait(false);
            return ToPricingConditionTypeDto(list);
        }

        public async Task<PricingConditionTypeDto> GetPricingConditionTypeByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.PricingConditionTypes.GetByIdAsync(id).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToPricingConditionTypeDto(obj);
        }
    }
}
