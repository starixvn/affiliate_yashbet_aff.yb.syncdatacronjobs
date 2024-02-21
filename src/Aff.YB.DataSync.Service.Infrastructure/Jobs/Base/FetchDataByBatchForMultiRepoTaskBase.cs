using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Helpers;
using Hangfire;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Base
{
    public abstract class FetchDataByBatchForMultiRepoTaskBase<TData> : FetchDataForMultipleRepositoryTaskBase<TData>
        where TData : class
    {
        protected const int MAX_ROW_COUNT = 2000;

        protected readonly ITrackerService _timeTrackerService;

        protected override int PagingSize => MAX_ROW_COUNT;


        protected FetchDataByBatchForMultiRepoTaskBase(IThirdPartyImportRepository thirdPartyImportRepository, FetchServiceSetting setting, ITrackerService trackerService)
            : base(thirdPartyImportRepository, setting)
        {
            _timeTrackerService = trackerService;
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var queryValue = await GetLastTrackingValueAsync();
            var data = await GetDataAsync(queryValue, PagingSize);

            await ImportDataByBatchAsync(data);
        }

        protected virtual async Task ImportDataByBatchAsync(IEnumerable<TData> data)
        {
            var importData = data;
            while (importData != null && importData.Any())
            {
                await ImportDataToMultipleRepoAsync(importData);
                var trackValue = GetInsertTrackingValue(importData);
                await _timeTrackerService.SaveTrackerAsync(TableName,
                    new TrackerInfo
                    {
                        CalledTime = DateTime.Now,
                        KeyValue = trackValue
                    });

                if (importData.Count() == PagingSize)
                {
                    importData = await GetDataAsync(trackValue, PagingSize);
                }
                else
                {
                    break;
                }
            }
        }

        protected abstract Task<IEnumerable<TData>> GetDataAsync(string trackValue, int batchCount);

        protected abstract string GetInsertTrackingValue(IEnumerable<TData> data);

        protected abstract Task<string> GetLastTrackingValueAsync();

        protected override string BuildXml(IEnumerable<TData> data)
             => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
