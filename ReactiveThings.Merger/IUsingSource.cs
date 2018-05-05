using System.Collections.Generic;

namespace ReactiveThings.Merger
{
    public interface IUsingSource<TTarget>
    {
        IMatchOn<TSource, TTarget> Using<TSource>(IEnumerable<TSource> source);
    }
}
