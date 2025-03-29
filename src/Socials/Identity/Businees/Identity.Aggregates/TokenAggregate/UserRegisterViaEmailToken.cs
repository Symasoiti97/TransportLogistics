using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserRegisterViaEmailToken : AggregateRoot<Guid>
{
    public UserRegisterViaEmailToken(Guid id, Email email) : base(id)
    {
        ArgumentNullException.ThrowIfNull(email);

        Email = email;
    }

    public Email Email { get; }

    public static UserRegisterViaEmailToken Create(Email email)
    {
        var token = new UserRegisterViaEmailToken(Guid.NewGuid(), email);
        return token;
    }
}