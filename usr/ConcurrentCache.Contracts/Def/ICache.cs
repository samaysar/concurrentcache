﻿using System.Collections.Generic;

namespace ConcurrentCache.Contracts.Def
{
    public interface ICache<TK, TV> : IEnumerable<ICachePair<TK, TV>>
    {
        bool TryGet(TK key, out ICachePair<TK, TV> value);
        bool TryRemove(TK key, out ICachePair<TK, TV> removedValue);
        ICachePair<TK, TV> AddOrGet(TK key, TV value);
        bool TryAdd(TK key, TV value);
        bool TryUpdate(TK key, TV value, out ICachePair<TK, TV> replacedValue);
    }
}