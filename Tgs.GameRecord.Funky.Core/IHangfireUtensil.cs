using Tgs.GameRecord.Funky.Core.JobApplicationServices;

namespace Tgs.GameRecord.Funky.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
