using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Funky.Core.ApplicationServices;

internal class HandleRecordCommandService(
	IOptions<GameRecordInfoOptions> options,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset();
		var wagerState = command.GameWager.Status switch
		{
			GameInterface.Funky.Protos.GameWagerStatus.Canceled => WagerStates.Reject,
			GameInterface.Funky.Protos.GameWagerStatus.Voided => WagerStates.Void,
			GameInterface.Funky.Protos.GameWagerStatus.Won => WagerStates.Win,
			GameInterface.Funky.Protos.GameWagerStatus.Loss => WagerStates.Lose,
			GameInterface.Funky.Protos.GameWagerStatus.Draw => WagerStates.Draw,
			_ => WagerStates.Running
		};

		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = 0.0m;
		var winLoss = 0.0m;
		var settle = 0.0m;
		if (wagerState != WagerStates.Running)
		{
			externalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset();
			bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
			validBet = command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0);
			winLoss = command.GameWager.WinLoss.ToDecimal().GetValueOrDefault(0);
			settle = command.GameWager.Settle.ToDecimal().GetValueOrDefault(0);
		}

		return client.MessageAsync(
			gameProviderCode: _gameRecordInfo.GameProviderCode,
			playerCode: command.GameWager.PlayerCode,
			wagerState: wagerState,
			externalRoundId: command.GameWager.ExternalWagerId,
			externalWagerId: command.GameWager.ExternalWagerId,
			externalGameId: command.GameWager.ExternalGameId,
			externalCreatedOn: externalCreatedOn,
			externalClosedOn: externalClosedOn,
			bet: bet,
			validBet: validBet,
			settle: settle,
			commission: 0.0m,
			turnover: validBet,
			winLoss: winLoss,
			additionalData: JsonNode.Parse(command.GameWager.AdditionalData)!,
			cancellationToken: cancellationToken);
	}
}