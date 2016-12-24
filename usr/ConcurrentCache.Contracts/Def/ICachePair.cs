namespace ConcurrentCache.Contracts.Def
{
    public interface ICachePair<out TK, out TV>
    {
        TK Key { get; }
        TV Value { get; }
    }
}