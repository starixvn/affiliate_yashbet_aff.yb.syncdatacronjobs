using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Helpers;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Log
{
    public class FetchRebateMLogDataTask : FetchDataForMultipleRepositoryTaskBase<RebateMLogModel>
    {
        private ILicenseeApi Api { get; }
        private ILogger Logger { get; }
        private ITrackerService TrackerService { get; }

        private const int API_SERVER_TIME_OFFSET = 0;

        public FetchRebateMLogDataTask(ILicenseeApi licenseeApi,
            IThirdPartyImportRepository _thirdPartyRepo,
            ILogger logger,
            ITrackerService timeTrackerService,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_REBATEMLOG_SETTING))
        {
            Api = licenseeApi;
            Logger = logger;
            TrackerService = timeTrackerService;
        }

        public override async Task FetchDataAsync()
        {
            var tracker = await TrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = DateTime.Parse(tracker.KeyValue);
            var toDate = GetAdjustmentTime();
            await FetchDataAndTrackTimeAsync(fromDate, toDate);
        }

        private async Task FetchDataAndTrackTimeAsync(DateTime fromDate, DateTime toDate)
        {
            var intervalTime = GetApiMaxTimeRange();
            var timeRanges = DateTimeHelper.GetProportionTimeRange(fromDate, toDate, intervalTime);
            foreach (var range in timeRanges)
            {
                try
                {
                    await FetchAndInsertDataAsync(range.FromDate, range.ToDate);
                    var tracker = new TrackerInfo { CalledTime = DateTime.Now, KeyValue = range.ToDate.ToString() };
                    await TrackerService.SaveTrackerAsync(TableName, tracker);
                }
                catch
                {
                    Logger.LogError($"Excetion when fetch {TableName} from {range.FromDate} to {range.ToDate}");
                    throw;
                }
            }
        }

        private async Task FetchAndInsertDataAsync(DateTime fromDate, DateTime toDate)
        {
            var datas = await Api.GetRebateMLogs(fromDate, toDate);
            await ImportDataToMultipleRepoAsync(datas);
        }

        private DateTime GetAdjustmentTime() => GetAdjustmentApiServerDate(new TimeSpan(0, Settings.AdjustmentValue, 0));

        protected override string BuildXml(IEnumerable<RebateMLogModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        private TimeSpan GetApiMaxTimeRange() => TimeSpan.FromMinutes(Settings.MaxTimeRange);

        private DateTime GetAdjustmentApiServerDate(TimeSpan ajustmentTime)
        {
            var date = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(API_SERVER_TIME_OFFSET));
            var adjustDate = date.Add(ajustmentTime);
            return adjustDate.DateTime;
        }
    }
}