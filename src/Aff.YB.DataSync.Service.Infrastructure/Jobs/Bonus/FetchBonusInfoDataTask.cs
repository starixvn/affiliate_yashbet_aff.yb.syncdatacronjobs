using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Bonus
{
    public class FetchBonusInfoDataTask : FetchDataByBatchForMultiRepoTaskBase<BonusModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;

        protected override int PagingSize => 500;

        public FetchBonusInfoDataTask(ILicenseeApi licenseeApi,
            IThirdPartyImportRepository _thirdPartyRepo,
            ILogger<FetchBonusInfoDataTask> logger,
            ITrackerService timeTrackerService,
            IOptionsSnapshot<FetchServiceSetting> configuration)
            : base(_thirdPartyRepo, configuration.Get(Constants.ServiceConfigName.PULL_BONUS_INFO_SETTING), timeTrackerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        protected override Task<IEnumerable<BonusModel>> GetDataAsync(string trackValue, int batchCount)
        {
            var id = Convert.ToInt32(trackValue);
            return _licenseeApi.GetBonusInfo(id, batchCount);
        }

        protected override string GetInsertTrackingValue(IEnumerable<BonusModel> data)
            => data.Max(d => d.BonusId).ToString();

        protected override async Task<string> GetLastTrackingValueAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var id = string.IsNullOrWhiteSpace(tracker.KeyValue) ? 0 : int.Parse(tracker.KeyValue);
            var queryId = GetAdjustmentId(id);
            return queryId.ToString();
        }

        private int GetAdjustmentId(int id)
            => id - Settings.AdjustmentValue < 0 ? 0 : id - Settings.AdjustmentValue;
    }
}
