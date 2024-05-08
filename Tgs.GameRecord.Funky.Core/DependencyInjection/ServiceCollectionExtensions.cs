using System.Reflection;
using StackExchange.Redis;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Funky.Client;
using Tgs.GameRecord.Funky.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceFunkyClientOptions, IServiceProvider> gameInterfaceFunkyClientOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameInterfaceFunkyClient(gameInterfaceFunkyClientOptions)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IPullRecordDispatcher, PullRecordDispatcher>()
		.AddTransient<IHangfireUtensil, HangfireUtensil>()
		.AddTransient<IHangfireLock>(sp => ActivatorUtilities.CreateInstance<HangfireLock>(sp, redisDatabaseResolver(sp)));
}
