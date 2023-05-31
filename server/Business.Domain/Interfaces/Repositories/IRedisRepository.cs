namespace Business.Domain.Interfaces.Repositories
{
    public interface IRedisRepository : IDisposable
    {
        Task Set(string key, string value);
        ICollection<string> GetAllKeys();
        Task<ICollection<string>> GetAllKeysWithValue();
        Task<ICollection<T>> GetAllKeysWithValue<T>() where T : new();
        Task<string> Get(string key);
        Task<T> Get<T>(string key);
        Task<T> GetJson<T>(string key) where T : new();
        Task<IEnumerable<T>> GetIEnumerableJson<T>(string key);
        Task Delete(string key);
    }
}
