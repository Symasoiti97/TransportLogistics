using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed record RegisterUserWithPasswordCommand(Email Email, string Password) : IUseCase;