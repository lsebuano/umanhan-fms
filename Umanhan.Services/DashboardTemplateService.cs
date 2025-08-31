using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class DashboardTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<DashboardTemplateService> _logger;

        private static List<DashboardTemplateDto> ToDashboardTemplateDto(IEnumerable<DashboardTemplate> dashboardTemplates)
        {
            return [.. dashboardTemplates.Select(x => new DashboardTemplateDto
            {
                TemplateId = x.Id,
                Description = x.Description,
                Name = x.Name,
                DashboardComponentName = x.DashboardComponentName,
            })
            .OrderBy(x => x.Name)];
        }

        private static DashboardTemplateDto ToDashboardTemplateDto(DashboardTemplate dashboardTemplate)
        {
            return new DashboardTemplateDto
            {
                TemplateId = dashboardTemplate.Id,
                Description = dashboardTemplate.Description,
                Name = dashboardTemplate.Name,
                DashboardComponentName = dashboardTemplate.DashboardComponentName,
            };
        }

        public DashboardTemplateService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<DashboardTemplateService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<IEnumerable<DashboardTemplateDto>> GetAllDashboardTemplatesAsync(params string[] includeEntities)
        {
            var list = await _unitOfWork.DashboardTemplates.GetAllAsync(includeEntities).ConfigureAwait(false);
            return ToDashboardTemplateDto(list);
        }

        public async Task<DashboardTemplateDto> GetDashboardTemplateByIdAsync(Guid id, params string[] includeEntities)
        {
            var obj = await _unitOfWork.DashboardTemplates.GetByIdAsync(id, includeEntities).ConfigureAwait(false);
            if (obj == null)
                return null;
            return ToDashboardTemplateDto(obj);
        }
    }
}
