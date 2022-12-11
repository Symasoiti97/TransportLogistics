using EnsureThat;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Extensions;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Queries;

internal sealed class GetTariffQueryHandler : IQueryHandler<GetTariffQuery, TariffView>
{
    private readonly TariffDbContext _tariffDbContext;

    public GetTariffQueryHandler(TariffDbContext tariffDbContext)
    {
        EnsureArg.IsNotNull(tariffDbContext, nameof(tariffDbContext));

        _tariffDbContext = tariffDbContext;
    }

    public async Task<TariffView> HandleAsync(GetTariffQuery command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var tariffViews = await _tariffDbContext
            .ReadAsync(
                query => query
                    .Match("(t:Tariff {Id: $tariffId})")
                    .WithParam("tariffId", command.TariffId)
                    .OptionalMatch("(t)-[:HAS_ROUTE]->(r:Route)-[hp:HAS_POINT]->(p:Location)")
                    .With(
                        @"{
                            Id: t.Id, 
                            CargoType: t.CargoType, 
                            ContainerOwn: t.ContainerOwn, 
                            ContainerSize: t.ContainerSize, 
                            IsDraft: t.IsDraft, 
                            ManagerProfileId: t.ManagerProfileId, 
                            Price: CASE WHEN t.Price IS NOT NULL THEN { 
                                Value: t.Price, 
                                CurrencyCode: t.CurrencyCode
                            } END, 
                            Route: CASE WHEN r IS NOT NULL THEN {
                                Type: r.Type, 
                                Hash: r.Hash, 
                                Points: collect({ 
                                            Hash: hp.Hash, 
                                            Type: hp.Type, 
                                            Order: hp.Order, 
                                            LocationId: p.Id
                                        })
                            } END
                        } as tariff")
                    .Return<TariffView>("tariff")
                    .ResultsAsync,
                cancellationToken)
            .ConfigureAwait(false);

        var tariffView = tariffViews.FirstOrDefault();

        Error.Throw().TariffNotFoundIfNull(tariffView, command.TariffId);

        return tariffView;
    }
}