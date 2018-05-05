using System;
using System.Threading.Tasks;

namespace ReactiveThings.Merger
{
    public interface IMergerBuilder<TSource, TTarget>
    {
        IMergerBuilder<TSource, TTarget> WhenMatched(Action<TSource, TTarget> mergeMatched);
        IMergerBuilder<TSource, TTarget> WhenMatched(Func<TSource, TTarget, Task> mergeMatched);

        IMergerBuilder<TSource, TTarget> WhenNotMatched(Action<TSource> mergeNotMatched);
        IMergerBuilder<TSource, TTarget> WhenNotMatched(Func<TSource, Task> mergeNotMatched);

        IMergerBuilder<TSource, TTarget> WhenNotMatchedBySource(Action<TTarget> mergeNotMatchedBySource);
        IMergerBuilder<TSource, TTarget> WhenNotMatchedBySource(Func<TTarget, Task> mergeNotMatchedBySource);

        Task ExecuteAsync();
    }
}
