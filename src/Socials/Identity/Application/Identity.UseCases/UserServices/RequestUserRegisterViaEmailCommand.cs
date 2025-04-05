using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed record RequestUserRegisterViaEmailCommand(Email Email) : IUseCase<bool>;