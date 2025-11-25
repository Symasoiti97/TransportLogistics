using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

public sealed record LoginUserViaPhoneCommand(PhoneNumber PhoneNumber) : IUseCase<UserTokens>;
