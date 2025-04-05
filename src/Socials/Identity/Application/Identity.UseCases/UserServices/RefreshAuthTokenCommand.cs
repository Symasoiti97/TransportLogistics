using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed record RefreshAuthTokenCommand(string RefreshToken) : IUseCase<UserTokens>;