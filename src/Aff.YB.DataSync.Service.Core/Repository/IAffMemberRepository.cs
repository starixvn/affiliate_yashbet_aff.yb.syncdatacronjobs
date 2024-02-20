using Aff.YB.DataSync.Service.Core.Domain;

namespace Aff.YB.DataSync.Service.Core.Repository
{
    public interface IAffMemberRepository
    {
        Task ImportMemberAsync(IAffMemberImportInfo memberInfo);

        Task<DateTime> GetLastedMemberImportedTimeAsync(ISettingInfo setting);

        Task<string> GetSiteNameAsync(ISettingInfo settingInfo);
    }
}
