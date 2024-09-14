using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Infrastructure.Neo4j;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Queries;

internal sealed class GetTariffQueryHandler : IQueryHandler<GetTariffQuery, TariffView>
{
    private readonly ICypherGraphClientFactory _graphClientFactory;

    public GetTariffQueryHandler(ICypherGraphClientFactory graphClientFactory)
    {
        ArgumentNullException.ThrowIfNull(graphClientFactory);

        _graphClientFactory = graphClientFactory;
    }

    public async Task<TariffView> HandleAsync(GetTariffQuery command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var query = await _graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

        var tariffViews = await query
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
            .ResultsAsync
            .ConfigureAwait(false);

        var tariffView = tariffViews.SingleOrDefault();

        if (tariffView is null)
        {
            throw new TariffNotFound(command.TariffId);
        }

        return tariffView;
    }
}