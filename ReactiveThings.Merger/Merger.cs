using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactiveThings.Merger
{
    public static class Merger
    {
        public static IMatchOn<TSource, TTarget> MergeUsing<TSource, TTarget>(this IEnumerable<TTarget> target, IEnumerable<TSource> source)
        {
            return Merge(target).Using(source);
        }
        public static IUsingSource<TTarget> Merge<TTarget>(IEnumerable<TTarget> target)
        {
            return new UsingSource<TTarget>(target);
        }
    }

    public class Merger<TSource, TTarget, TKey>
    {
        public Func<TSource, Task> MergeNotMatched { get; set; } = model => Task.CompletedTask;
        public Func<TSource, TTarget, Task> MergeMatched { get; set; } = (entity, model) => Task.CompletedTask;
        public Func<TTarget, Task> MergeNotMatchedBySource { get; set; } = model => Task.CompletedTask;

        IEnumerable<TSource> source;
        IEnumerable<TTarget> target;
        Func<TSource, TKey> sourceItemKeySelector;
        Func<TTarget, TKey> targetItemKeySelector;

        public Merger(IEnumerable<TSource> source, 
            IEnumerable<TTarget> target,
            Func<TSource, TKey> sourceItemKeySelector,
            Func<TTarget, TKey> targetItemKeySelector
            )
        {
            this.source = source;
            this.target = target;
            this.sourceItemKeySelector = sourceItemKeySelector;
            this.targetItemKeySelector = targetItemKeySelector;
        }

        public async Task MergeAsync()
        {
            ThrowWhenTargetItemMatchesMoreThanOneSourceItem(source, sourceItemKeySelector);

            var notMatchedBySourceItems = target.ToDictionary(targetItemKeySelector, p => p);
            foreach (var sourceItem in source)
            {
                var targetItem = FindTargetItemByKey(notMatchedBySourceItems, sourceItemKeySelector(sourceItem));
                if (targetItem == null)
                {
                    await MergeNotMatched(sourceItem).ConfigureAwait(false);
                }
                else
                {
                    await MergeMatched(sourceItem, targetItem).ConfigureAwait(false);
                }
            }

            foreach (var sourceItem in notMatchedBySourceItems)
            {
                await MergeNotMatchedBySource(sourceItem.Value).ConfigureAwait(false);
            }
        }

        private void ThrowWhenTargetItemMatchesMoreThanOneSourceItem(IEnumerable<TSource> source, Func<TSource, TKey> sourceItemKeySelector)
        {
            var notNullKeys = source.Select(sourceItemKeySelector).Where(sourceItemKey => sourceItemKey != null);
            if (notNullKeys.Distinct().Count() != notNullKeys.Count())
            {
                throw new ArgumentException("Target item matches more than one source item");
            }
        }

        private TTarget FindTargetItemByKey(Dictionary<TKey, TTarget> notMatchedBySourceItems, TKey key)
        {
            TTarget result = default(TTarget);

            if (key != null && notMatchedBySourceItems.TryGetValue(key, out result))
            {
                notMatchedBySourceItems.Remove(key);
            }

            return result;
        }
    }
}
