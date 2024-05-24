using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;

namespace Aff.YB.DataSync.Service.Core.Repository
{
    public interface IDataTrackerRepository
    {
        Task UpdateDataTrackerAsync(IDataTracker command);

        Task<IEnumerable<DataTrackerItem>> FetchDataTracker(IDataTracker criteria);
    }
}
