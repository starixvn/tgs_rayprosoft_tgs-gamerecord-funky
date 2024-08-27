using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Funky.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Funky.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FunkyController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		FunkyGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: FunkyNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		FunkyGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: FunkyNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
