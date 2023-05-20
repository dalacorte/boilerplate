using Business.Domain.Interfaces.UOW;

namespace Business.Domain.Interfaces.Application
{
    public interface IBaseApplication<TEntity, TContext>
        where TEntity : class
        where TContext : IUnitOfWork
    {
        Task Insert(TEntity entity);
        Task Update(TEntity entity);
        Task DeleteById(Guid id);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
    }
}
