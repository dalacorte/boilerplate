using Business.Domain.Interfaces.Mongo;
using Business.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using ServiceStack;

namespace Business.Repository.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        protected readonly IMongoContext _context;
        protected IMongoCollection<TEntity> DbSet;

        protected BaseRepository(IMongoContext context)
        {
            _context = context;
            DbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual void Insert(TEntity entity)
            => _context.AddCommand(() => DbSet.InsertOneAsync(entity));

        public virtual void Update(TEntity entity)
            => _context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", entity.GetId()), entity));

        public virtual void DeleteById(Guid id)
            => _context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id)));

        public virtual async Task<TEntity> GetById(Guid id)
        {
            IAsyncCursor<TEntity> data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            IAsyncCursor<TEntity> all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public void Dispose()
            => _context?.Dispose();
    }
}
