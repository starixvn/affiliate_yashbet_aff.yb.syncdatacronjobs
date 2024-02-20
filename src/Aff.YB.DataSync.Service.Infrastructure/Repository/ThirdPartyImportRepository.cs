using Aff.YB.DataSync.Service.Core.Domain;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Infrastructure.Exceptions;
using Aff.YB.DataSync.Service.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff.YB.DataSync.Service.Infrastructure.Repository
{
    public class ThirdPartyImportRepository : IThirdPartyImportRepository
    {
        public readonly IDynamicRepository _dynamicRepository;

        public ThirdPartyImportRepository(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public async Task ImportRecordAsync(IPushDataRecord betRecord)
        {
            var command = (InsertThirdPartyRecodInfoCommand)betRecord;
            await _dynamicRepository.ExecuteAsync(command);
            if (betRecord.ErrCode != 0)
            {
                throw new ImporterException(command.ErrCode.ToString(), command.ErrMsg, command.XmlData);
            }
        }
    }
}
