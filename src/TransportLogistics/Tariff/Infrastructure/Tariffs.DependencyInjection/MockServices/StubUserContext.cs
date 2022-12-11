using TL.SharedKernel.Application.Repositories;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.MockServices;

internal sealed class StubUserContext : IUserContext
{
    public Guid GetProfileId()
    {
        return Guid.Parse("47715058-fbd0-42b5-9b7d-9ef087b324ba");
    }
}