using Business.Domain.Interfaces.Mongo;
using Business.Domain.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Business.Database
{
    public class MongoContext : IMongoContext
    {
        private IMongoDatabase _database { get; set; }
        private IMongoClient _client { get; set; }
        public IClientSessionHandle _session { get; set; }
        private readonly List<Func<Task>> _commands;

        public MongoContext(IOptions<MongoConnection> settings)
        {
            //_client = new MongoClient(new MongoClientSettings
            //{
            //    Server = new MongoServerAddress("127.0.0.1", 27017),
            //    ClusterConfigurator = cluster =>
            //    {
            //        cluster.Subscribe<CommandStartedEvent>(command =>
            //        {
            //            string query = command.Command.ToJson(new JsonWriterSettings { Indent = true });
            //        });
            //    }
            //});

            _commands = new List<Func<Task>>();
            _client = new MongoClient(settings.Value.ConnectionString);

            if (_client is not null)
                _database = _client.GetDatabase(settings.Value.Database);
        }

        public void AddCommand(Func<Task> func)
            => _commands.Add(func);

        public IMongoCollection<T> GetCollection<T>(string name)
            => _database.GetCollection<T>(name);

        public async Task<int> SaveChanges()
        {
            using (_session = await _client.StartSessionAsync())
            {
                _session.StartTransaction();

                IEnumerable<Task> commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await _session.CommitTransactionAsync();
            }

            int count = _commands.Count;
            _commands.Clear();

            return count;
        }

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}