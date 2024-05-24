

using Aff.YB.DataSync.API.Core.Models;
using Aff.YB.DataSync.API.Core.Params;
using System;
using System.Threading.Tasks;

namespace Aff.YB.DataSync.API.Core.Services
{
    public interface IAuthService
    {
        Task<Tuple<bool, Token>> IsAuthenticated(TokenRequest request);
    }
}
