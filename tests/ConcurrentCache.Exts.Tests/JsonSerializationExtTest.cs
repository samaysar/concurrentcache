using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ConcurrentCache.Exts.Tests
{
    [TestFixture]
    public class JsonSerializationExtTest
    {
        [Test]
        public async Task ToJsonAsync_FromJson_Based_On_Stream_Harmonize()
        {
            var obj = new object();
            await obj.ToJsonAsync((Stream)null).ConfigureAwait(false);
        }
    }
}