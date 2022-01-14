using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using DevExtreme.AspNet.Data.MongoDB.Constants;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevExtreme.AspNet.Data.MongoDB.Builders
{
    internal static class DxFilterDefinitionBuilder<T>
    {
        private static readonly FilterDefinitionBuilder<T> Builder = Builders<T>.Filter;

        public static FilterDefinition<T> Build(IList filter)
        {
            var filterDefinition = FilterDefinition<T>.Empty;

            if (filter?.Count > 0)
            {
                var filterString = JsonConvert.SerializeObject(filter);
                var filterTree = JArray.Parse(filterString);

                filterDefinition = ReadExpression(filterTree);
            }

            return filterDefinition;
        }

        private static FilterDefinition<T> ReadExpression(JArray array)
        {
            if (!array.Any())
                return FilterDefinition<T>.Empty;

            var filterDefinition = FilterDefinition<T>.Empty;
            string currentLogicalOperator = string.Empty;

            if (array[0].Type == JTokenType.String)
                return BuildFromExpression(
                    array[0].ToString(),
                    array[1].ToString(),
                    array[2]);
            else
            {
                foreach (var item in array)
                {
                    if (item.Type == JTokenType.String) // logical, and, or vs
                    {
                        currentLogicalOperator = item.ToString();
                    }
                    else
                    {
                        FilterDefinition<T> exprFilter = ReadExpression((JArray) item);

                        filterDefinition = !string.IsNullOrEmpty(currentLogicalOperator)
                            ? BuildFromLogicalOperator(currentLogicalOperator, filterDefinition, exprFilter)
                            : exprFilter;

                        currentLogicalOperator = string.Empty;
                    }
                }
            }

            return filterDefinition;
        }

        private static FilterDefinition<T> BuildFromExpression(string columnName, string comparisonOperator, JToken value)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));
            
            if (string.IsNullOrEmpty(comparisonOperator))
                throw new ArgumentNullException(nameof(comparisonOperator));
            
            FilterDefinition<T> result;
            string valueString = value?.ToString();

            switch (comparisonOperator)
            {
                case ComparisonOperators.Equality:
                    result = Builder.Eq(columnName, valueString);
                    break;
                case ComparisonOperators.DoesNotEqual:
                    result = Builder.Ne(columnName, valueString);
                    break;
                case ComparisonOperators.LessThan:
                    result = Builder.Lt(columnName, valueString);
                    break;
                case ComparisonOperators.GreaterThan:
                    result = Builder.Gt(columnName, valueString);
                    break;
                case ComparisonOperators.LessThanOrEqual:
                    result = Builder.Lte(columnName, valueString);
                    break;
                case ComparisonOperators.GreaterThanOrEqual:
                    result = Builder.Gte(columnName, valueString);
                    break;
                case ComparisonOperators.StartsWith:
                    result = Builder.Regex(columnName, new Regex($"^{valueString}.*", RegexOptions.IgnoreCase));
                    break;
                case ComparisonOperators.EndsWith:
                    result = Builder.Regex(columnName, new Regex($"{valueString}$", RegexOptions.IgnoreCase));
                    break;
                case ComparisonOperators.Contains:
                    result = Builder.Regex(columnName, new Regex($".*{valueString}.*", RegexOptions.IgnoreCase));
                    break;
                case ComparisonOperators.NotContains:
                    result = Builder.Not(Builder.Regex(columnName, new Regex($".*{valueString}.*", RegexOptions.IgnoreCase)));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private static FilterDefinition<T> BuildFromLogicalOperator(string logicalOperator, params FilterDefinition<T>[] filterDefinitions)
        {
            if (string.IsNullOrEmpty(logicalOperator))
                throw new ArgumentNullException(nameof(logicalOperator));

            if (filterDefinitions.Count() == 1)
                return filterDefinitions[0];

            if (logicalOperator == LogicalOperators.And)
                return Builder.And(filterDefinitions);

            if (logicalOperator == LogicalOperators.Or)
                return Builder.Or(filterDefinitions);

            throw new NotImplementedException();
        }
    }
}