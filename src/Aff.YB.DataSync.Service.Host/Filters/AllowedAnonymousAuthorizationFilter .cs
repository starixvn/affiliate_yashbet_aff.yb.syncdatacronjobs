using Hangfire.Dashboard;

namespace Aff.YB.DataSync.Service.Host.Filters
{
    public class AllowedAnonymousAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

}
