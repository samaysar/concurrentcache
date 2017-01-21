using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConcurrentCache.Exts
{
    public static class XmlSerializationExt
    {
        public static Task SerializeXmlAsync<T>(this T obj, Stream outputStream, IEnumerable<Type> knownTypes,
            bool compress = true, CompressionScheme scheme = CompressionScheme.Deflate,
            CompressionLevel level = CompressionLevel.Optimal)
        {
            return compress
                ?obj.ToCompressedXmlAsync(outputStream, knownTypes, scheme, level)
                :obj.ToXmlAsync(outputStream, knownTypes);
        }

        public static Task<T> DeserializeXmlAsync<T>(this Stream inputStream, IEnumerable<Type> knownTypes,
            bool compressed = true, CompressionScheme scheme = CompressionScheme.Deflate)
        {
            return
                Task.FromResult(compressed
                    ?inputStream.FromCompressedXml<T>(knownTypes, scheme)
                    :inputStream.FromXml<T>(knownTypes));
        }

        public static async Task ToXmlAsync<T>(this T obj, Stream outputStream, IEnumerable<Type> knownTypes)
        {
            using (var streamWriter = new StreamWriter(outputStream, Encoding.UTF8, 1024, true) {AutoFlush = true})
            {
                await obj.ToXmlAsync(streamWriter, knownTypes).ConfigureAwait(false);
                await streamWriter.FlushAsync().ConfigureAwait(false);
                await outputStream.FlushAsync().ConfigureAwait(false);
            }
        }

        public static T FromXml<T>(this Stream inputStream, IEnumerable<Type> knownTypes)
        {
            using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 1024, true))
            {
                return streamReader.FromXml<T>(knownTypes);
            }
        }

        public static async Task ToCompressedXmlAsync<T>(this T obj, Stream compressedOutputStream,
            IEnumerable<Type> knownTypes, CompressionScheme scheme = CompressionScheme.Deflate,
            CompressionLevel level = CompressionLevel.Optimal)
        {
            var compressor = scheme == CompressionScheme.Deflate
                ?(Stream)new DeflateStream(compressedOutputStream, level, true)
                :new GZipStream(compressedOutputStream, level, true);
            using (compressor)
            {
                using (var streamWriter = new StreamWriter(compressor, Encoding.UTF8, 1024, true) {AutoFlush = true})
                {
                    await obj.ToXmlAsync(streamWriter, knownTypes).ConfigureAwait(false);
                    await streamWriter.FlushAsync().ConfigureAwait(false);
                    await compressor.FlushAsync().ConfigureAwait(false);
                    await compressedOutputStream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        public static T FromCompressedXml<T>(this Stream compressedInputStream, IEnumerable<Type> knownTypes,
            CompressionScheme scheme = CompressionScheme.Deflate)
        {
            var compressor = scheme == CompressionScheme.Deflate
                ?(Stream)new DeflateStream(compressedInputStream, CompressionMode.Decompress, true)
                :new GZipStream(compressedInputStream, CompressionMode.Decompress, true);
            using (compressor)
            {
                using (var streamReader = new StreamReader(compressor, Encoding.UTF8, true, 1024, true))
                {
                    return streamReader.FromXml<T>(knownTypes);
                }
            }
        }

        public static async Task ToXmlAsync<T>(this T obj, StreamWriter streamWriter, IEnumerable<Type> knownTypes)
        {
            streamWriter.AutoFlush = true;
            using (
                var xmlWriter = XmlWriter.Create(streamWriter,
                    new XmlWriterSettings {CloseOutput = false, Async = true, Encoding = Encoding.UTF8, Indent = false})
                )
            {
                var serializer = new DataContractSerializer(obj.GetType(), knownTypes);
                serializer.WriteObject(xmlWriter, obj);
                await xmlWriter.FlushAsync().ConfigureAwait(false);
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        public static T FromXml<T>(this StreamReader streamReader, IEnumerable<Type> knownTypes)
        {
            using (
                var xmlWriter = XmlReader.Create(streamReader, new XmlReaderSettings {CloseInput = false, Async = true})
                )
            {
                var serializer = new DataContractSerializer(typeof (T), knownTypes);
                return (T)serializer.ReadObject(xmlWriter);
            }
        }
    }
}