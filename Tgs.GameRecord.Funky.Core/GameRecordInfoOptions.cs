namespace Tgs.GameRecord.Funky.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; init; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-funky-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-funky-locking:";
	public double IntervalMinutes { get; init; } = 60;

	public double IntervalMinutesMax { get; init; } = 60;

	public double IntervalMinutesMinimum { get; init; } = 1;
}