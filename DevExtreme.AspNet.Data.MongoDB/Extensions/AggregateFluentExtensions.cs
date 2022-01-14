using System.Linq;
using MongoDB.Driver;

namespace DevExtreme.AspNet.Data.MongoDB.Extensions
{
    public static class AggregateFluentExtensions
    {
        public static bool IsSorted<TResult>(this IAggregateFluent<TResult> aggregateFluent)
        {
            return aggregateFluent.Stages.Any(stage => stage.OperatorName == "$sort");
        }
    }
}