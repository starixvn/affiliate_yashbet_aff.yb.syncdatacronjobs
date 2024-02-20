using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.SpParams;
using Aff.YB.DataSync.Service.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff.YB.DataSync.Service.Infrastructure.Repository
{
    public class TrackerRespository : ITrackerRepository
    {
        private readonly IDynamicRepository _repo;

        public TrackerRespository(IDynamicRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<TrackerInfo> GetLastedTrackerAsync(ITrackerSource timeInfo)
            => _repo.GetAsync<TrackerInfo>((GetLastedImportTimeCriteria)timeInfo);


        public Task ImportTimeTrackerAsync(IImportTimeTracker timeInfo)
            => _repo.ExecuteAsync((InsertImportTrackerCommand)timeInfo);
    }
}
