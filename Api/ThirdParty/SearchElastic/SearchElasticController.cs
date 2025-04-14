using Api.Contexts;
using Api.Permissions;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.SearchElastic
{
    /// <summary>Handles site search endpoints.</summary>
    [Route("v1/sitesearch")]
    public partial class SearchElasticController : AutoController
    {
        /// <summary>
        /// Exposes the site search
        /// </summary>
        [HttpPost("query")]
        public virtual async ValueTask<DocumentsResult> Query(Context context, [FromBody] JObject filters)
        {
            var queryString = filters["queryString"] != null ? filters["queryString"].ToString() : "";
            var aggregations = filters["aggregations"] != null ? filters["aggregations"].ToString() : "";
            var tags = filters["tags"] != null ? filters["tags"].ToString() : "";
            var contentTypes = filters["contentTypes"] != null ? filters["contentTypes"].ToString() : "";

            var indexName = filters["indexName"] != null ? filters["indexName"].ToString() : "";


            var sortField = filters["sortField"] != null ? filters["sortField"].ToString() : "_score";
            var sortOrder = filters["sortOrder"] != null ? filters["sortOrder"].ToString() : "descending";
            if (sortOrder == "asc" || sortOrder == "desc")
            {
                sortOrder = sortOrder + "ending";
            }

            int pageSize = filters["pageSize"] != null ? filters["pageSize"].ToObject<int>() : 10;
            int pageIndex = filters["pageIndex"] != null ? filters["pageIndex"].ToObject<int>() : 1;

            bool allFields = filters["allFields"] != null ? filters["allFields"].ToObject<bool>() : false;

            var documentsResult = await Services.Get<SearchElasticService>().Query(context, indexName, queryString, tags, contentTypes, aggregations, sortField, sortOrder, pageIndex, pageSize, allFields);

            return documentsResult;
        }

        /// <summary>
        /// Exposes the taxonomy values based on categories
        /// </summary>
        [HttpPost("taxonomy")]
        public virtual async ValueTask<AggregationStructure> Taxonomy(Context context, [FromBody] JObject filters)
        {
            var fields = filters["fields"] != null ? filters["fields"].ToString() : "";

            if (string.IsNullOrWhiteSpace(fields))
            {
                return null;
            }

            var taxonomyResult = await Services.Get<SearchElasticService>().Taxonomy(context, fields);

            return taxonomyResult;
        }

        /// <summary>
        /// Reset the indexer
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        public virtual async ValueTask<bool> Reset(Context context)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("elastic_reset", context);
            }

            return await Services.Get<SearchElasticService>().Reset(context);
        }

        /// <summary>
        /// Reset a single index (delete all documents)
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("reset/index/{indexName}")]
        public virtual async ValueTask<bool> ResetIndex(Context context, [FromRoute] string indexName)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("elastic_purge_index", context);
            }

            return await Services.Get<SearchElasticService>().Reset(context,indexName);
        }

        /// <summary>
        /// Reset a single index (delete all documents)
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("delete/index/{indexName}")]
        public virtual async ValueTask<bool> DeleteIndex(Context context, [FromRoute] string indexName)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("elastic_delete_index", context);
            }

            return await Services.Get<SearchElasticService>().DeleteIndex(context, indexName);
        }


        /// <summary>
        /// Exposes the current status of the elastic store
        /// </summary>
        /// <returns></returns>
        [HttpGet("health")]
        public virtual async ValueTask<Nest.ClusterHealthResponse> Health(Context context)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("elastic_health", context);
            }

            return await Services.Get<SearchElasticService>().Health(context);
        }

        /// <summary>
        /// Exposes details of the current shards in the elastic store
        /// </summary>
        /// <returns></returns>
        [HttpGet("shards")]
        public virtual async ValueTask<List<Nest.CatShardsRecord>> Shards(Context context)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("elastic_shards", context);
            }

            return await Services.Get<SearchElasticService>().Shards(context);
        }

    }
}