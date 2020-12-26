using System;
using System.Collections.Generic;
using System.Linq;

namespace Plando.Common
{
    public interface IAggregator<TAggregate> where TAggregate : new()
    {
        TAggregate Push(TAggregate aggregate);
        DateTime CreatedAt { get; set; }
    }

    public static class AggregationExtensions
    {
        public static TAggregate Aggregate<TAggregate>(
            this IEnumerable<IAggregator<TAggregate>> aggregators)
            where TAggregate : new()
        {
            var aggregate = new TAggregate();

            foreach (var aggregator in aggregators
                .OrderBy(x => x.CreatedAt)
                .AsEnumerable())
            {
                aggregate = aggregator.Push(aggregate);
            }

            return aggregate;
        }
    }
}