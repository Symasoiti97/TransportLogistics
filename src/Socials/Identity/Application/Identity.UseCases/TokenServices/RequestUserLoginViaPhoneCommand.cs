using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

public sealed record RequestUserLoginViaPhoneCommand(PhoneNumber PhoneNumber) : IUseCase;
