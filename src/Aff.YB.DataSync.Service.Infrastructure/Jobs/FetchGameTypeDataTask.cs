using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Constants;
using Aff.YB.DataSync.Service.Infrastructure.Helpers;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs
{
    public class FetchGameTypeDataTask : FetchDataForMultipleRepositoryTaskBase<TpGameTypeModel>
    {
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;

        public FetchGameTypeDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ILogger logger
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_GAME_TYPE_SETTING))
        {
            _licenseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async override Task FetchDataAsync()
        {
            var data = await _licenseeApi.GetGameTypeAsync();
            await ImportDataToMultipleRepoAsync(data);
        }

        protected override string BuildXml(IEnumerable<TpGameTypeModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;
    }
}
