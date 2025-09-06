using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class StaffEndpoints
    {
        private readonly StaffService _staffService;
        private readonly IValidator<StaffDto> _validator;
        private readonly ILogger<StaffEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "staff";
        private const string MODULE_CACHE_TAG = "staff:list:all";

        public StaffEndpoints(StaffService staffService, IValidator<StaffDto> validator, ILogger<StaffEndpoints> logger)
        {
            _staffService = staffService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllStaffsAsync()
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _staffService.GetAllStaffsAsync().ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all staffs");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetStaffsByFarmAsync(Guid farmId)
        {
            try
            {
                string key = $"{MODULE_CACHE_KEY}:list:{farmId}";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _staffService.GetStaffsByFarmAsync(farmId).ConfigureAwait(false);
                //    return result;
                //}, tag: MODULE_CACHE_TAG);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staffs for farm with ID {FarmId}", farmId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetStaffByIdAsync(Guid id)
        {
            try
            {
                var staff = await _staffService.GetStaffByIdAsync(id).ConfigureAwait(false);
                return staff is not null ? Results.Ok(staff) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff with ID {StaffId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateStaffAsync(StaffDto staff)
        {
            var validationResult = await _validator.ValidateAsync(staff).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newStaff = await _staffService.CreateStaffAsync(staff).ConfigureAwait(false);

                //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);

                return Results.Created($"/api/staffs/{newStaff.StaffId}", newStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new staff");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateStaffAsync(Guid id, StaffDto staff)
        {
            if (id != staff.StaffId)
                return Results.BadRequest("Soil Type ID mismatch");

            var validationResult = await _validator.ValidateAsync(staff).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedStaff = await _staffService.UpdateStaffAsync(staff).ConfigureAwait(false);
                if (updatedStaff is not null)
                {
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    return Results.Ok(updatedStaff);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff with ID {StaffId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteStaffAsync(Guid id)
        {
            try
            {
                var deletedStaff = await _staffService.DeleteStaffAsync(id).ConfigureAwait(false);
                if (deletedStaff is not null)
                {
                    //_ = _cacheService.InvalidateTagAsync(MODULE_CACHE_TAG);
                    return Results.Ok(deletedStaff);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff with ID {StaffId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
