using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConcurrentCache.Exts
{
    public enum CompressionScheme
    {
        Deflate = 0,
        GZip
    }

    public static class JsonSerializationExt
    {
        public static Task SerializeAsync<T>(this T obj, Stream outputStream, bool compress = true,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            return compress ? obj.ToCompressedJson(outputStream, scheme, level) : obj.ToJson(outputStream);
        }

        public static Task<T> DeserializeAsync<T>(this Stream inputStream, bool compressed = true,
            CompressionScheme scheme = CompressionScheme.Deflate)
        {
            return Task.FromResult(compressed ? inputStream.FromCompressedJson<T>(scheme) : inputStream.FromJson<T>());
        }

        public static async Task ToJson<T>(this T obj, Stream outputStream)
        {
            using (var streamWriter = new StreamWriter(outputStream, Encoding.UTF8, 1024, true) {AutoFlush = true})
            {
                await obj.ToJson(streamWriter).ConfigureAwait(false);
                await streamWriter.FlushAsync().ConfigureAwait(false);
                await outputStream.FlushAsync().ConfigureAwait(false);
            }
        }

        public static T FromJson<T>(this Stream inputStream)
        {
            using (var streamReader = new StreamReader(inputStream, Encoding.UTF8, true, 1024, true))
            {
                return streamReader.FromJson<T>();
            }
        }

        public static async Task ToCompressedJson<T>(this T obj, Stream compressedOutputStream,
            CompressionScheme scheme = CompressionScheme.Deflate, CompressionLevel level = CompressionLevel.Optimal)
        {
            var compressor = scheme == CompressionScheme.Deflate
                ?(Stream)new DeflateStream(compressedOutputStream, level, true)
                :new GZipStream(compressedOutputStream, level, true);
            using (compressor)
            {
                using (var streamWriter = new StreamWriter(compressor, Encoding.UTF8, 1024, true) {AutoFlush = true})
                {
                    await obj.ToJson(streamWriter).ConfigureAwait(false);
                    await streamWriter.FlushAsync().ConfigureAwait(false);
                    await compressor.FlushAsync().ConfigureAwait(false);
                    await compressedOutputStream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        public static T FromCompressedJson<T>(this Stream compressedInputStream,
            CompressionScheme scheme = CompressionScheme.Deflate)
        {
            var compressor = scheme == CompressionScheme.Deflate
                ?(Stream)new DeflateStream(compressedInputStream, CompressionMode.Decompress, true)
                :new GZipStream(compressedInputStream, CompressionMode.Decompress, true);
            using (compressor)
            {
                using (var streamReader = new StreamReader(compressor, Encoding.UTF8, true, 1024, true))
                {
                    return streamReader.FromJson<T>();
                }
            }
        }

        public static async Task ToJson<T>(this T obj, StreamWriter streamWriter)
        {
            streamWriter.AutoFlush = true;
            using (var jsonWriter = new JsonTextWriter(streamWriter)
            {
                CloseOutput = false,
                Culture = CultureInfo.CurrentCulture,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            })
            {
                var serializer = new JsonSerializer
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    Formatting = Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    Culture = CultureInfo.CurrentCulture,
                    TypeNameHandling = TypeNameHandling.Auto,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
                };
                serializer.Serialize(jsonWriter, obj);
                jsonWriter.Flush();
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        public static T FromJson<T>(this StreamReader streamReader)
        {
            using (var jsonReader = new JsonTextReader(streamReader)
            {
                Culture = CultureInfo.CurrentCulture,
                CloseInput = false,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            })
            {
                var serializer = new JsonSerializer
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    Formatting = Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    Culture = CultureInfo.CurrentCulture,
                    TypeNameHandling = TypeNameHandling.Auto,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
                };
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}