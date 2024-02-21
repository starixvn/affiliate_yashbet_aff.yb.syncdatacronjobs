using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Constants;
using Aff.YB.DataSync.Service.Infrastructure.Helpers;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs
{
    public class FetchServerInfoDataTask : FetchDataForMultipleRepositoryTaskBase<ServerInfoModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;

        public FetchServerInfoDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger<FetchServerInfoDataTask> logger
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_SERVER_INFO_SETTING))
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async override Task FetchDataAsync()
        {
            var data = await _licenseeApi.GetServerInfoAsync();
            await ImportDataToMultipleRepoAsync(data);
        }

        protected override string BuildXml(IEnumerable<ServerInfoModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
