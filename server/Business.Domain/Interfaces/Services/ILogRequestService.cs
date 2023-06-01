using Business.Domain.Models.Others;

namespace Business.Domain.Interfaces.Services
{
    public interface ILogRequestService<TEntity> : IBaseService<LogRequest>
        where TEntity : class
    {

    }
}
