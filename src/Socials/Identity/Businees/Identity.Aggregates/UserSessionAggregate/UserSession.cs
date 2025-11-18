using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

public sealed class UserSession : AggregateRoot<Guid>
{
    public Guid UserId { get; }
    public string RefreshToken { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public UserSession(
        Guid id,
        Guid userId,
        string refreshToken,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        UserId = userId;
        RefreshToken = refreshToken;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static UserSession Create(Guid userId, string refreshToken)
    {
        var createdAt = DateTimeOffset.UtcNow;
        var expiresAt = createdAt.Add(FromDays);
        return new(Guid.NewGuid(), userId, refreshToken, createdAt, expiresAt);
    }

    public void UpdateRefreshToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        RefreshToken = refreshToken;
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = CreatedAt.Add(FromDays);
    }

    private static TimeSpan FromDays => TimeSpan.FromDays(30);
}