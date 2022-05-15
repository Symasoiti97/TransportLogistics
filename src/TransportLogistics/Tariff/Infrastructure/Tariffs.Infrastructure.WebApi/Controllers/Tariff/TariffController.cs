using Application.Abstracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tariffs.Application.TariffServices;
using Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff;

/// <summary>
/// АПИ для управления тарифами
/// </summary>
[Route("api/tariff")]
public class TariffController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public TariffController(IUserContext userContext, IMapper mapper)
    {
        _userContext = userContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Создать тариф
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="handler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPost("/create")]
    public async Task<IActionResult> CreateTariff([FromBody] Guid tariffId, [FromServices] ICommandHandler<CreateTariffCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateTariffCommand
        {
            TariffId = tariffId,
            ManagerProfileId = _userContext.GetProfileId()
        };

        await handler.HandleAsync(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Сохранить тариф с параметрами маршрута
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="handler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("/save/route/{tariffId}")]
    public async Task<IActionResult> SaveTariffRoute([FromRoute] Guid tariffId, [FromBody] SaveTariffRouteRequest request,
        [FromServices] ICommandHandler<SaveTariffRouteCommand> handler, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<SaveTariffRouteCommand>(request, options =>
        {
            options.AfterMap((_, dest) =>
            {
                dest.TariffId = tariffId;
                dest.ManagerProfileId = _userContext.GetProfileId();
            });
        });

        await handler.HandleAsync(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Сохранить тариф с параметрами маршрута
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="handler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("/save/cargo/{tariffId}")]
    public async Task<IActionResult> SaveTariffCargo([FromRoute] Guid tariffId, [FromBody] SaveTariffCargoRequest request,
        [FromServices] ICommandHandler<SaveTariffCargoCommand> handler, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<SaveTariffCargoCommand>(request, options => { options.AfterMap((_, dest) => { dest.TariffId = tariffId; }); });

        await handler.HandleAsync(command, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Сохранить тариф с параметрами маршрута
    /// </summary>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="request">Параметры запроса</param>
    /// <param name="handler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("/save/price/{tariffId}")]
    public async Task<IActionResult> SaveTariffPrice([FromRoute] Guid tariffId, [FromBody] SaveTariffPriceRequest request,
        [FromServices] ICommandHandler<SaveTariffPriceCommand> handler, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<SaveTariffPriceCommand>(request, options => { options.AfterMap((_, dest) => { dest.TariffId = tariffId; }); });

        await handler.HandleAsync(command, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Опубликовать тариф
    /// </summary>
    /// <remarks>
    /// Переводит тариф из черновика в действующий, создавая копию и удаляя черновик.
    /// </remarks>
    /// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Идентификатор тарифа</param>
    /// <param name="handler">Обработчик команды</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("/publish/{tariffId}")]
    public async Task<IActionResult> PublishTariff([FromRoute] Guid tariffId, [FromServices] ICommandHandler<PublishTariffCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new PublishTariffCommand
        {
            TariffId = tariffId
        };

        await handler.HandleAsync(command, cancellationToken);

        return Ok();
    }
}