using System.Text.Json.Nodes;
using System.Threading;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Funky.Protos;
using Tgs.GameRecord.Funky.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Funky.Core.UnitTest.ApplicationServices;
public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async void Win()
	{
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "FUNKY",
			GameProviderConfiguration = "FUNKYConfiguration",
			HangfirePrefix = "gamerecord-funky-hangfire:",
			HangfireLocking = "gamerecord-funky-locking:",
			IntervalMinutes = 10d,
			IntervalMinutesMax = 60d,
			IntervalMinutesMinimum = 1d
		});
		var fakeClient = Substitute.For<IGameRecordHandleClient>();

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

		var service = new HandleRecordCommandService(
			gameRecordInfo,
			fakeClient);

		await service.ExecuteAsync(command);

		await fakeClient.MessageAsync(
			gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
			playerCode: Arg.Is(command.GameWager.PlayerCode),
			wagerState: Arg.Is(WagerStates.Lose),
			externalRoundId: Arg.Is(command.GameWager.ExternalWagerId),
			externalWagerId: Arg.Is(command.GameWager.ExternalWagerId),
			externalGameId: Arg.Is(command.GameWager.ExternalGameId),
			externalCreatedOn: Arg.Is(command.GameWager.ExternalCreatedOn.ToDateTimeOffset()),
			externalClosedOn: Arg.Is(command.GameWager.ExternalClosedOn.ToDateTimeOffset()),
			bet: Arg.Is(command.GameWager.Bet.ToDecimal().GetValueOrDefault(0)),
			validBet: Arg.Is(command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0)),
			settle: Arg.Is(command.GameWager.Settle.ToDecimal().GetValueOrDefault(0)),
			commission: Arg.Is(0.0m),
			turnover: Arg.Is(command.GameWager.ValidBet.ToDecimal().GetValueOrDefault(0)),
			winLoss: Arg.Is(command.GameWager.WinLoss.ToDecimal().GetValueOrDefault(0)),
			additionalData: Arg.Is(JsonNode.Parse(command.GameWager.AdditionalData)!),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
