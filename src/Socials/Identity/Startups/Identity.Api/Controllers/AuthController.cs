using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.TokenServices;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Application.UseCases.UserSessionServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Startups.Api.Models;

namespace TL.Socials.Identity.Startups.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
{
    private const string AccessToken = "access_token";

    [HttpPost("register/email/request")]
    public async Task<RequestUserRegisterViaEmailAnswerDto> RequestUserRegisterViaEmailAsync(
        [FromBody] RequestUserRegisterViaEmailDto transfer,
        [FromServices] IUseCaseHandler<RequestUserRegisterViaEmailCommand, bool> commandHandler,
        CancellationToken cancellationToken)
    {
        var isNewUser = await commandHandler.HandleAsync(
            new RequestUserRegisterViaEmailCommand(new Email(transfer.Email)),
            cancellationToken);

        return new RequestUserRegisterViaEmailAnswerDto(isNewUser);
    }

    [HttpPost("register/email")]
    public Task RegisterUserViaEmailAsync(
        [FromBody] RegisterUserViaEmailDto transfer,
        [FromServices] IUseCaseHandler<RegisterUserViaEmailCommand> commandHandler,
        CancellationToken cancellationToken)
        => commandHandler.HandleAsync(
            new RegisterUserViaEmailCommand(
                new Email(transfer.Email),
                transfer.Password,
                transfer.Token),
            cancellationToken);

    [HttpPost("login/email")]
    public async Task<LoginResultDto> LoginUserViaEmailAsync(
        [FromBody] LoginUserViaEmailDto transfer,
        [FromServices] IUseCaseHandler<LoginUserViaEmailCommand, UserTokens> commandHandler,
        CancellationToken cancellationToken)
    {
        var userTokens = await commandHandler.HandleAsync(
            new LoginUserViaEmailCommand(
                new Email(transfer.Email),
                transfer.Password),
            cancellationToken);

        HttpContext.Response.Cookies.Append(AccessToken, userTokens.AccessToken);

        return new LoginResultDto(userTokens.UserId, userTokens.RefreshToken);
    }

    [HttpPost("refresh/token")]
    public async Task<LoginResultDto> RefreshAuthTokenAsync(
        [FromBody] RefreshAuthTokenDto transfer,
        [FromServices] IUseCaseHandler<RefreshAuthTokenCommand, UserTokens> commandHandler,
        CancellationToken cancellationToken)
    {
        var userTokens = await commandHandler.HandleAsync(
            new RefreshAuthTokenCommand(transfer.RefreshToken),
            cancellationToken);

        HttpContext.Response.Cookies.Append(AccessToken, userTokens.AccessToken);

        return new LoginResultDto(userTokens.UserId, userTokens.RefreshToken);
    }

    [HttpPost("login/phone/request")]
    public Task RequestUserLoginViaPhone(
        [FromBody] RequestUserLoginViaPhoneDto transfer,
        [FromServices] IUseCaseHandler<RequestUserLoginViaPhoneCommand> commandHandler,
        CancellationToken cancellationToken)
        => commandHandler.HandleAsync(
            new RequestUserLoginViaPhoneCommand(new PhoneNumber(transfer.PhoneNumber)),
            cancellationToken);

    [HttpPost("login/phone")]
    public async Task<LoginResultDto> LoginUserViaPhone(
        [FromBody] LoginUserViaPhoneDto transfer,
        [FromServices] IUseCaseHandler<RegisterUserViaPhoneCommand> registerCommandHandler,
        [FromServices] IUseCaseHandler<LoginUserViaPhoneCommand, UserTokens> loginCommandHandler,
        CancellationToken cancellationToken)
    {
        await registerCommandHandler
            .HandleAsync(
                new RegisterUserViaPhoneCommand(new PhoneNumber(transfer.PhoneNumber), transfer.Code),
                cancellationToken)
            .ConfigureAwait(false);

        var userTokens = await loginCommandHandler
            .HandleAsync(
                new LoginUserViaPhoneCommand(new PhoneNumber(transfer.PhoneNumber)),
                cancellationToken)
            .ConfigureAwait(false);

        HttpContext.Response.Cookies.Append(AccessToken, userTokens.AccessToken);

        return new LoginResultDto(userTokens.UserId, userTokens.RefreshToken);
    }
}