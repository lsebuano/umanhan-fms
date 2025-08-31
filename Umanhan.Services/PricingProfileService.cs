using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Models.Models;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Umanhan.Services
{
    public class PricingProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<PricingProfileService> _logger;

        private static List<PricingProfileDto> ToPricingProfileDto(IEnumerable<PricingProfile> profiles)
        {
            return [.. profiles.Select(x => new PricingProfileDto
            {
                ProfileId = x.Id,
                FarmId = x.FarmId,
                FarmName = x.Farm.FarmName,
                Name = x.Name,
                Description = x.Description,
                FinalPrice = x.FinalPrice,
                PricingConditions = x.PricingConditions.Select(c => new PricingDto {
                    FarmId = x.FarmId,
                    ApplyType = c.ApplyType,
                    PricingId = c.Id,
                    ConditionTypeId = c.ConditionTypeId,
                    ProfileId = c.ProfileId,
                    Value = c.Value,
                    Sequence = c.Sequence,
                    ConditionType = c.ConditionType.Name,
                    ConditionIsDeduction = c.ConditionType.IsDeduction,
                })
                .OrderBy(c => c.Sequence)
            })
            .OrderBy(x => x.Name)];
        }

        private static PricingProfileDto ToPricingProfileDto(PricingProfile profile)
        {
            return new PricingProfileDto
            {
                ProfileId = profile.Id,
                FarmId = profile.FarmId,
                FarmName = profile.Farm.FarmName,
                Name = profile.Name,
                Description = profile.Description,
                FinalPrice = profile.FinalPrice,
                PricingConditions = profile.PricingConditions.Select(c => new PricingDto
                {
                    FarmId = profile.FarmId,
                    ApplyType = c.ApplyType,
                    PricingId = c.Id,
                    ConditionTypeId = c.ConditionTypeId,
                    ProfileId = c.ProfileId,
                    Value = c.Value,
                    Sequence = c.Sequence,
                    ConditionType = c.ConditionType.Name,
                    ConditionIsDeduction = c.ConditionType.IsDeduction
                })
                .OrderBy(c => c.Sequence)
            };
        }

        public PricingProfileService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<PricingProfileService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<PricingProfileDto>> GetAllPricingsAsync()
        {
            var list = await _unitOfWork.PricingProfiles.GetAllAsync().ConfigureAwait(false);
            return ToPricingProfileDto(list);
        }

        public async Task<IEnumerable<PricingProfileDto>> GetPricingProfilesByFarmIdAsync(Guid farmId)
        {
            var list = await _unitOfWork.PricingProfiles.GetPricingProfilesByFarmIdAsync(farmId, "Farm", "PricingConditions.ConditionType").ConfigureAwait(false);
            return ToPricingProfileDto(list);
        }

        public async Task<PricingProfileDto> GetPricingProfileByIdAsync(Guid id)
        {
            var obj = await _unitOfWork.PricingProfiles.GetByIdAsync(id, "Farm", "PricingConditions.ConditionType").ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToPricingProfileDto(obj);
        }

        public async Task<PricingProfileDto> CreatePricingProfileAsync(PricingProfileDto pricing)
        {
            var newProfile = new PricingProfile
            {
                FarmId = pricing.FarmId,
                Name = pricing.Name,
                Description = pricing.Description,
                FinalPrice = pricing.FinalPrice,
            };

            try
            {
                var createdProfile = await _unitOfWork.PricingProfiles.AddAsync(newProfile).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPricingProfileDto(createdProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pricing profile");
                throw;
            }
        }

        public async Task<PricingProfileDto> UpdatePricingProfileAsync(PricingProfileDto pricing)
        {
            var profileEntity = await _unitOfWork.PricingProfiles.GetByIdAsync(pricing.ProfileId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Pricing Profile not found.");
            profileEntity.Name = pricing.Name;
            profileEntity.Description = pricing.Description;
            profileEntity.FarmId = pricing.FarmId;
            profileEntity.FinalPrice = pricing.FinalPrice;

            try
            {
                var updatedProfile = await _unitOfWork.PricingProfiles.UpdateAsync(profileEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPricingProfileDto(updatedProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pricing profile");
                throw;
            }
        }

        public async Task<PricingProfileDto> DeletePricingProfileAsync(Guid id)
        {
            var profileEntity = await _unitOfWork.PricingProfiles.GetByIdAsync(id).ConfigureAwait(false);
            if (profileEntity == null)
                return null;

            try
            {
                var deletedProfile = await _unitOfWork.PricingProfiles.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToPricingProfileDto(new PricingProfile());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing profile: {Message}", ex.Message);
                throw;
            }
        }
    }
}
