using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

// TODO: Uses IQuery, because ICommand cannot have a result model
public sealed record LoginUserByPasswordCommand(Email Email, string Password) : IQuery<UserTokens>;