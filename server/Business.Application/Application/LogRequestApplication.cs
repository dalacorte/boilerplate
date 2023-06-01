using Business.Application.UOW;
using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Business.Domain.Interfaces.UOW;
using Business.Domain.Models.Others;

namespace Business.Application.Application
{
    public class LogRequestApplication<TEntity, TContext> : BaseApplication<LogRequest, UnitOfWork>, ILogRequestApplication<TEntity, TContext>
        where TEntity : class
        where TContext : IUnitOfWork
    {
        public LogRequestApplication(ILogRequestService<TEntity> logRequestService,
                                     IUnitOfWork unitOfWork) : base(logRequestService, unitOfWork)
        {

        }
    }
}