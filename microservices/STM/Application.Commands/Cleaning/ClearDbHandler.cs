using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;

namespace Application.Commands.Cleaning;

public sealed class ClearDbHandler(ITripWriteRepository tripWriteRepository, IStopWriteRepository stopWriteRepository) : ICommandHandler<ClearDb>
{
    public async Task Handle(ClearDb command, CancellationToken cancellation)
    {
        await tripWriteRepository.ClearAsync();
        await stopWriteRepository.ClearAsync();
    }
}