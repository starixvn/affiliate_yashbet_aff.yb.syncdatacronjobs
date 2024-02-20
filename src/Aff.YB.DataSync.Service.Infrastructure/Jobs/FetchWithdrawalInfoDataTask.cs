using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs
{

    public class FetchWithdrawalInfoDataTask : FetchDataByBatchForMultiRepoTaskBase<WithdrawalInfoModel>
    {
        private ILicenseeApi Api { get; }
        private ILogger Logger { get; }

        private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        private const int BatchCount = 2000;

        public FetchWithdrawalInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository,
            ITrackerService trackerService,
            ILogger logger,
            ILicenseeApi licenseeApi,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(thirdPartyImportRepository, configuration.Get(Constants.ServiceConfigName.PULL_WITHDRAWAL_INFO), trackerService)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Api = licenseeApi;
        }

        public override async Task FetchDataAsync()
        {
            try
            {
                var queryValue = await GetLastTrackingValueAsync();
                var fromDate = Convert.ToDateTime(queryValue);

                var data = await GetDataAsync(fromDate, BatchCount);

                await ImportDataByBatchAsync(data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        protected override async Task ImportDataByBatchAsync(IEnumerable<WithdrawalInfoModel> data)
        {
            var importData = data;
            while (importData != null && importData.Any())
            {
                await ImportDataToMultipleRepoAsync(importData);
                var trackValue = GetInsertTrackingValue(importData);
                var fromTime = Convert.ToDateTime(trackValue);

                await _timeTrackerService.SaveTrackerAsync(TableName,
                    new TrackerInfo
                    {
                        CalledTime = DateTime.Now,
                        KeyValue = trackValue
                    });

                if (importData.Count() == BatchCount)
                {
                    importData = await GetDataAsync(fromTime, BatchCount);
                }
                else
                {
                    break;
                }
            }
        }

        protected override Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(string trackValue, int batchCount)
        {
            throw new NotImplementedException();
        }

        private Task<IEnumerable<WithdrawalInfoModel>> GetDataAsync(DateTime updateTime, int batchCount)
            => Api.GetWithdrawalInfo(updateTime, batchCount);

        protected override string GetInsertTrackingValue(IEnumerable<WithdrawalInfoModel> data)
        {
            var trackValue = data.Max(d => d.UpdateTime);

            if (trackValue.HasValue)
            {
                return trackValue.Value.ToString();
            }
            throw new ArgumentException($"Can not insert nullable DateTime Tracker Value of table {TableName}");
        }

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue} of table {TableName}")
                : Convert.ToDateTime(tracker.KeyValue);
            var query = GetAdjustmentTime(date);
            return query.ToString(DateTimeFormat);
        }

        private DateTime GetAdjustmentTime(DateTime date) => date.AddMinutes(Settings.AdjustmentValue);
    }
}