using System.Threading.Tasks;

namespace ConcurrentCache.Contracts.Assets
{
    public interface ISerializer<T>
    {
        Task SerializeAsync(T obj);
        Task<T> DeserializeAsync();
    }
}