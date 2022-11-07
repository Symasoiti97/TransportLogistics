using Application.Abstracts.Repositories;

namespace TL.SharedKernel.Infrastructure.DataAccess;

internal interface IRepositoryFactory
{
    TRepository Get<TRepository>() where TRepository : IRepository;
}