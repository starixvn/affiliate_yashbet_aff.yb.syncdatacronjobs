using Aff.YB.DataSync.DataDomain.Api;
using Aff.YB.DataSync.DataDomain.Params;
using Refit;

namespace Aff.YB.DataSync.Service.Infrastructure.Apis
{
    public interface ITokenApi
    {
        [Post("/Auth")]
        Task<TokenInfo> GetTokenAsync([Body] TokenRequestParam param);
    }
}
