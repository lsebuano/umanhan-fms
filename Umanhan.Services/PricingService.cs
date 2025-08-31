using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Services
{
    public class PricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<PricingService> _logger;

        private IEnumerable<PricingCondition> ConfigureProcedure(IEnumerable<PricingCondition> conditions)
        {
            // 1) Fetch classification from each condition
            static bool IsDeduction(PricingCondition c) => c.ConditionType.IsDeduction;

            // 2) Sort:
            //    a) deductions first, then additions
            //    b) within each: percentage before fixed
            //    c) tie-breaker: by ConditionType.Name
            var ordered = conditions.OrderByDescending(c => IsDeduction(c))
                                    .ThenBy(c => c.ApplyType.Equals("PERCENTAGE", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                                    .ThenBy(c => c.ConditionType.Name)
                                    .ToList();

            // 3) Assign sequence
            for (int i = 0; i < ordered.Count; i++)
                ordered[i].Sequence = i + 1;

            return ordered;
        }

        private async Task AutoConfigPricingProcedureAsync(Guid profileId)
        {
            var conditions = await _unitOfWork.PricingConditions.GetPricingsByProfileIdAsync(profileId, "ConditionType").ConfigureAwait(false);

            var ordered = ConfigureProcedure(conditions);
            await _unitOfWork.PricingConditions.UpdateBatchAsync(ordered).ConfigureAwait(false);
            await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);
        }

        private static List<PricingDto> ToPricingDto(IEnumerable<PricingCondition> pricings)
        {
            return [.. pricings.Select(x => new PricingDto
            {
                PricingId = x.Id,
                ApplyType = x.ApplyType,
                ConditionTypeId = x.ConditionTypeId,
                ConditionType = x.ConditionType?.Name,
                ConditionIsDeduction = x.ConditionType?.IsDeduction ?? false,
                FarmId = x.PricingProfile.FarmId,
                FarmName = x.PricingProfile.Farm.FarmName,
                Sequence = x.Sequence,
                Value = x.Value,
                ProfileId = x.ProfileId,
            })
            .OrderBy(x => x.FarmName)
            .ThenBy(x => x.Sequence)];
        }

        private static PricingDto ToPricingDto(PricingCondition pricing)
        {
            return new PricingDto
            {
                PricingId = pricing.Id,
                ApplyType = pricing.ApplyType,
                ConditionTypeId = pricing.ConditionTypeId,
                ConditionType = pricing.ConditionType?.Name,
                ConditionIsDeduction = pricing.ConditionType?.IsDeduction ?? false,
                FarmId = pricing.PricingProfile.FarmId,
                FarmName = pricing.PricingProfile.Farm.FarmName,
                Sequence = pricing.Sequence,
                Value = pricing.Value,
                ProfileId = pricing.ProfileId,
            };
        }

        public PricingService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<PricingService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<PricingDto>> GetAllPricingsAsync()
        {
            var list = await _unitOfWork.PricingConditions.GetAllAsync("ConditionType", "PricingProfile.Farm").ConfigureAwait(false);
            return ToPricingDto(list);
        }

        public async Task<IEnumerable<PricingDto>> GetPricingsByFarmIdAsync(Guid farmId)
        {
            var list = await _unitOfWork.PricingConditions.GetPricingsByFarmIdAsync(farmId, "ConditionType", "PricingProfile.Farm").ConfigureAwait(false);
            return ToPricingDto(list);
        }

        public async Task<PricingDto> GetPricingByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.PricingConditions.GetByIdAsync(id, "ConditionType", "PricingProfile.Farm").ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToPricingDto(obj);
        }

        public async Task<PricingDto> CreatePricingAsync(PricingDto pricing)
        {
            var newPricing = new PricingCondition
            {
                ApplyType = pricing.ApplyType,
                ConditionTypeId = pricing.ConditionTypeId,
                Sequence = pricing.Sequence,
                Value = pricing.Value,
                ProfileId = pricing.ProfileId,
            };

            try
            {
                var createdPricing = await _unitOfWork.PricingConditions.AddAsync(newPricing).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                await AutoConfigPricingProcedureAsync(pricing.ProfileId).ConfigureAwait(false);

                var newObj = await _unitOfWork.PricingConditions.GetByIdAsync(createdPricing.Id, "ConditionType", "PricingProfile.Farm").ConfigureAwait(false);
                return ToPricingDto(newObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pricing: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<PricingDto> UpdatePricingAsync(PricingDto pricing)
        {
            var pricingEntity = await _unitOfWork.PricingConditions.GetByIdAsync(pricing.PricingId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Pricing not found.");
            pricingEntity.ApplyType = pricing.ApplyType;
            pricingEntity.ConditionTypeId = pricing.ConditionTypeId;
            pricingEntity.Sequence = pricing.Sequence;
            pricingEntity.Value = pricing.Value;
            pricingEntity.ProfileId = pricing.ProfileId;

            try
            {
                var updatedPricing = await _unitOfWork.PricingConditions.UpdateAsync(pricingEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                await AutoConfigPricingProcedureAsync(pricing.ProfileId).ConfigureAwait(false);

                var existingObj = await _unitOfWork.PricingConditions.GetByIdAsync(pricing.PricingId, "ConditionType", "PricingProfile.Farm").ConfigureAwait(false);
                return ToPricingDto(existingObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pricing: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<PricingDto> DeletePricingAsync(Guid id)
        {
            var pricingEntity = await _unitOfWork.PricingConditions.GetByIdAsync(id, "PricingProfile.Farm").ConfigureAwait(false);
            if (pricingEntity == null)
                return null;

            try
            {
                var deletedPricing = await _unitOfWork.PricingConditions.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                await AutoConfigPricingProcedureAsync(pricingEntity.ProfileId).ConfigureAwait(false);

                return ToPricingDto(new PricingCondition());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<PricingResult> CalculateFinalPriceAsync(Guid profileId, decimal basePrice)
        {
            var breakdown = new List<PricingDto>();
            decimal running = basePrice;

            var conditions = await _unitOfWork.PricingConditions.GetPricingsByProfileIdAsync(profileId, "ConditionType").ConfigureAwait(false);
            foreach (var condition in conditions)
            {
                decimal delta = 0;
                decimal before = running;
                bool isDeduction = condition.ConditionType.IsDeduction;
                decimal value = condition.Value;
                string applyType = condition.ApplyType;
                Guid id = condition.Id;

                if (applyType.Equals("PERCENTAGE", StringComparison.OrdinalIgnoreCase))
                {
                    delta = (running * (value / 100));
                    delta = isDeduction ? -delta
                                        : +delta;
                }
                else if (applyType.Equals("FIXED_AMOUNT", StringComparison.OrdinalIgnoreCase))
                {
                    delta = isDeduction ? -value : +value;
                }
                else
                {
                    //TODO:
                    Console.WriteLine($"Unknown ApplyType {applyType} on condition {id}");
                }

                running += delta;

                breakdown.Add(new PricingDto
                {
                    Sequence = condition.Sequence,
                    ConditionType = condition.ConditionType.Name,
                    ApplyType = condition.ApplyType,
                    Value = condition.Value,
                    ConditionIsDeduction = condition.ConditionType.IsDeduction,
                    Before = before,
                    Delta = running,
                    After = running
                });
            }

            return new PricingResult
            {
                BasePrice = basePrice,
                FinalPrice = running,
                Breakdown = breakdown
            };
        }
    }
}
