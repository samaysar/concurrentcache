using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ConcurrentCache.Contracts.Assets;
using ConcurrentCache.Exts;

namespace ConcurrentCache.Assets.Serializers.Json
{
    public class JsonFileSerializer<T> : ISerializer<T>
    {
        private readonly string _fileFullPath;
        private readonly bool _useCompression;
        private readonly CompressionScheme _scheme;
        private readonly CompressionLevel _level;

        public JsonFileSerializer(string fileFullPath, bool useCompression = true,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            _fileFullPath = fileFullPath;
            _useCompression = useCompression;
            _scheme = scheme;
            _level = level;
        }

        public virtual async Task SerializeAsync(T obj)
        {
            using (
                var fileReader = new FileStream(_fileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                    1024, FileOptions.Asynchronous))
            {
                await
                    new JsonStreamSerializer<T>(fileReader, _useCompression, _scheme, _level).SerializeAsync(obj)
                        .ConfigureAwait(false);
                fileReader.Flush(true);
            }
        }

        public virtual async Task<T> DeserializeAsync()
        {
            using (
                var fileWriter = new FileStream(_fileFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None,
                    1024, FileOptions.Asynchronous))
            {
                return
                    await
                        new JsonStreamSerializer<T>(fileWriter, _useCompression, _scheme, _level).DeserializeAsync()
                            .ConfigureAwait(false);
            }
        }
    }
}