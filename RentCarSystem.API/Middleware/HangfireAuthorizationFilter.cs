using Hangfire.Dashboard;

namespace RentCarSystem.API.Middleware
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //  herkese açık bir durumda.
            
            return true;
        }
    }
}