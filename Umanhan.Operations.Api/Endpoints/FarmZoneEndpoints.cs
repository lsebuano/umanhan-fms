using FluentValidation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;

namespace Umanhan.Operations.Api.Endpoints
{
    public class FarmZoneEndpoints
    {
        private readonly FarmZoneService _farmZoneService;
        private readonly IValidator<FarmZoneDto> _validator;
        private readonly ILogger<FarmZoneEndpoints> _logger;

        public FarmZoneEndpoints(FarmZoneService farmZoneService, IValidator<FarmZoneDto> validator, ILogger<FarmZoneEndpoints> logger)
        {
            _farmZoneService = farmZoneService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IResult> GetAllFarmZonesAsync()
        {
            try
            {
                var farmZones = await _farmZoneService.GetAllFarmZonesAsync("FarmCrops").ConfigureAwait(false);
                return Results.Ok(farmZones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all farm zones");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmZonesByFarmAsync(Guid farmId)
        {
            try
            {
                var farmZones = await _farmZoneService.GetFarmZonesByFarmAsync(farmId, "Farm").ConfigureAwait(false);
                return Results.Ok(farmZones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm zones for farm {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetFarmZoneByIdAsync(Guid id)
        {
            try
            {
                var farmZone = await _farmZoneService.GetFarmZoneByIdAsync(id, "Tasks", "FarmCrops").ConfigureAwait(false);
                return farmZone is not null ? Results.Ok(farmZone) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving farm zone by ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateFarmZoneAsync(FarmZoneDto farmZone)
        {
            var validationResult = await _validator.ValidateAsync(farmZone).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmZone = await _farmZoneService.CreateFarmZoneAsync(farmZone).ConfigureAwait(false);
                return Results.Created($"/api/farm-zones/{newFarmZone.ZoneId}", newFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating farm zone");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmZoneAsync(Guid id, FarmZoneDto farmZone)
        {
            if (id != farmZone.ZoneId)
            {
                return Results.BadRequest("Farm Zone ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmZone).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmZone = await _farmZoneService.UpdateFarmZoneAsync(farmZone).ConfigureAwait(false);
                return updatedFarmZone is not null ? Results.Ok(updatedFarmZone) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm zone with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteFarmZoneAsync(Guid id)
        {
            try
            {
                var deletedFarmZone = await _farmZoneService.DeleteFarmZoneAsync(id).ConfigureAwait(false);
                return deletedFarmZone is not null ? Results.Ok(deletedFarmZone) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting farm zone with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateUpdateFarmZoneBoundaryAsync(FarmZoneDto farmZone)
        {
            var validationResult = await _validator.ValidateAsync(farmZone).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var newFarmZone = await _farmZoneService.CreateUpdateFarmZoneBoundaryAsync(farmZone).ConfigureAwait(false);
                return Results.Created($"/api/farm-zones/boundary", newFarmZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or updating farm zone boundary");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateFarmBoundaryZoneAsync(Guid id, FarmZoneDto farmZone)
        {
            if (id != farmZone.ZoneId)
            {
                return Results.BadRequest("Farm Zone ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(farmZone).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedFarmZone = await _farmZoneService.UpdateFarmZoneBoundaryAsync(farmZone).ConfigureAwait(false);
                return updatedFarmZone is not null ? Results.Ok(updatedFarmZone) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating farm zone boundary with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}