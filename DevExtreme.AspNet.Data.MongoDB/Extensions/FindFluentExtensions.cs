using MongoDB.Driver;

namespace DevExtreme.AspNet.Data.MongoDB.Extensions
{
    public static class FindFluentExtensions
    {
        public static bool IsSorted<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> findFluent)
        {
            return findFluent.Options.Sort != null;
        }
    }
}