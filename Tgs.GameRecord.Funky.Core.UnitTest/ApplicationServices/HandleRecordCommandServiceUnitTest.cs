using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Funky.Protos;
using Tgs.GameRecord.FaChai.Core;
using Tgs.GameRecord.Funky.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Funky.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakeGameRecordInfoOptions = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "FUNKY",
			GameProviderConfiguration = "FUNKYConfiguration",
			HangfirePrefix = "gamerecord-funky-hangfire:",
			HangfireLocking = "gamerecord-funky-locking:",
			IntervalMinutes = 10d,
			IntervalMinutesMax = 60d,
			IntervalMinutesMinimum = 1d
		});
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			GameWager: new GameWagerInfo

			{
				ExternalWagerId = "Funky_tt_4xCaptainMoneyx5199x1359",
				ExternalGameId = "10211",
				PlayerCode = "4ucmqplxb029",
				Status = GameWagerStatus.Loss,
				ExternalCreatedOn = new DateTimeOffset(2024, 03, 18, 09, 44, 00, TimeSpan.Zero).ToTimestamp(),
				ExternalClosedOn = new DateTimeOffset(2024, 03, 18, 09, 44, 00, TimeSpan.Zero).ToTimestamp(),
				Bet = 107.1m,
				ValidBet = 77.5m,
				Settle = 62.7m,
				WinLoss = -14.8m,
				AdditionalData = new JsonObject
				{
					["playerId"] = "4ucmqplxb029",
					["statementDate"] = "2024-03-18",
					["betTime"] = "2024-03-18 09:44:00",
					["refNo"] = "Funky_tt_4xCaptainMoneyx5199x1359",
					["status"] = "L",
					["gameCode"] = "10211",
					["gameName"] = "Captain Money",
					["currency"] = "CNY",
					["betAmount"] = 107.1,
					["effectiveStake"] = 77.5,
					["winloss"] = -14.8,
					["isTransferGame"] = true
				}.ToJsonString()
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakeGameRecordInfoOptions,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = fakeGameRecordInfoOptions.Value.GameProviderCode,
				PlayerCode = command.GameWager.PlayerCode,
				ExternalRoundId = command.GameWager.ExternalWagerId,
				ExternalBetId = command.GameWager.ExternalWagerId,
				ExternalGameId = command.GameWager.ExternalGameId,
				ExternalCreatedOn = command.GameWager.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.GameWager.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = command.GameWager.Bet.ToDecimal().GetValueOrDefault(0),
				ValidBet = command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0),
				Settle = command.GameWager.Settle.ToDecimal().GetValueOrDefault(0),
				Commission = 0.0m,
				Turnover = command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0),
				WinLoss = command.GameWager.WinLoss.ToDecimal().GetValueOrDefault(0),
				Jackpot = 0.0M,
				AdditionalData = command.GameWager.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
