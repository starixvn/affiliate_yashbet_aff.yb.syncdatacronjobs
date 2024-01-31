using Aff.YB.DataSync.API.Infastructure.Middlewares;
using Microsoft.AspNetCore.Builder;


namespace Aff.YB.DataSync.API.Infastructure
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseJWTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }
    }
}
