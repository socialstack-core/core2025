using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Startup;
using Api.CanvasRenderer;
using Api.Eventing;
using Api.SocketServerLibrary;
using Microsoft.AspNetCore.Http;

namespace Api.Pages
{
    /// <summary>
    /// Handles page endpoints.
    /// </summary>

    [Route("v1/page")]
    public partial class PageController : AutoController<Page>
    {
        private static HtmlService _htmlService;
        private static FrontendCodeService _frontendService;
        private static byte[] _oldVersion = System.Text.Encoding.UTF8.GetBytes("{\"oldVersion\":1}");
        private PageService _pageService;

        /// <summary>
        /// 
        /// </summary>
        public PageController(PageService ps)
        {
            _pageService = ps;
        }

		/// <summary>
		/// Attempts to get the page state of a page given the url and the version. Not available to the SSR or websocket APIs.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="context"></param>
		/// <param name="pageDetails"></param>
		/// <returns></returns>
		[HttpPost("state")]
        [Returns(typeof(PageStateResult))]
		public async ValueTask PageState(HttpContext httpContext, Context context, [FromBody] PageDetails pageDetails)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (_htmlService == null)
            {
                _htmlService = Services.Get<HtmlService>();
                _frontendService = Services.Get<FrontendCodeService>();
            }

            // Version check - are they out of date?
            if (pageDetails.version < _frontendService.Version)
            {
                await response.Body.WriteAsync(_oldVersion);
                return;
            }

			// If services have not finished starting up yet, wait.
			var svcWaiter = Services.StartupWaiter;

			if (svcWaiter != null)
			{
				await svcWaiter.Task;
			}

			// we first need to get the pageAndTokens
			var pageAndTokens = await _pageService.GetPage(context, request.Host.Value, pageDetails.Url, Microsoft.AspNetCore.Http.QueryString.Empty, true);

            if (pageAndTokens.RedirectTo != null)
            {
                // Redirecting to the given url, as a 302:
                var writer = Writer.GetPooled();
                writer.Start(null);

                writer.WriteASCII("{\"redirect\":");
                writer.WriteEscaped(pageAndTokens.RedirectTo);
                writer.Write((byte)'}');
                await writer.CopyToAsync(response.Body);
                writer.Release();
                return;
            }
        
            await Events.Page.BeforeNavigate.Dispatch(context, pageAndTokens.Page, pageDetails.Url);

            response.ContentType = "application/json";
			await _htmlService.RenderState(context, pageAndTokens, response, pageDetails.Url);
		}

        /// <summary>
        /// Used when getting the page state.
        /// </summary>
        public class PageDetails
        {
            /// <summary>
            /// The url of the page we are getting the state for.
            /// </summary>
            public string Url;

            /// <summary>
            /// The version
            /// </summary>
            public long version;
        }
    }
}