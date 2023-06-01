using Business.Domain.Models.Others;

namespace Business.Domain.Interfaces.Repositories
{
    public interface ILogRequestRepository<TEntity> : IBaseRepository<LogRequest>
        where TEntity : class
    {

    }
}
