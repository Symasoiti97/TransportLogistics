using TL.SharedKernel.Application.Repositories;

namespace TL.SharedKernel.Infrastructure.DataAccess;

internal interface IRepositoryFactory
{
    TRepository Get<TRepository>() where TRepository : IRepository;
}