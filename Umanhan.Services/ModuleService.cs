using Microsoft.Extensions.Logging;
using Umanhan.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class ModuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<ModuleService> _logger;

        private static List<ModuleDto> ToModuleDto(IEnumerable<Module> modules)
        {
            return [.. modules.Select(x => new ModuleDto
            {
                ModuleId = x.Id,
                Name = x.Name,
            })
            .OrderBy(x => x.Name)];
        }

        private static ModuleDto ToModuleDto(Module module)
        {
            return new ModuleDto
            {
                ModuleId = module.Id,
                Name = module.Name,
            };
        }

        public ModuleService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<ModuleService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.Modules.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToModuleDto(list);
        }

        public async Task<ModuleDto> GetModuleByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.Modules.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToModuleDto(obj);
        }

        public async Task<ModuleDto> CreateModuleAsync(ModuleDto Module)
        {
            var newModule = new Module
            {
                Name = Module.Name
            };

            try
            {
                var createdModule = await _unitOfWork.Modules.AddAsync(newModule).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToModuleDto(createdModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module: {ModuleName}", Module.Name);
                throw;
            }
        }

        public async Task<ModuleDto> UpdateModuleAsync(ModuleDto module)
        {
            var moduleEntity = await _unitOfWork.Modules.GetByIdAsync(module.ModuleId).ConfigureAwait(false) ?? throw new KeyNotFoundException("Module not found.");
            moduleEntity.Name = module.Name;

            try
            {
                var updatedModule = await _unitOfWork.Modules.UpdateAsync(moduleEntity).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToModuleDto(updatedModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module: {ModuleName}", module.Name);
                throw;
            }
        }

        public async Task<ModuleDto> DeleteModuleAsync(Guid id)
        {
            var moduleEntity = await _unitOfWork.Modules.GetByIdAsync(id).ConfigureAwait(false);
            if (moduleEntity == null)
                return null;

            try
            {
                var deletedModule = await _unitOfWork.Modules.DeleteAsync(id).ConfigureAwait(false);
                await _unitOfWork.CommitAsync(_userContext.Username).ConfigureAwait(false);

                return ToModuleDto(new Module());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module with ID: {ModuleId}", id);
                throw;
            }
        }
    }
}
