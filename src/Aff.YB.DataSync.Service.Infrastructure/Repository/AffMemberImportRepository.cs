using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.Exceptions;
using Aff.YB.DataSync.Service.Infrastructure.SpParams;
using Aff.YB.DataSync.Service.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff.YB.DataSync.Service.Infrastructure.Repository
{
    public class AffMemberImportRepository : IAffMemberRepository
    {
        private readonly IDynamicRepository _dynamicRepository;

        public AffMemberImportRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task<DateTime> GetLastedMemberImportedTimeAsync(ISettingInfo setting)
            => await _dynamicRepository.GetAsync<DateTime>((MemberImportedTimeInfo)setting);

        public async Task<string> GetSiteNameAsync(ISettingInfo settingInfo)
            => await _dynamicRepository.GetAsync<string>((SiteNameInfo)settingInfo);

        public async Task ImportMemberAsync(IAffMemberImportInfo memberInfo)
        {
            var memberImportInfo = (InsertMemberInfoCommand)memberInfo;
            await _dynamicRepository.ExecuteAsync(memberImportInfo);
            if (memberImportInfo.ErrCode != 0)
            {
                throw new ImporterException(memberImportInfo.ErrCode.ToString(), memberImportInfo.ErrMsg, memberImportInfo.XmlData);
            }
        }
    }
}