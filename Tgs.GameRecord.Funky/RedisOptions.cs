namespace Tgs.GameRecord.Funky;

public record RedisOptions
{
	public string ConnectionString { get; set; } = "localhost";
}
