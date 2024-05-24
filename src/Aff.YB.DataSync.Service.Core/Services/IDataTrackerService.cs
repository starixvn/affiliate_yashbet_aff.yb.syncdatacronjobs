using Aff.YB.DataSync.Service.Core.Domain.DTOs;

namespace Aff.YB.DataSync.Service.Core.Services
{
    public interface IDataTrackerService
    {
        Task<IEnumerable<DataTrackerItem>> FetchTrackerAsync(IEnumerable<string> prefixProviders, IEnumerable<int> dataTypes, IEnumerable<int> statuses);

        Task UpdateTrackerStatusAsync(DataTrackerItem item, int status);
    }
}
