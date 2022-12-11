using EnsureThat;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Application.Repositories;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик для создания тарифа
/// </summary>
internal sealed class CreateTariffCommandHandler : ICommandHandler<CreateTariffCommand>
{
    private readonly ITariffRepository _tariffRepository;
    private readonly IUserContext _userContext;

    public CreateTariffCommandHandler(ITariffRepository tariffRepository, IUserContext userContext)
    {
        EnsureArg.IsNotNull(tariffRepository, nameof(tariffRepository));
        EnsureArg.IsNotNull(userContext, nameof(userContext));

        _tariffRepository = tariffRepository;
        _userContext = userContext;
    }

    public async Task HandleAsync(CreateTariffCommand command, CancellationToken cancellationToken)
    {
        var tariff = Tariff.Create(command.TariffId, _userContext.GetProfileId());

        _tariffRepository.Add(tariff);

        await _tariffRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}