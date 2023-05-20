namespace Business.Domain.Interfaces.Services
{
    public interface IBaseService<TEntity> 
        where TEntity : class
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void DeleteById(Guid id);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
    }
}
