using Business.Domain.Interfaces.UOW;
using Business.Domain.Models.Others;

namespace Business.Domain.Interfaces.Application
{
    public interface ILogRequestApplication<TEntity, TContext> : IBaseApplication<LogRequest, TContext>
        where TEntity : class
        where TContext : IUnitOfWork
    {

    }
}
