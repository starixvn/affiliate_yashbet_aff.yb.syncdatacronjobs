using Aff.YB.DataSync.Service.Core.Domain;

namespace Aff.YB.DataSync.Service.Core.Repository
{
    public interface IThirdPartyImportRepository
    {
        Task ImportRecordAsync(IPushDataRecord betRecord);
    }
}
