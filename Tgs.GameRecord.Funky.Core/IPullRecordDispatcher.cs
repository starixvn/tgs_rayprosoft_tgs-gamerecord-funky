using Tgs.GameRecord.Funky.Core.ApplicationServices;

namespace Tgs.GameRecord.Funky.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
