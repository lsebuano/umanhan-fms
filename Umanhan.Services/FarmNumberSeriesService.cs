using Microsoft.Extensions.Logging;
using Umanhan.Repositories.Interfaces;
using Umanhan.Services.Interfaces;

namespace Umanhan.Services
{
    public class FarmNumberSeriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContext;
        private readonly ILogger<FarmNumberSeriesService> _logger;

        public FarmNumberSeriesService(IUnitOfWork unitOfWork, 
            IUserContextService userContext,
            ILogger<FarmNumberSeriesService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._userContext = userContext;
            this._logger = logger;
        }

        public async Task<string> GenerateSeryAsync(Guid farmId, string type)
        {
            try
            {
                return await _unitOfWork.FarmNumberSeries.GenerateNumberSeryAsync(farmId, type).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating number series for farm {FarmId} and type {Type}", farmId, type);
                throw;
            }
        }
    }
}
