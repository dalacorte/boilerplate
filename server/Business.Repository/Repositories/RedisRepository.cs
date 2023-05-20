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

        public List<string> GetAllKeys()
        {
            List<string> keys = new List<string>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
                keys.Add(key);

            return keys.Count() > 0 ? keys : null;
        }

        public async Task<List<string>> GetAllKeysWithValue()
        {
            List<string> value = new List<string>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
                value.Add(await Get(key));

            return value.Count() > 0 ? value : null;
        }

        public async Task<List<T>> GetAllKeysWithValue<T>() where T : new()
        {
            List<T> value = new List<T>();

            foreach (RedisKey key in _multiplexer.GetServer(_redisConnectionString).Keys(pattern: "*"))
                value.Add(await GetJson<T>(key));

            return value.Count() > 0 ? value : null;
        }

        public async Task<string> Get(string key)
        {
            RedisValue value = await _clientRedis.StringGetAsync(key);

            return value.HasValue ? value.ToString() : null;
        }

        public async Task<T> Get<T>(string key)
        {
            RedisValue value = await _clientRedis.StringGetAsync(key);

            return value.HasValue ? (T)Convert.ChangeType(value, typeof(T)) : default(T);
        }

        public async Task<T> GetJson<T>(string key) where T : new()
        {
            RedisValue value = await _clientRedis.StringGetAsync(key);

            return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default(T);
        }

        public async Task<IEnumerable<T>> GetIEnumerableJson<T>(string key)
        {
            RedisValue value = await _clientRedis.StringGetAsync(key);

            return value.HasValue ? JsonConvert.DeserializeObject<IEnumerable<T>>(value) : default(IEnumerable<T>);
        }

        public async Task Set(string key, string value)
            => await _clientRedis.StringSetAsync(key, value);

        public void Dispose()
           => _connectionRedis?.Dispose();
    }
}
