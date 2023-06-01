using Business.Domain.Interfaces.Mongo;
using Business.Domain.Interfaces.Repositories;
using Business.Domain.Models.Others;

namespace Business.Repository.Repositories
{
    public class LogRequestRepository<TEntity> : BaseRepository<LogRequest>, ILogRequestRepository<TEntity>
        where TEntity : class
    {
        public LogRequestRepository(IMongoContext context) : base(context)
        {

        }
    }
}
