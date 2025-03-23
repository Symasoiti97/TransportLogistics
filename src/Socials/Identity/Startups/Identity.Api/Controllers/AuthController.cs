using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Startups.Api.Models;

namespace TL.Socials.Identity.Startups.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login/password")]
    public async Task<LoginResultDto> Login(
        [FromBody] LoginUserByPasswordDto loginUserByPassword,
        [FromServices] IQueryHandler<LoginUserByPasswordCommand, UserTokens> commandHandler,
        CancellationToken cancellationToken)
    {
        var userTokens = await commandHandler.HandleAsync(
            new LoginUserByPasswordCommand(
                new Email(loginUserByPassword.Email),
                loginUserByPassword.Password),
            cancellationToken);

        HttpContext.Response.Cookies.Append("access_token", userTokens.AccessToken);

        return new LoginResultDto(userTokens.UserId, userTokens.RefreshToken);
    }

    [HttpPost("register/password")]
    public Task RegisterByPassword(
        [FromBody] RegisterUserByPasswordDto registerUserByPassword,
        [FromServices] ICommandHandler<RegisterUserWithPasswordCommand> commandHandler,
        CancellationToken cancellationToken)
    {
        return commandHandler.HandleAsync(
            new RegisterUserWithPasswordCommand(
                new Email(registerUserByPassword.Email),
                registerUserByPassword.Password),
            cancellationToken);
    }
}