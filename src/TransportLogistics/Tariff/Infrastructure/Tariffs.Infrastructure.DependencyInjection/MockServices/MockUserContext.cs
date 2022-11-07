using Application.Abstracts;

namespace Tariffs.Infrastructure.DependencyInjection.MockServices;

internal sealed class MockUserContext : IUserContext
{
    public Guid GetProfileId()
    {
        return Guid.Parse("47715058-fbd0-42b5-9b7d-9ef087b324ba");
    }
}