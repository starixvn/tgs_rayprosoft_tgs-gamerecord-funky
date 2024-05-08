using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameRecord.Funky;
using Tgs.GameRecord.Funky.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecord(
	   this IServiceCollection services,
	   Action<RedisOptions, IServiceProvider> redisDBconfigure,
	   Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
	   Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
	   Action<GameInterfaceFunkyClientOptions, IServiceProvider> gameInterfaceFunkyClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions,
			gameInterfaceFunkyClientOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
