using Aff.YB.DataSync.API.Infastructure.Params;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Constants;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.RetryJob
{
    public class ReInsertAffiliateInfoDataTask : IRetryDataTask
    {
        private readonly IDataTrackerService _trackerService;
        private readonly ILicenseeApi _liceseeApi;
        private readonly ILogger _logger;

        private static IEnumerable<int> ListPullStatusId = new int[] { DataTrackerStatus.InProcess, DataTrackerStatus.Failure };

        private FetchServiceSetting Setting { get; }

        public ReInsertAffiliateInfoDataTask(IDataTrackerService dataTrackerService,
            ILicenseeApi licenseeApi,
            IOptionsSnapshot<FetchServiceSetting> configuration,
            ILogger<ReInsertAffiliateInfoDataTask> logger
            )
        {
            _trackerService = dataTrackerService;
            _liceseeApi = licenseeApi;
            _logger = logger;
            Setting = configuration.Get(ServiceConfigName.INSERT_AFFILIATE_SETTING);
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task HandleAsync()
        {
            var prefixPrviders = Setting.SupportedPrefix;
            var dataTrackerItems = await _trackerService.FetchTrackerAsync(
                prefixPrviders,
                new int[] { DataTrackerType.InsertAffiliate },
                ListPullStatusId
            );

            foreach (var item in dataTrackerItems)
            {
                var affiliateInfo = JsonConvert.DeserializeObject<AffiliateParam>(item.PushDataInfo);
                var result = await _liceseeApi.InsertAffiliateInfo(affiliateInfo);
                if (result.IsSuccess)
                {
                    await _trackerService.UpdateTrackerStatusAsync(item, DataTrackerStatus.Success);
                }
                else
                {
                    _logger.LogError($"Insert affiliate to B2C DB fail - {result.ErrMsg}");
                }
            }
        }
    }
}
