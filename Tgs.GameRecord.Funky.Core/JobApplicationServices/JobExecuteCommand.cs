namespace Tgs.GameRecord.Funky.Core.JobApplicationServices;

public record JobExecuteCommand(
	Uri ApiUrl,
	string Authentication,
	string UserAgent);