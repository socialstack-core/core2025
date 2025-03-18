using Api.Database;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Eventing;
using Api.Automations;
using System;
using Api.Pages;
using Api.Translate;
using Api.CanvasRenderer;
using System.Reflection;
using Api.Startup;
using Microsoft.ClearScript;

namespace Api.SearchCrawler
{
    /// <summary>
    /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
    /// </summary>

    [LoadPriority(9)]
    public partial class SearchCrawlerService : AutoService
    {
        private PageService _pageService;
        private LocaleService _locales;
        private CanvasRendererService _canvasRenderer;
        private HtmlService _htmlService;
        private SearchCrawlerConfig _cfg;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public SearchCrawlerService(PageService pageService, LocaleService locales, CanvasRendererService canvasRenderer, HtmlService htmlService)
        {
            _pageService = pageService;
            _locales = locales;
            _canvasRenderer = canvasRenderer;
            _htmlService = htmlService;
            _cfg = GetConfig<SearchCrawlerConfig>();

            Events.Service.AfterStart.AddEventListener((Context context, object sender) =>
            {
                // The cron expression runs it every hour. It does nothing if nothing is using the crawlers output.
                Events.Automation("search_crawler", "0 0 */2 ? * * *",false,"Process all the site pages for search").AddEventListener(async (Context context, AutomationRunInfo runInfo) =>
                {
                    if (!IsConfigured())
                    {
                        return runInfo;
                    }

                    // define the mode can pass single or combine modes
                    // e.g. SearchCrawlerMode.Indexing | SearchCrawlerMode.Sitemap
                    await CrawlEverything(SearchCrawlerMode.Indexing);

                    return runInfo;
                });

                // The cron expression runs it every hour. It does nothing if nothing is using the crawlers output.
                Events.Automation("sitemap_crawler", "0 30 6 ? * * *", false, "Process all the site pages for sitemap").AddEventListener(async (Context context, AutomationRunInfo runInfo) =>
                {
                    if (!IsConfigured())
                    {
                        return runInfo;
                    }

                    // define the mode can pass single or combine modes
                    // e.g. SearchCrawlerMode.Indexing | SearchCrawlerMode.Sitemap
                    await CrawlEverything(SearchCrawlerMode.Sitemap);

                    return runInfo;
                });

                return new ValueTask<object>(sender);
            });
        }

        /// <summary>
        /// Tells the crawler to crawl every page in the sitemap now. Does nothing if there is nothing to receive the output of the crawler.
        /// This means you turn the crawler on by simply adding an event handler to it.
        /// As it runs, it invokes both the Events.Crawler.PageCrawled event and also the given handler.
        /// </summary>
        /// <returns></returns>
        public async ValueTask CrawlEverything(SearchCrawlerMode mode)
        {
            if (!Events.Crawler.PageCrawledNoPrimaryContent.HasListeners())
            {
                // Nothing wants the crawler output so we don't need to run at all.
                return;
            }

            // Anonymous context - Let's not accidentally leak things through the search index.
            var context = new Context();

            // Get all the current locales:
            var locales = await _locales.Where("").ListAll(context);

            await Events.Crawler.CrawlerStatus.Dispatch(context, SearchCrawlerStatus.Started, mode);

            // For each locale..
            foreach (var locale in locales)
            {
                context.LocaleId = locale.Id;

                // Get the raw page tree for the locale (does not iterate every token page internally):
                var sitemap = await _pageService.GetPageTree(context);

                // Iterate through it, expanding any dynamic token-like pages as we go. Non-expandable pages are ignored however.
                await CrawlTreeNode(context, mode, sitemap.Root);
            }

            await Events.Crawler.CrawlerStatus.Dispatch(context, SearchCrawlerStatus.Completed, mode);
        }

        /// <summary>
        /// Crawls a single page. This expects its URL to be static with no tokens in it.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mode"></param>
        /// <param name="page"></param>
        /// <param name="onCrawledPage"></param>
        /// <returns></returns>
        public async ValueTask Crawl(Context context, SearchCrawlerMode mode, Page page, Func<Context, CrawledPageMeta, ValueTask> onCrawledPage = null)
        {
            try
            {
                byte[] anonPg = Array.Empty<byte>();

                if (_cfg.IncludePageContent)
                {
                    // Get potentially cached page (or caches it now):
                    anonPg = await _htmlService.GetCachedAnonymousPage(context, page.Url);
                }

                var cpm = new CrawledPageMeta()
                {
                    Title = page.Title,
                    Url = ParseUrl(page.Url),
                    Page = page,
                    BodyCompressedBytes = anonPg
                };

                if (onCrawledPage != null)
                {
                    await onCrawledPage(context, cpm);
                }

                await Events.Crawler.PageCrawledNoPrimaryContent.Dispatch(context, cpm, mode);
            }
            catch (ScriptEngineException se)
            {
                Log.Error(LogTag, se, "Crawling '" + page.Url + " Failed");

            }
            catch (Exception e)
            {
                Log.Error(LogTag, e, "Crawling '" + page.Url + " Failed");
            }
        }

