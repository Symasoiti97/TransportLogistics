using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

public sealed record RefreshAuthTokenCommand(string RefreshToken) : IUseCase<UserTokens>;
