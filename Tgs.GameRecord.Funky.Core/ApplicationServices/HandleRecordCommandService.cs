using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Funky.Core.ApplicationServices;

internal class HandleRecordCommandService(
	ILogger<HandleRecordCommandService> logger,
	IOptions<GameRecordInfoOptions> options,
	IMessageSender messageSender) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var externalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset();
		var betState = command.GameWager.Status switch
		{
			GameInterface.Funky.Protos.GameWagerStatus.Canceled => Handle.Common.Protos.BetState.Cancel,
			GameInterface.Funky.Protos.GameWagerStatus.Voided => Handle.Common.Protos.BetState.Void,
			GameInterface.Funky.Protos.GameWagerStatus.Won => Handle.Common.Protos.BetState.Win,
			GameInterface.Funky.Protos.GameWagerStatus.Loss => Handle.Common.Protos.BetState.Loss,
			GameInterface.Funky.Protos.GameWagerStatus.Draw => Handle.Common.Protos.BetState.Draw,
			_ => Handle.Common.Protos.BetState.Running
		};

		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
		var validBet = 0.0m;
		var winLoss = 0.0m;
		var settle = 0.0m;
		if (betState != Handle.Common.Protos.BetState.Running)
		{
			externalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset();
			bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0);
			validBet = command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0);
			winLoss = command.GameWager.WinLoss.ToDecimal().GetValueOrDefault(0);
			settle = command.GameWager.Settle.ToDecimal().GetValueOrDefault(0);
		}

		var request = new Handle.Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = _gameRecordInfo.GameProviderCode,
			PlayerCode = command.GameWager.PlayerCode,
			ExternalRoundId = command.GameWager.ExternalWagerId,
			ExternalBetId = command.GameWager.ExternalWagerId,
			ExternalGameId = command.GameWager.ExternalGameId,
			ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
			ExternalClosedOn = externalClosedOn.ToTimestamp(),
			ExternalPayoffOn = externalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = betState,
			BetType = Handle.Common.Protos.BetType.Normal,
			FreeGameState = Handle.Common.Protos.FreeGameState.None,
			GambleState = Handle.Common.Protos.GambleState.None,
			CashoutState = Handle.Common.Protos.CashoutState.None,
			ResettleState = Handle.Common.Protos.ResettleState.None,
			JackpotState = Handle.Common.Protos.JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = validBet,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = command.GameWager.AdditionalData
		};

		await messageSender.PublishAsync(
			subject: Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This publication requests information.({request})",
			DateTime.Now.ToString("o"),
			Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			new
			{
				request.GameProviderCode,
				request.PlayerCode,
				request.ExternalRoundId,
				request.ExternalBetId,
				request.ExternalGameId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = request.ExternalClosedOn.ToDateTimeOffset(),
				request.BetState
			});
	}
}
