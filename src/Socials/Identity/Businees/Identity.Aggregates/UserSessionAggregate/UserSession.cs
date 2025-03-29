using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

public sealed class UserSession : AggregateRoot<Guid>
{
    public UserSession(Guid id, Guid userId, string refreshToken) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        UserId = userId;
        RefreshToken = refreshToken;
    }

    public Guid UserId { get; }
    public string RefreshToken { get; private set; }

    public static UserSession Create(Guid userId, string refreshToken)
    {
        return new UserSession(Guid.NewGuid(), userId, refreshToken);
    }

    public void UpdateRefreshToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        RefreshToken = refreshToken;
    }
}