using Aff.YB.DataSync.API.Core.Services;
using Aff.YB.DataSync.API.Infastructure.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Aff.YB.DataSync.API.Infastructure
{
    public static class ApiCoreServiceExtentions
    {
        public static IServiceCollection AddApiCoreService(this IServiceCollection service)
        {
            service.AddSingleton<IDynamicRepository, DynamicRepository>();

            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IUserManagementService, UserManagementService>();
            return service;
        }
    }
}
