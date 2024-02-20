
namespace Aff.YB.DataSync.Service.Core.Domain
{
    public interface IAffMemberImportInfo
    {
        string SiteName { get; }

        string SettingPrefix { get; }

        string XmlData { get; }
    }
}
