using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.Masterdata.Api.Endpoints
{
    public class ModuleEndpoints
    {
        private readonly ModuleService _moduleService;
        private readonly IValidator<ModuleDto> _validator;
        private readonly ILogger<ModuleEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "module";

        public ModuleEndpoints(ModuleService moduleService, IValidator<ModuleDto> validator, ILogger<ModuleEndpoints> logger)
        {
            _moduleService = moduleService;
            _validator = validator;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllModulesAsync()
        {
            try
            {
                //string key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{
                var result = await _moduleService.GetAllModulesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modules");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var module = await _moduleService.GetModuleByIdAsync(id).ConfigureAwait(false);
                return module is not null ? Results.Ok(module) : Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving module with ID {ModuleId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateModuleAsync(ModuleDto module)
        {
            var validationResult = await _validator.ValidateAsync(module).ConfigureAwait(false);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var newModule = await _moduleService.CreateModuleAsync(module).ConfigureAwait(false);

                //string key = $"{MODULE_CACHE_KEY}:list";
                //_ = _cacheService.RemoveAsync(key);

                return Results.Created($"/api/modules/{newModule.ModuleId}", newModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> UpdateModuleAsync(Guid id, ModuleDto module)
        {
            if (id != module.ModuleId)
                return Results.BadRequest("Module ID mismatch");

            var validationResult = await _validator.ValidateAsync(module).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var updatedModule = await _moduleService.UpdateModuleAsync(module).ConfigureAwait(false);
                if (updatedModule is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(updatedModule);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module with ID {ModuleId}", id);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DeleteModuleAsync(Guid id)
        {
            try
            {
                var deletedModule = await _moduleService.DeleteModuleAsync(id).ConfigureAwait(false);
                if (deletedModule is not null)
                {
                    //// Clear cache for the list of system settings
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list");
                    return Results.Ok(deletedModule);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module with ID {ModuleId}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}