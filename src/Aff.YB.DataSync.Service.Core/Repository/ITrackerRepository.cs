using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;

namespace Aff.YB.DataSync.Service.Core.Repository
{
    public interface ITrackerRepository
    {
        Task<TrackerInfo> GetLastedTrackerAsync(ITrackerSource timeInfo);

        Task ImportTimeTrackerAsync(IImportTimeTracker timeInfo);
    }
}
