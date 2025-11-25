using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserRegisterViaEmailTokenCreated(Guid id, Guid tokenId) : Event(id), IUseCase
{
    public Guid TokenId { get; } = tokenId;
}
