using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed record RegisterUserViaEmailCommand(Email Email, string Password, string Token) : IUseCase;
