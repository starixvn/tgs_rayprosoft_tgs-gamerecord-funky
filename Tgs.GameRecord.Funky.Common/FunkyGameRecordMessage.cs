using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Funky.Common;

public record FunkyGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "http://trial-gp-api-report.funkytest.com/")]
	public Uri ApiUrl { get; init; } = default!;

	[Required]
	[DefaultValue(typeof(string), "109-PG")]
	public string Authentication { get; init; } = default!;

	[Required]
	[DefaultValue(typeof(string), "c69d57e5-93e7-488c-90a4-e20876b495bc")]
	public string UserAgent { get; init; } = default!;

	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-03-19T03:00:00+00:00")]
	public DateTimeOffset StartTime { get; init; }

	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-03-19T04:00:00+00:00")]
	public DateTimeOffset EndTime { get; init; }
}
