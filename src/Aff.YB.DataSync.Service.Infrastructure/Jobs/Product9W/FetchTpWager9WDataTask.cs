using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.DataDomain.Params;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Comparer;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Constants;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Product9W
{
    public class FetchTpWager9WDataTask : FetchDataByBatchForMultiRepoTaskBase<TpWager9WModel>
    {
        private readonly IProduct9WApi _licenseeApi;
        private readonly ILogger _logger;

        public FetchTpWager9WDataTask(IProduct9WApi licenseeApi,
             IThirdPartyImportRepository _thirdPartyRepo,
             ILogger<FetchTpWager9WDataTask> logger,
             ITrackerService timeTrackerService,
             IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_TPWAGER_9W_SETTING), timeTrackerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        protected override async Task<IEnumerable<TpWager9WModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var fromDate = DateTime.Parse(trackValue);
            var toDate = DateTime.Now;
            return await _licenseeApi.GetTpWagerAsync(new BaseParam
            {
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = 0,
                PageSize = batchCount
            });
        }

        protected override string GetInsertTrackingValue(IEnumerable<TpWager9WModel> data)
            => data.Max(d => d.UpdateTime).Value.ToString(Format.DateTimeFormat);

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.KeyValue, new StringToDateTimeComparer());
            var date = string.IsNullOrWhiteSpace(tracker.KeyValue)
                ? throw new ArgumentException($"Not valid DateTime Tracker Value {tracker.KeyValue} of table {TableName}")
                : DateTime.Parse(tracker.KeyValue);
            var adjustDate = GetAdjustmentDate(date);
            return adjustDate.ToString(Format.DateTimeFormat);
        }

        private DateTime GetAdjustmentDate(DateTime date)
            => date.AddMinutes(Settings.AdjustmentValue);
    }
}
