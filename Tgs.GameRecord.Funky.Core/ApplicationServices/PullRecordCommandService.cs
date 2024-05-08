using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameInterface.Funky.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Funky.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	IGameInterfaceFunkyClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		var page = 1;

		IEnumerable<GameWagerInfo> gameWagers;

		do
		{
			gameWagers = await client.GameWagerAsync(
				apiUrl: command.ApiUrl,
				userAgent: command.UserAgent,
				authentication: command.Authentication,
				startTime: command.StartTime,
				endTime: command.EndTime,
				page: page,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<GameWagerInfo>();
			foreach (var gameWager in gameWagers)
			{
				logger.LogDebug("LogOn:{logOn} GameWager:{gameWager}", DateTime.Now.ToString("o"), gameWager);

				try
				{
					await service
						.ExecuteAsync(
						command: new(gameWager),
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game wager.({gameWager})", DateTime.Now.ToString("o"), gameWager);
				}
			}

			page++;
		} while (gameWagers.Any());
	}
}
