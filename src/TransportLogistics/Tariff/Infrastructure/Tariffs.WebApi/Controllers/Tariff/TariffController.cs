using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Application.Repositories;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;
using Route = TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Route;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff;

/// <summary>
/// API for managing tariffs
/// </summary>
/// <response code="200">Success</response>
/// <response code="401">User is not authenticated</response>
/// <response code="400">Validation error</response>
[ApiController]
[Route("api/tariff")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class TariffController : ControllerBase
{
    /// <summary>
    /// Get a tariff
    /// </summary>
    /// <param name="tariffId" example="d8aae288-7ae9-4536-bc69-00e74cc85865">Tariff identifier</param>
    /// <response code="404">Tariff not found</response>
    [HttpGet("{tariffId:guid}")]
    [ProducesResponseType(typeof(TariffView), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTariff(
        [FromRoute] Guid tariffId,
        [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
        CancellationToken cancellationToken)
    {
        var getTariffQuery = new GetTariffQuery(tariffId);
        var tariffView = await queryHandler.HandleAsync(getTariffQuery, cancellationToken).ConfigureAwait(false);

        return Ok(tariffView);
    }

    /// <summary>
    /// Create a tariff
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TariffView), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTariff(
        [FromBody] CreateTariffRequest request,
        [FromServices] IUseCaseHandler<CreateTariffCommand> commandHandler,
        [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
        [FromServices] IUserContext userContext,
        CancellationToken cancellationToken)
    {
        var command = new CreateTariffCommand(request.TariffId, userContext.GetProfileId());

        await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        var getTariffQuery = new GetTariffQuery(request.TariffId);
        var tariffView = await queryHandler.HandleAsync(getTariffQuery, cancellationToken).ConfigureAwait(false);

        return StatusCode(StatusCodes.Status201Created, tariffView);
    }

    /// <summary>
    /// Save a tariff with route parameters
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Tariff identifier</param>
    [HttpPut("{tariffId:guid}/route")]
    [ProducesResponseType(typeof(TariffView), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveTariffRoute(
        [FromRoute] Guid tariffId,
        [FromBody] SaveTariffRouteRequest request,
        [FromServices] IUseCaseHandler<SaveTariffRouteCommand> commandHandler,
        [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
        CancellationToken cancellationToken)
    {
        var points = request.Route.Points
            .Select(point => new Point(point.LocationId, point.Type, point.Order))
            .ToHashSet();
        await commandHandler
            .HandleAsync(new SaveTariffRouteCommand(tariffId, new Route(points)), cancellationToken)
            .ConfigureAwait(false);

        var getTariffQuery = new GetTariffQuery(tariffId);
        var tariffView = await queryHandler.HandleAsync(getTariffQuery, cancellationToken).ConfigureAwait(false);

        return Ok(tariffView);
    }

    /// <summary>
    /// Save a tariff with cargo parameters
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Tariff identifier</param>
    [HttpPut("{tariffId:guid}/cargo")]
    [ProducesResponseType(typeof(TariffView), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveTariffCargo(
        [FromRoute] Guid tariffId,
        [FromBody] SaveTariffCargoRequest request,
        [FromServices] IUseCaseHandler<SaveTariffCargoEquipmentCommand> commandHandler,
        [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
        CancellationToken cancellationToken)
    {
        var command = new SaveTariffCargoEquipmentCommand(
            tariffId,
            new CargoEquipment(request.CargoType, request.ContainerOwn, request.ContainerSize));
        await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        var getTariffQuery = new GetTariffQuery(tariffId);
        var tariffView = await queryHandler.HandleAsync(getTariffQuery, cancellationToken).ConfigureAwait(false);

        return Ok(tariffView);
    }

    /// <summary>
    /// Save a tariff with price parameters
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Tariff identifier</param>
    [HttpPut("{tariffId:guid}/price")]
    [ProducesResponseType(typeof(TariffView), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveTariffPrice(
        [FromRoute] Guid tariffId,
        [FromBody] SaveTariffPriceRequest request,
        [FromServices] IUseCaseHandler<SaveTariffPriceCommand> commandHandler,
        [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
        CancellationToken cancellationToken)
    {
        var command = new SaveTariffPriceCommand(tariffId, new Price(request.Price, request.CurrencyCode));
        await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        var getTariffQuery = new GetTariffQuery(tariffId);
        var tariffView = await queryHandler.HandleAsync(getTariffQuery, cancellationToken).ConfigureAwait(false);

        return Ok(tariffView);
    }

    /// <summary>
    /// Publish a tariff
    /// </summary>
    /// <remarks>
    /// Moves a tariff from draft to active by creating a copy and deleting the draft.
    /// </remarks>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Tariff identifier</param>
    [HttpPost("{tariffId:guid}/publish/")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PublishTariff(
        [FromRoute] Guid tariffId,
        [FromServices] IUseCaseHandler<PublishTariffCommand> commandHandler,
        CancellationToken cancellationToken)
    {
        var command = new PublishTariffCommand(tariffId);
        await commandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
