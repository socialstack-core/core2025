using Api.Contexts;
using Api.Permissions;
using Api.SearchElastic;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Api.SearchEntities
{
    /// <summary>Handles searchEntity endpoints.</summary>
    [Route("v1/entitysearch")]
	public partial class SearchEntityController : AutoController<SearchEntity>
    {

        /// <summary>
        /// Exposes the site search
        /// </summary>
        [HttpPost("query")]
        public virtual async ValueTask<DocumentsResult> Query([FromBody] JObject filters)
        {
            var context = await Request.GetContext();

            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("entity_search", context);
            }

            var _cfg = _service.GetConfig<SearchEntityConfig>();

            if (_cfg != null && !string.IsNullOrEmpty(_cfg.IndexName))
            {
                var queryString = filters["queryString"] != null ? filters["queryString"].ToString() : "";
                var aggregations = filters["aggregations"] != null ? filters["aggregations"].ToString() : "";
                var tags = filters["tags"] != null ? filters["tags"].ToString() : "";
                var contentTypes = filters["contentTypes"] != null ? filters["contentTypes"].ToString() : "";

                var sortField = filters["sortField"] != null ? filters["sortField"].ToString() : "editedUtc";
                var sortOrder = filters["sortOrder"] != null ? filters["sortOrder"].ToString() : "descending";
                if (sortOrder == "asc" || sortOrder == "desc")
                {
                    sortOrder = sortOrder + "ending";
                }

                int pageSize = filters["pageSize"] != null ? filters["pageSize"].ToObject<int>() : 10;
                int pageIndex = filters["pageIndex"] != null ? filters["pageIndex"].ToObject<int>() : 1;

                bool allFields = filters["allFields"] != null ? filters["allFields"].ToObject<bool>() : false;

                var documentsResult = await Services.Get<SearchElasticService>().Query(context, _cfg.IndexName, queryString, tags, contentTypes, aggregations, sortField, sortOrder, pageIndex, pageSize, allFields);
                
                return documentsResult;
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Reset the index
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        public virtual async ValueTask<bool> Reset()
        {
            var context = await Request.GetContext();

            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("entity_search_reset", context);
            }

            var _cfg = _service.GetConfig<SearchEntityConfig>();

            if (_cfg != null && !string.IsNullOrEmpty(_cfg.IndexName))
            {
                return await Services.Get<SearchElasticService>().Reset(context, _cfg.IndexName);
            }

            return false;
        }

        /// <summary>
        /// Reindex the content
        /// </summary>
        /// <returns></returns>
        [HttpGet("reindex")]
        public virtual async ValueTask<bool> Index()
        {
            var context = await Request.GetContext();

            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw PermissionException.Create("entity_search_index", context);
            }

            var _cfg = _service.GetConfig<SearchEntityConfig>();

            if (_cfg != null && !string.IsNullOrEmpty(_cfg.IndexName))
            {
                await Services.Get<SearchEntityService>().Index(context);
                return true;
            }

            return false;
        }


    }
}