using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.DataDomain.Params;
using Refit;

namespace Aff.YB.DataSync.Service.Infrastructure.Apis
{
    public interface IProduct9WApi
    {
        [Get("/Product9W/GetTpReport")]
        Task<IEnumerable<TpReport9WModel>> GetTpReportAsync(BaseParam param);

        [Get("/Product9W/GetTpWager")]
        Task<IEnumerable<TpWager9WModel>> GetTpWagerAsync(BaseParam param);
    }
}
