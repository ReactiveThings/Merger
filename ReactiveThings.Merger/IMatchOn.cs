using System;

namespace ReactiveThings.Merger
{
    public interface IMatchOn<TSource, TTarget>
    {
        IMergerBuilder<TSource, TTarget> On<TKey>(Func<TSource, TKey> sourceItemKeySelector, Func<TTarget, TKey> targetItemKeySelector);
    }
}
