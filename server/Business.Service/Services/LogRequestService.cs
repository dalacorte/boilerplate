using Business.Domain.Interfaces.Repositories;
using Business.Domain.Interfaces.Services;
using Business.Domain.Models.Others;

namespace Business.Service.Services
{
    public class LogRequestService<TEntity> : BaseService<LogRequest>, ILogRequestService<TEntity>
        where TEntity : class
    {
        public LogRequestService(ILogRequestRepository<TEntity> logRequestRepository) : base(logRequestRepository)
        {

        }
    }
}
