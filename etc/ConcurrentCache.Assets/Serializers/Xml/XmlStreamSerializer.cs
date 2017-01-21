using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ConcurrentCache.Contracts.Assets;
using ConcurrentCache.Exts;

namespace ConcurrentCache.Assets.Serializers.Xml
{
    public class XmlStreamSerializer<T> : ISerializer<T>
    {
        private readonly Stream _stream;
        private readonly IEnumerable<Type> _knownTypes;
        private readonly bool _useCompression;
        private readonly CompressionScheme _scheme;
        private readonly CompressionLevel _level;

        public XmlStreamSerializer(Stream stream, IEnumerable<Type> knownTypes, bool useCompression = true,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            _stream = stream;
            _knownTypes = knownTypes;
            _useCompression = useCompression;
            _scheme = scheme;
            _level = level;
        }

        public virtual async Task SerializeAsync(T obj)
        {
            await obj.SerializeXmlAsync(_stream, _knownTypes, _useCompression, _scheme, _level).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> DeserializeAsync()
        {
            return await _stream.DeserializeXmlAsync<T>(_knownTypes, _useCompression, _scheme).ConfigureAwait(false);
        }
    }
}