namespace Tgs.GameRecord.Funky.Core.ApplicationServices;
public record PullRecordCommand(
	Uri ApiUrl,
	string Authentication,
	string UserAgent,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);