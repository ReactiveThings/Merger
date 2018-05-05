using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactiveThings.Merger
{
    public class MergerBuilder<TSource, TTarget, TKey> : IMergerBuilder<TSource, TTarget>
    {
        private Merger<TSource, TTarget, TKey> instance;

        public MergerBuilder(IEnumerable<TSource> source, IEnumerable<TTarget> target, Func<TSource, TKey> sourceItemKeySelector, Func<TTarget, TKey> targetItemKeySelector)
        {
            instance = new Merger<TSource, TTarget, TKey>(source, target, sourceItemKeySelector, targetItemKeySelector);
        }

        public IMergerBuilder<TSource, TTarget> WhenMatched(Action<TSource, TTarget> mergeMatched)
        {
            WhenMatched((sourceItem, targetItem) =>
            {
                mergeMatched(sourceItem, targetItem);
                return Task.CompletedTask;
            });
            return this;
        }

        public IMergerBuilder<TSource, TTarget> WhenMatched(Func<TSource, TTarget, Task> mergeMatched)
        {
            instance.MergeMatched = mergeMatched;
            return this;
        }

        public IMergerBuilder<TSource, TTarget> WhenNotMatched(Action<TSource> mergeNotMatched)
        {
            WhenNotMatched((sourceItem) =>
            {
                mergeNotMatched(sourceItem);
                return Task.CompletedTask;
            });
            return this;
        }

        public IMergerBuilder<TSource, TTarget> WhenNotMatched(Func<TSource, Task> mergeNotMatched)
        {
            instance.MergeNotMatched = mergeNotMatched;
            return this;
        }

        public IMergerBuilder<TSource, TTarget> WhenNotMatchedBySource(Action<TTarget> mergeNotMatchedBySource)
        {
            WhenNotMatchedBySource((targetItem) =>
            {
                mergeNotMatchedBySource(targetItem);
                return Task.CompletedTask;
            });
            return this;
        }

        public IMergerBuilder<TSource, TTarget> WhenNotMatchedBySource(Func<TTarget, Task> mergeNotMatchedBySource)
        {
            instance.MergeNotMatchedBySource = mergeNotMatchedBySource;
            return this;
        }

        public async Task ExecuteAsync()
        {
            await instance.MergeAsync();
        }

    }
}
