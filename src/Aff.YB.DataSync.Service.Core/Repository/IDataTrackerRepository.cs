using Aff.YB.DataSync.Service.Core.Domain;

namespace Aff.YB.DataSync.Service.Core.Repository
{
    public interface IDataTrackerRepository
    {
        Task UpdateDataTrackerAsync(IDataTrackerItem command);

        Task<IEnumerable<IDataTrackerItem>> FetchDataTracker(IDataTrackerItem criteria);
    }
}
