using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umanhan.Models.Dtos;

namespace Umanhan.Services.Interfaces
{
    public interface IChangeLogService
    {
        Task<IEnumerable<ChangeLogDto>> GetChangeLogsAsync(DateTime date);
        Task<ChangeLogDto> GetChangeLogByIdAsync(Guid id);
    }
}
