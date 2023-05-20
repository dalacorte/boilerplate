using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Business.Domain.Interfaces.UOW;

namespace Business.Application.Application
{
    public class BaseApplication<TEntity, TContext> : IBaseApplication<TEntity, TContext>
        where TEntity : class
        where TContext : IUnitOfWork
    {
        protected readonly IBaseService<TEntity> _service;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseApplication(IBaseService<TEntity> service,
                                  IUnitOfWork unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Insert(TEntity entity)
        {
            _service.Insert(entity);
            await _unitOfWork.Commit();
        }

        public virtual async Task Update(TEntity entity)
        {
            _service.Update(entity);
            await _unitOfWork.Commit();
        }

        public virtual async Task DeleteById(Guid id)
        {
            _service.DeleteById(id);
            await _unitOfWork.Commit();
        }

        public virtual async Task<TEntity> GetById(Guid id)
            => await _service.GetById(id);

        public virtual async Task<IEnumerable<TEntity>> GetAll()
            => await _service.GetAll();
    }
}
