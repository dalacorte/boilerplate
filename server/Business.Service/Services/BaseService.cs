using Business.Domain.Interfaces.Repositories;
using Business.Domain.Interfaces.Services;

namespace Business.Service.Services
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity>
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity> _repository;

        protected BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual void Insert(TEntity entity)
            => _repository.Insert(entity);

        public virtual void Update(TEntity entity)
            => _repository.Update(entity);

        public virtual void DeleteById(Guid id)
            => _repository.DeleteById(id);

        public virtual async Task<TEntity> GetById(Guid id)
            => await _repository.GetById(id);

        public virtual async Task<IEnumerable<TEntity>> GetAll()
            => await _repository.GetAll();
    }
}
