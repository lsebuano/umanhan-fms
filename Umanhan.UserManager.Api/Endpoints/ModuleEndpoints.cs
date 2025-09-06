using FluentValidation;
using Umanhan.Dtos;
using Umanhan.Services;

namespace Umanhan.UserManager.Api.Endpoints
{
    public class ModuleEndpoints
    {
        private readonly ModuleService _moduleService;
        private readonly IValidator<ModuleDto> _validator;
        private readonly ILogger<ModuleEndpoints> _logger;
        //private readonly ICacheService _cacheService;

        private const string MODULE_CACHE_KEY = "module";

        public ModuleEndpoints(ModuleService moduleService, 
            ILogger<ModuleEndpoints> logger
            )
        {
            _moduleService = moduleService;
            _logger = logger;
            //_cacheService = cacheService;
        }

        public async Task<IResult> GetAllModulesAsync()
        {
            try
            {
                var key = $"{MODULE_CACHE_KEY}:list";
                //var result = await _cacheService.GetOrSetAsync(key, async () =>
                //{   
                var result = await _moduleService.GetAllModulesAsync().ConfigureAwait(false);
                //    return result;
                //});
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all modules");
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var farmModule = await _moduleService.GetModuleByIdAsync(id).ConfigureAwait(false);
                return Results.Ok(farmModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching module by ID {Id}", id);
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
                //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                return Results.Created($"/api/user-mgr/modules/{newModule.ModuleId}", newModule);
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
                return Results.ValidationProblem(validationResult.ToDictionary());

            try
            {
                var updatedModule = await _moduleService.UpdateModuleAsync(module).ConfigureAwait(false);
                if (updatedModule is not null)
                {
                    // Clear cache for the list of modules
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                    return Results.Ok(updatedModule);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module with ID {Id}", id);
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
                    // Clear cache for the list of modules
                    //_ = _cacheService.RemoveAsync($"{MODULE_CACHE_KEY}:list").ConfigureAwait(false);
                    return Results.Ok(deletedModule);
                }
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module with ID {Id}", id);
                return Results.Problem(ex.Message);
            }
        }
    }
}
