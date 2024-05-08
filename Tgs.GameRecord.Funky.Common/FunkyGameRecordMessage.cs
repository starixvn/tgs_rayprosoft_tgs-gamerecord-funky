using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Funky.Common;

public record FunkyGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string Authentication { get; init; } = default!;
	[Required]
	public string UserAgent { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}
