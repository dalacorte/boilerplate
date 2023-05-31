using Business.Domain.Interfaces.Repositories;
using Business.Domain.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Business.Repository.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly RedisConnection _configRedis;
        private readonly ConnectionMultiplexer _connectionRedis;
        private readonly IDatabase _clientRedis;

        private string _redisConnectionString;

        public RedisRepository(IConnectionMultiplexer multiplexer, IOptions<RedisConnection> configRedis)
        {
            _multiplexer = multiplexer;
            _configRedis = configRedis.Value;
            _redisConnectionString = $"{_configRedis.Host}:{_configRedis.Port}";
            _connectionRedis = ConnectionMultiplexer.Connect(_redisConnectionString);
            _clientRedis = _connectionRedis.GetDatabase();
        }

        public async Task Delete(string key)
        {
            if (await _clientRedis.KeyExistsAsync(key))
                await _clientRedis.KeyDeleteAsync(key);
        }

        public ICollection<string> GetAllKeys()
        {
            ICollection<string> keys = new List<string>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
                keys.Add(key);

            return keys.Count() > 0 ? keys : null;
        }

        public async Task<ICollection<string>> GetAllKeysWithValue()
        {
            ICollection<string> values = new List<string>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
                values.Add(await Get(key));

            return values.Count() > 0 ? values : null;
        }

        public async Task<ICollection<T>> GetAllKeysWithValue<T>() where T : new()
        {
            ICollection<T> values = new List<T>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
            {
                T entity = await GetJson<T>(key);
                if (entity is not null && typeof(T) == entity.GetType())
                    values.Add(entity);
            }

            return values.Count() > 0 ? values : null;
        }

        public async Task<string> Get(string key)
        {
            RedisValue values = await _clientRedis.StringGetAsync(key);

            return values.HasValue ? values.ToString() : null;
        }

        public async Task<T> Get<T>(string key)
        {
            RedisValue values = await _clientRedis.StringGetAsync(key);

            return values.HasValue ? (T)Convert.ChangeType(values, typeof(T)) : default(T);
        }

        public async Task<T> GetJson<T>(string key) where T : new()
        {
            try
            {
                RedisValue values = await _clientRedis.StringGetAsync(key);

                return values.HasValue ? JsonConvert.DeserializeObject<T>(values) : default(T);
            }
            catch (JsonReaderException)
            {
                return default(T);
            }
        }

        public async Task<IEnumerable<T>> GetIEnumerableJson<T>(string key)
        {
            try
            {
                RedisValue values = await _clientRedis.StringGetAsync(key);

                return values.HasValue ? JsonConvert.DeserializeObject<IEnumerable<T>>(values) : default(IEnumerable<T>);
            }
            catch (JsonReaderException)
            {
                return default(IEnumerable<T>);
            }
        }

        public async Task Set(string key, string value)
            => await _clientRedis.StringSetAsync(key, value);

        public void Dispose()
           => _connectionRedis?.Dispose();
    }
}
