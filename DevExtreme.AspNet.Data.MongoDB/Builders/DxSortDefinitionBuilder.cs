using System;
using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data.MongoDB.Helpers;
using MongoDB.Driver;

namespace DevExtreme.AspNet.Data.MongoDB.Builders
{
    internal static class DxSortDefinitionBuilder<T>
    {
        private static readonly SortDefinitionBuilder<T> Builder = new SortDefinitionBuilder<T>();

        public static SortDefinition<T> Build(DataSourceLoadOptionsBase loadOptions, bool ensurePrimaryKeySorting)
        {
            if (loadOptions is null)
                throw new ArgumentNullException(nameof(loadOptions));

            // apply if sort
            if (loadOptions.Sort != null && loadOptions.Sort.Any())
                return CombineSortingInfos(loadOptions.Sort);

            // if there is paging and no sorting is done, try to sort with the primary key
            if (ensurePrimaryKeySorting && (loadOptions.Skip > 0 || loadOptions.Take > 0))
                return SortByPrimaryKey(loadOptions);

            return null;
        }

        private static SortDefinition<T> SortByPrimaryKey(DataSourceLoadOptionsBase loadOptions)
        {
            string[] primaryKeys = loadOptions.PrimaryKey ?? ReflectionUtil.GetPrimaryKeysByType<T>();

            if (primaryKeys != null && primaryKeys.Any())
            {
                loadOptions.SortByPrimaryKey = true;
                var primaryKeySortingInfoList = new List<SortingInfo>();

                foreach (string primaryKey in primaryKeys)
                {
                    primaryKeySortingInfoList.Add(new SortingInfo()
                    {
                        Selector = primaryKey,
                        Desc = false
                    });
                }
                    
                return CombineSortingInfos(primaryKeySortingInfoList);
            }
            
            return null;
        }

        private static SortDefinition<T> CombineSortingInfos(IEnumerable<SortingInfo> sortingInfos)
        {
            if (sortingInfos == null || !sortingInfos.Any())
                return null;
            
            var sortDefinitionList = new List<SortDefinition<T>>();
            	
            foreach (var sortingInfo in sortingInfos)
            {
                SortDefinition<T> sortDefinition = sortingInfo.Desc 
                    ? Builder.Descending(sortingInfo.Selector)
                    : Builder.Ascending(sortingInfo.Selector);
            			
                sortDefinitionList.Add(sortDefinition);
            }
            	
            return Builder.Combine(sortDefinitionList);
        }
    }
}