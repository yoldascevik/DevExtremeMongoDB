using System;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data.MongoDB.Builders;
using DevExtreme.AspNet.Data.MongoDB.Extensions;
using DevExtreme.AspNet.Data.ResponseModel;
using MongoDB.Driver;

namespace DevExtreme.AspNet.Data.MongoDB
{
    public class MongoDataSourceLoader : DataSourceLoader
    {
        public static LoadResult Load<TDocument>(IAggregateFluent<TDocument> source, DataSourceLoadOptionsBase options)
        {
            return Task.Run(() => LoadAsync(source, options)).Result;
        }

        public static async Task<LoadResult> LoadAsync<TDocument>(IAggregateFluent<TDocument> source, DataSourceLoadOptionsBase options)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            var loadResult = new LoadResult();
            IAggregateFluent<TDocument> result = source;

            // filter
            if (options.Filter?.Count > 0)
            {
                FilterDefinition<TDocument> filterDefinition = DxFilterDefinitionBuilder<TDocument>.Build(options.Filter);
                result = result.Match(filterDefinition);
            }

            // RequireTotalCount
            if (options.RequireTotalCount)
            {
                var countResult = await result.Count().FirstOrDefaultAsync();
                loadResult.totalCount = Convert.ToInt32(countResult?.Count ?? 0);
            }

            // sorting
            SortDefinition<TDocument> sortDefinition = DxSortDefinitionBuilder<TDocument>.Build(options, !result.IsSorted());
            if (sortDefinition != null)
            {
                result = result.Sort(sortDefinition);
            }

            // paging
            if (options.Skip > 0)
                result = result.Skip(options.Skip);

            if (options.Take > 0)
                result = result.Limit(options.Take);

            loadResult.data = await result.ToListAsync();

            return loadResult;
        }

        public static LoadResult Load<TDocument, TProjection>(IFindFluent<TDocument, TProjection> source, DataSourceLoadOptionsBase options)
        {
            return Task.Run(() => LoadAsync(source, options)).Result;
        }

        public static async Task<LoadResult> LoadAsync<TDocument, TProjection>(IFindFluent<TDocument, TProjection> source, DataSourceLoadOptionsBase options)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            var loadResult = new LoadResult();
            IFindFluent<TDocument, TProjection> result = source;
            
            // filter
            if (options.Filter?.Count > 0)
            {
                FilterDefinition<TDocument> filterDefinition = DxFilterDefinitionBuilder<TDocument>.Build(options.Filter);
                result.Filter = Builders<TDocument>.Filter.And(result.Filter, filterDefinition);
            }

            // RequireTotalCount
            if (options.RequireTotalCount)
            {
                var countResult = await result.CountDocumentsAsync();
                loadResult.totalCount = Convert.ToInt32(countResult);
            }

            // sorting
            SortDefinition<TDocument> sortDefinition = DxSortDefinitionBuilder<TDocument>.Build(options, !result.IsSorted());
            if (sortDefinition != null)
            {
                result = result.Sort(sortDefinition);
            }

            // paging
            if (options.Skip > 0)
                result = result.Skip(options.Skip);

            if (options.Take > 0)
                result = result.Limit(options.Take);

            loadResult.data = await result.ToListAsync();

            return loadResult;
        }
    }
}