using System;
using ConcurrentCache.Contracts.Error;
using NUnit.Framework;

namespace ConcurrentCache.Contracts.Tests.Error
{
    [TestFixture]
    public class ConcurrentCacheExceptionTest
    {
        [Test]
        [TestCase(CacheErrorCode.UnknownError)]
        [TestCase(CacheErrorCode.ConfigError)]
        [TestCase(CacheErrorCode.UnitTestRelated)]
        public void Ctor_Sets_Error_Code(CacheErrorCode code)
        {
            Assert.True(new ConcurrentCacheException(code).Code.Equals(code));
            Assert.True(new ConcurrentCacheException(code, "Test").Code.Equals(code));
            Assert.True(new ConcurrentCacheException(code, "Test", new Exception()).Code.Equals(code));
        }

        [Test]
        [TestCase(CacheErrorCode.UnknownError)]
        [TestCase(CacheErrorCode.ConfigError)]
        [TestCase(CacheErrorCode.UnitTestRelated)]
        public void Ctor_Error_Message_Has_Fixed_Known_Format(CacheErrorCode code)
        {
            const string errMessage = "some error message related text.";
            Assert.True(new ConcurrentCacheException(code).Message.Equals(code.ToString("G")));
            Assert.True(
                new ConcurrentCacheException(code, errMessage).Message.Equals($"{code.ToString("G")}:{errMessage}"));
            Assert.True(
                new ConcurrentCacheException(code, errMessage, new Exception()).Message.Equals(
                    $"{code.ToString("G")}:{errMessage}"));
        }

        [Test]
        public void Ctor_Passes_Inner_Exception_Down_The_Lane()
        {
            var err = new Exception();
            var innerException =
                new ConcurrentCacheException(CacheErrorCode.UnitTestRelated, "anything", err).InnerException;
            Assert.NotNull(innerException);
            Assert.True(innerException.Equals(err));
        }
    }
}