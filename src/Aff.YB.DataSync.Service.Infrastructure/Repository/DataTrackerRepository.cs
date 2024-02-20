


using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff.YB.DataSync.Service.Infrastructure.Repository
{
    public class DataTrackerRepository : IDataTrackerRepository
    {
        private readonly IDynamicRepository _repo;

        public DataTrackerRepository(IDynamicRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<IEnumerable<DataTrackerItem>> FetchDataTracker(IDataTracker criteria)
           => _repo.FetchAsync<DataTrackerItem>((ICriteria)criteria);

        public Task UpdateDataTrackerAsync(IDataTracker command)
            => _repo.ExecuteAsync((UpdateDataTrackerCommand)command);
    }
}
