using System;

namespace ConcurrentCache.Contracts.Error
{
    public sealed class ConcurrentCacheException : Exception
    {
        public CacheErrorCode Code { get; }

        public ConcurrentCacheException(CacheErrorCode code) : base($"{code.ToString("G")}")
        {
            Code = code;
        }

        public ConcurrentCacheException(CacheErrorCode code, string message) : base($"{code.ToString("G")}:{message}")
        {
            Code = code;
        }

        public ConcurrentCacheException(CacheErrorCode code, string message, Exception e) : base($"{code.ToString("G")}:{message}", e)
        {
            Code = code;
        }
    }
}