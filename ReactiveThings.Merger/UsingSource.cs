using System.Collections.Generic;

namespace ReactiveThings.Merger
{
    public class UsingSource<TTarget> : IUsingSource<TTarget>
    {
        private readonly IEnumerable<TTarget> target;

        public UsingSource(IEnumerable<TTarget> target)
        {
            this.target = target;
        }

        public IMatchOn<TSource, TTarget> Using<TSource>(IEnumerable<TSource> source)
        {
            return new MatchOn<TSource, TTarget>(target, source);
        }
    }
}
