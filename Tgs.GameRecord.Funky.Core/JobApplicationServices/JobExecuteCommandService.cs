using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Funky.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Funky.Core.JobApplicationServices;

internal class JobExecuteCommandService(
	ILogger<JobExecuteCommandService> logger,
	TimeProvider timeProvider,
	IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
	public async ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();

		var endTime = new DateTimeOffset(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0, currentTime.Offset).AddHours(1);
		var startTime = endTime.AddHours(-6);

		do
		{
			var tempStartTime = startTime;
			startTime = startTime.AddHours(+1);

			var message = new FunkyGameRecordMessage
			{
				ApiUrl = command.ApiUrl,
				UserAgent = command.UserAgent,
				Authentication = command.Authentication,
				StartTime = tempStartTime,
				EndTime = startTime
			};

			logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), message);

			await messageSender.PublishAsync(
				subject: FunkyNatsConst.SchedulePublish,
				data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		} while (startTime.CompareTo(endTime) < 0);
	}
}