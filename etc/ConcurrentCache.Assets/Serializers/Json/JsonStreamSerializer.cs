using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ConcurrentCache.Contracts.Assets;
using ConcurrentCache.Exts;

namespace ConcurrentCache.Assets.Serializers.Json
{
    public class JsonStreamSerializer<T> : ISerializer<T>
    {
        private readonly Stream _stream;
        private readonly bool _useCompression;
        private readonly CompressionScheme _scheme;
        private readonly CompressionLevel _level;

        public JsonStreamSerializer(Stream stream, bool useCompression = true,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            _stream = stream;
            _useCompression = useCompression;
            _scheme = scheme;
            _level = level;
        } 

        public virtual async Task SerializeAsync(T obj)
        {
            await obj.SerializeJsonAsync(_stream, _useCompression, _scheme, _level).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> DeserializeAsync()
        {
            return await _stream.DeserializeJsonAsync<T>(_useCompression, _scheme).ConfigureAwait(false);
        }
    }
}