        /// <summary>
        /// Crawls a specific page. The optional lookupNode provides metadata about the parsed URL.
        /// If tokenData is provided and the parsed URL contains 1 type of token then crawling the 
        /// page will result in iterating through all permutations of the tokens.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mode"></param>
        /// <param name="page"></param>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async ValueTask CrawlPermutations(Context context, SearchCrawlerMode mode, Page page, UrlLookupTerminal tokenData)
        {
            // Skip admin pages and any flagged as excluded
            if (page == null || page.ExcludeFromSearch || page.Url.StartsWith("/en-admin") || page.Url.StartsWith("en-admin"))
            {
                return;
            }

            if (tokenData.UrlTokens != null && tokenData.UrlTokens.Count != 0)
            {
                AutoService service = null;

                foreach (var token in tokenData.UrlTokens)
                {
                    // This crawler only supports pages with 1 data type in the url tokens
                    if (service != null && service != token.Service)
                    {
                        return;
                    }

                    service = token.Service;
                }

                if (service == null || service.ServicedType == null)
                {
                    // Not a PO token. We have no information for how to crawl this
                    // page as the token(s) in the URL are too generic.
                    return;
                }

                if (_crawlAService == null)
                {
                    _crawlAService = GetType().GetMethod(nameof(CrawlService));
                }

                var crawlAServiceGenericMethod = _crawlAService.MakeGenericMethod(new Type[] {
                    service.ServicedType,
                    service.IdType
                });

                await (ValueTask)(crawlAServiceGenericMethod.Invoke(this, new object[] {
                    context,
                    mode,
                    service,
                    tokenData,
                    page
                }));
            }
            else
            {
                // Singular static page:
                await Crawl(context, mode, page);
            }
        }

        private MethodInfo _crawlAService;

        /// <summary>
        /// Crawls a particular service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ID"></typeparam>
        /// <param name="context"></param>
        /// <param name="mode"></param>
        /// <param name="service"></param>
        /// <param name="tokenData"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async ValueTask CrawlService<T, ID>(Context context, SearchCrawlerMode mode, AutoService<T, ID> service, UrlLookupTerminal tokenData, Page page)
                    where T : Content<ID>, new()
                    where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            // As with the generic page crawl, do nothing if the service PageCrawl event is not subscribed to.
            if (!service.EventGroup.PageCrawled.HasListeners())
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Warn(service.EntityName.ToLower(), $"[{service.EntityName}Service] not registered for search/crawl - check LoadPriority");
                }

                return;
            }

            // Create a URL generator for this page:
            var urlGenerator = new UrlGenerator(page.Url);

            // For each object in the service, establish its URL and crawl.
            await service.Where(DataOptions.IgnorePermissions).ListAll(context, async (Context c, T po, int index, object a, object b) =>
            {
                if (po == null)
                {
                    return;
                }

                var svc = (AutoService<T, ID>)a;

                // resolve the apparent URL:
                var resolvedUrl = urlGenerator.Generate(po);

                if (string.IsNullOrWhiteSpace(resolvedUrl))
                {
                    return;
                }

                resolvedUrl = resolvedUrl.Trim();

                // resolve any meta data in the page title
                var resolvedTitle = await _htmlService.ReplaceTokens(context, page.Title, po);

                try
                {
                    byte[] anonPg = Array.Empty<byte>();

                    if (_cfg.IncludePageContent)
                    {
                        // Get potentially cached page (or caches it now):
                        anonPg = await _htmlService.GetCachedAnonymousPage(c, resolvedUrl);
                    }

                    var cpm = new CrawledPageMeta()
                    {
                        Url = ParseUrl(resolvedUrl),
                        Title = resolvedTitle,
                        Page = page,
                        BodyCompressedBytes = anonPg
                    };

                    // Tell other things about this page being crawled:
                    await svc.EventGroup.PageCrawled.Dispatch(c, cpm, po, mode);
                }
                catch (ScriptEngineException se)
                {
                    Log.Error(LogTag, se, "Crawling '" + page.Url + "-" + resolvedUrl + " failed.");
                }
                catch (Exception e)
                {
                    Log.Error(LogTag, e, "Crawling '" + page.Url + "-" + resolvedUrl + " failed.");
                }

            }, service, null);

        }

        private string ParseUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            url = url.Trim();

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                return url;
            }

            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }

        private async ValueTask CrawlTreeNode(Context context, SearchCrawlerMode mode, UrlLookupNode urlTreeNode)
        {
            if (urlTreeNode == null)
            {
                return;
            }

            // Crawl pages at this node (if there are any - pass through and redirect nodes exist too).
            var pages = urlTreeNode.Terminals;

            if (pages != null)
            {
                foreach (var terminal in pages)
                {
                    if (terminal.Page == null)
                    {
                        continue;
                    }

                    await CrawlPermutations(context, mode, terminal.Page, terminal);
                }
            }

            // Crawl the child nodes too:
            var childNodes = urlTreeNode.Children;

            if (childNodes != null)
            {
                foreach (var child in childNodes)
                {
                    await CrawlTreeNode(context, mode, child.Value);
                }
            }
        }

        private bool IsConfigured()
        {
            return !_cfg.Disabled;
        }


    }

}
