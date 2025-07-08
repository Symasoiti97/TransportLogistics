using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Application.Repositories;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;
using Route = TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Route;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff;

/// <summary>
/// АПИ для управления тарифами
/// </summary>
/// <response code="200">Успех</response>
/// <response code="401">Пользователь не авторизован</response>
/// <response code="400">Ошибка валидации</response>
[ApiController]
[Route("api/tariff")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class TariffController : ControllerBase
{
    /// <summary>
    /// Получить тариф
    /// </summary>
    /// <param name="tariffId" example="d8aae288-7ae9-4536-bc69-00e74cc85865">Идентификатор тарифа</param>
    /// <param name="queryHandler">Обработчик запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <response code="404">Тариф не найден</response>
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
    /// Создать тариф
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="commandHandler">Обработчик команды</param>
    /// <param name="queryHandler">Обработчик запроса</param>
    /// <param name="userContext">Пользовательский контекст</param>
    /// <param name="cancellationToken">Токен отмены</param>
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
    /// Сохранить тариф с параметрами маршрута
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="commandHandler">Обработчик команды</param>
    /// <param name="queryHandler">Обработчик запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
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
    /// Сохранить тариф с параметрами груза
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="commandHandler">Обработчик команды</param>
    /// <param name="queryHandler">Обработчик запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
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
    /// Сохранить тариф с параметрами стоимости
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="commandHandler">Обработчик команды</param>
    /// <param name="queryHandler">Обработчик запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
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
    /// Опубликовать тариф
    /// </summary>
    /// <remarks>
    /// Переводит тариф из черновика в действующий, создавая копию и удаляя черновик.
    /// </remarks>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="commandHandler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
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