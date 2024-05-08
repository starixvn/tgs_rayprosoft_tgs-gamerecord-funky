using System.Text;
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
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"apiUrl": "http://trial-gp-api-report.funkytest.com/",
	///				"userAgent": "109-PG",
	///				"authentication": "c69d57e5-93e7-488c-90a4-e20876b495bc",
	///				"StartTime": "2024-03-19T03:00:00+00:00",
	///				"EndTime": "2024-03-19T04:00:00+00:00"
	///         }
	/// </remarks>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		FunkyGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: FunkyNatsConst.ManualPublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///         {
	///				"apiUrl": "http://trial-gp-api-report.funkytest.com/",
	///				"userAgent": "109-PG",
	///				"authentication": "c69d57e5-93e7-488c-90a4-e20876b495bc",
	///				"StartTime": "2024-03-19T03:00:00+00:00",
	///				"EndTime": "2024-03-19T04:00:00+00:00"
	///         }
	/// </remarks>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		FunkyGameRecordMessage source)
		=> messageSender.PublishAsync(
			subject: FunkyNatsConst.SchedulePublish,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(source)),
			cancellationToken: HttpContext.RequestAborted);
}
