using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Startup;
using Api.CanvasRenderer;
using Api.Eventing;
using Api.SocketServerLibrary;
using Microsoft.AspNetCore.Http;
using Api.Startup.Routing;
using System;
using System.Collections.Generic;

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

            // Construct the pageWithTokens via first locating the terminal in the router.
            var terminalWithTokens = Router.CurrentRouter.ResolveWithTokens(context, pageDetails.Url);

            if (terminalWithTokens == null)
            {
                response.StatusCode = 404;
                return;
            }

            var redirectTerminal = terminalWithTokens.Value.TerminalNode as TerminalRedirectNode;

			if (redirectTerminal != null)
            {
                // Redirecting to the given url, as a 302:
                var writer = Writer.GetPooled();
                writer.Start(null);

                writer.WriteASCII("{\"redirect\":");
                writer.WriteEscaped(redirectTerminal.GetTarget());
                writer.Write((byte)'}');
                await writer.CopyToAsync(response.Body);
                writer.Release();
                return;
            }

			var pageTerminal = terminalWithTokens.Value.TerminalNode as RouterPageTerminal;

			if (pageTerminal == null)
			{
				response.StatusCode = 404;
				return;
			}

            // Build the pageAndTokens, collecting the primary content too:
            var pageWithTokens = new PageWithTokens() {
                PageTerminal = pageTerminal,
                Host = request.Host,
                TokenValues = terminalWithTokens.Value.Tokens
            };

            pageWithTokens.PrimaryService = pageTerminal.GetPrimaryService();
			pageWithTokens.PrimaryObject = await pageTerminal.GetPrimaryObject(context, pageWithTokens);

			await Events.Page.BeforeNavigate.Dispatch(context, pageWithTokens);

            response.ContentType = "application/json";
			await _htmlService.RenderState(context, response, pageWithTokens);
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