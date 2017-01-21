using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ConcurrentCache.Assets.Serializers.Json;
using ConcurrentCache.Contracts.Assets;
using ConcurrentCache.Exts;

namespace ConcurrentCache.Assets.Serializers.Xml
{
    public class XmlFileSerializer<T> : ISerializer<T>
    {
        private readonly string _fileFullPath;
        private readonly IEnumerable<Type> _knownTypes;
        private readonly bool _useCompression;
        private readonly CompressionScheme _scheme;
        private readonly CompressionLevel _level;

        public XmlFileSerializer(string fileFullPath, IEnumerable<Type> knownTypes, bool useCompression = true,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            _fileFullPath = fileFullPath;
            _knownTypes = knownTypes;
            _useCompression = useCompression;
            _scheme = scheme;
            _level = level;
        }

        public virtual async Task SerializeAsync(T obj)
        {
            using (
                var fileStream = new FileStream(_fileFullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                    1024, FileOptions.Asynchronous))
            {
                await
                    new XmlStreamSerializer<T>(fileStream, _knownTypes, _useCompression, _scheme, _level).SerializeAsync(obj)
                        .ConfigureAwait(false);
                fileStream.Flush(true);
            }
        }

        public virtual async Task<T> DeserializeAsync()
        {
            using (
                var fileStream = new FileStream(_fileFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None,
                    1024, FileOptions.Asynchronous))
            {
                return
                    await
                        new XmlStreamSerializer<T>(fileStream, _knownTypes, _useCompression, _scheme, _level).DeserializeAsync()
                            .ConfigureAwait(false);
            }
        }
    }
}