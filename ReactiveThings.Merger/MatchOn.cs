using System;
using System.Collections.Generic;

namespace ReactiveThings.Merger
{
    public class MatchOn<TSource, TTarget> : IMatchOn<TSource, TTarget>
    {
        private readonly IEnumerable<TTarget> target;
        private readonly IEnumerable<TSource> source;
        public MatchOn(IEnumerable<TTarget> target, IEnumerable<TSource> source)
        {
            this.target = target;
            this.source = source;
        }
        public IMergerBuilder<TSource, TTarget> On<TKey>(Func<TSource, TKey> sourceItemKeySelector, Func<TTarget, TKey> targetItemKeySelector)
        {
            return new MergerBuilder<TSource, TTarget, TKey>(source, target, sourceItemKeySelector, targetItemKeySelector);
        }
    }
}
