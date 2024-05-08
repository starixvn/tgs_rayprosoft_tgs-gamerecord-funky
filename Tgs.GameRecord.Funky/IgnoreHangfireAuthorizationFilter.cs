using Hangfire.Dashboard;

namespace Tgs.GameRecord.Funky;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
