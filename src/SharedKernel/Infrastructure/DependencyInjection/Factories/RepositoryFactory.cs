using Application.Abstracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Infrastructure.DataAccess;

namespace TL.SharedKernel.Infrastructure.DependencyInjection.Factories;

internal class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TRepository Get<TRepository>() where TRepository : IRepository
    {
        return _serviceProvider.GetRequiredService<TRepository>();
    }
}