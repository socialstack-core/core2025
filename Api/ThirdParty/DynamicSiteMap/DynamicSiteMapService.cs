using Api.CanvasRenderer;
using Api.Database;
using Api.Eventing;
using Api.Pages;
using Api.Permissions;
using Api.SearchCrawler;
using Api.SiteDomains;
using Api.Startup;
using Api.Translate;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Context = Api.Contexts.Context;

namespace Api.DynamicSiteMap
{
    /// <summary>
    /// Service to extract dynamic sitemaps based on site search crawler data 
    /// </summary>

    [LoadPriority(9)]

    public partial class DynamicSiteMapService : AutoService
    {
        private DynamicSiteMapServiceConfig _cfg;
        private readonly PageService _pageService;
        private readonly FrontendCodeService _frontend;
        private readonly LocaleService _locales;
        private readonly SiteDomainService _siteDomainService;
        private Capability _pageLoadCapability;

        private HashSet<string> _processedPages = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private ConcurrentDictionary<string, StringBuilder> _currentSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();
        private ConcurrentDictionary<string, StringBuilder> _masterSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();
        private ConcurrentDictionary<string, int> _siteMapCount = new ConcurrentDictionary<string, int>();

        private List<Locale> _allLocales;

        private int _pageCount = 0;

        /// <summary>
        /// True if the service is active.
        /// </summary>
        private bool _isConfigured;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public DynamicSiteMapService(PageService pageService, FrontendCodeService frontend, LocaleService localeService, SiteDomainService siteDomainService)
        {
            _pageService = pageService;
            _frontend = frontend;
            _locales = localeService;
            _siteDomainService = siteDomainService;

            _pageLoadCapability = Events.Page.GetLoadCapability();
			_cfg = GetConfig<DynamicSiteMapServiceConfig>();
            UpdateIsConfigured();

            _cfg.OnChange += () => {
				UpdateIsConfigured();
				return new ValueTask();
            };

            var setupForTypeMethod = GetType().GetMethod(nameof(SetupForType));

            Events.Service.AfterStart.AddEventListener((Context context, object sender) =>
            {
                // subscribe to site crawler which will extract pages for all locales
                Events.Crawler.PageCrawledNoPrimaryContent.AddEventListener((Context ctx, CrawledPageMeta pageMeta, SearchCrawlerMode mode) =>
                {
                    if (IsConfigured() && (mode & SearchCrawlerMode.Sitemap) == SearchCrawlerMode.Sitemap)
                    {
                        ProcessPage<object>(ctx, pageMeta, "", null);
                    }

                    return new ValueTask<CrawledPageMeta>(pageMeta);
                });

                // subscribe to site crawler status change event
                Events.Crawler.CrawlerStatus.AddEventListener((Context ctx, SearchCrawlerStatus status, SearchCrawlerMode mode) =>
                {
                    if (!IsConfigured() || (mode & SearchCrawlerMode.Sitemap) != SearchCrawlerMode.Sitemap)
                    {
                        return new ValueTask<SearchCrawlerStatus>(status);
                    }

                    if (status == SearchCrawlerStatus.Started)
                    {
                        Log.Info(LogTag, "Dynamic Site Map indexer starting.");
                        _masterSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();
                        _currentSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();

                        _siteMapCount = new ConcurrentDictionary<string, int>();
                        _processedPages = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                        _pageCount = 0;
                    }

                    if (status == SearchCrawlerStatus.Completed)
                    {
                        try
                        {
                            // all done so close any files etc 
                            WriteMasterSiteMapFiles(ctx.LocaleId);
                            Log.Ok(LogTag, "Dynamic Site Map indexing completed.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(LogTag, ex, "Dynamic Site Map failed to save SiteMap File");
                        }
                    }

                    return new ValueTask<SearchCrawlerStatus>(status);
                });

                return new ValueTask<object>(sender);
            });

            // subscribe to events triggered by content types so we can add index listeners 
            Events.Service.AfterCreate.AddEventListener((Context ctx, AutoService service) =>
            {
                if (service == null)
                {
                    return new ValueTask<AutoService>(service);
                }

                // Get the content type for this service and event group:
                var servicedType = service.ServicedType;
                if (servicedType == null)
                {
                    // Things like the ffmpeg service.
                    return new ValueTask<AutoService>(service);
                }

                // If it's a mapping type, ignore
                if (ContentTypes.IsAssignableToGenericType(servicedType, typeof(Mapping<,>)))
                {
                    return new ValueTask<AutoService>(service);
                }

                // Add List event:
                var setupType = setupForTypeMethod.MakeGenericMethod(new Type[] {
                    servicedType,
                    service.IdType
                });

                setupType.Invoke(this, new object[] {
                    service
                });

                return new ValueTask<AutoService>(service);
            });
        }

        /// <summary>
        /// Gets the service config (readonly).
        /// </summary>
        /// <returns></returns>
        public DynamicSiteMapServiceConfig GetConfiguration()
        {
            return _cfg;
        }

		/// <summary>
		/// Handler for content types to expose content related data
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="service"></param>
		public void SetupForType<T, ID>(AutoService<T, ID> service)
            where T : Content<ID>, new()
            where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            if (_cfg.DebugToConsole)
            {
                Log.Info(LogTag, $"Attaching dynamic site map to '{service.EntityName}'");
            }

            // Content types that can be used by the page system all appear here.    
            // Let's hook up to the PageCrawled event which tells us when a page of this primary object type (and from this service) has been crawled:
            service.EventGroup.PageCrawled.AddEventListener((Context ctx, CrawledPageMeta pageMeta, T po, SearchCrawlerMode mode) =>
            {
                if (!IsConfigured() || (mode & SearchCrawlerMode.Sitemap) != SearchCrawlerMode.Sitemap)
                {
                    return new ValueTask<CrawledPageMeta>(pageMeta);
                }

                try
                {
                    var mapdata = ProcessPage(ctx, pageMeta, po.Type, po);
                }
                catch (Exception ex)
                {
                    Log.Error(LogTag, ex, "Dynamic Site Map failed to process crawled page");
                }

				return new ValueTask<CrawledPageMeta>(pageMeta);
			});
        }

        /// <summary>
        /// Extract and create sitermpa entry from the current page
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pageDocument"></param>
        /// <param name="contentType"></param>
        /// <param name="po"></param>
        private bool ProcessPage<T>(Context ctx, CrawledPageMeta pageDocument, string contentType, T po)
        {
            if (pageDocument.Url.Length > 1 && pageDocument.Url.Count(x => (x == '/')) == pageDocument.Url.Length)
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Warn(LogTag, $"Dynamic Site Map - Invalid Url - {ctx.LocaleId} {pageDocument.Url} {pageDocument.Title} ");
                }
                return false;
            }

            if (!Roles.Public.IsGranted(_pageLoadCapability, ctx, pageDocument.Page, false).Result)
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Info("", "Not public - Ignoring - " + pageDocument.Url);
                }
                return false;
            }

            var url = pageDocument.Url;

            // first check if the page has an excluded prefix
            // pages may also get ignored in the crawler if ExcludeFromSearch is set
            if (_cfg.ExcludedPaths != null
                && _cfg.ExcludedPaths.Count > 0
                && _cfg.ExcludedPaths.Any(exp => $"{url}/".StartsWith(exp, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            // only want unique entries for simple sitemaps
            if (!_cfg.UseSiteDomains && !_cfg.UseLocaleDomains && _processedPages.Contains(url))
            {
                return false;
            }

			// check: is this a locale-specific URL?
			var locales = GetAllLocales(ctx);
			var urlLocale = GetUrlLocale(locales, pageDocument.Url);

			// skip if this locale has been marked as permanently redirected
			if (urlLocale != null && urlLocale.isRedirected && urlLocale.PermanentRedirect)
			{
				return false;
			}

			// assume basic single sitemap for site
			var siteMapKey = "*";

            // page could global, locale specific or domain specific 
            // depending on the site setup

            if (_cfg.UseLocaleDomains)
            {
                siteMapKey = ctx.LocaleId.ToString();

                // if this is a locale-specific URL (e.g. domain.com/uk/blog)
                if (urlLocale != null && ctx.LocaleId != urlLocale.Id)
                {
                    return false;
                }

                if (urlLocale != null)
                {
                    // strip out the locale path
                    url = url.Length > urlLocale.PagePath.Length + 1 ? url.Substring(urlLocale.PagePath.Length + 1) : "";
                }
            }
            else if (_cfg.UseSiteDomains)
            {
                // if using site domains all pages will be linked to a domain 
                // normally "core" is the main/default site domain code
                siteMapKey = "core";

                // are we processing a sub domain 
                var urlDomain = _siteDomainService.GetByUrl(pageDocument.Url + "/");
                if (urlDomain != null && !string.IsNullOrWhiteSpace(urlDomain.Code))
                {
                    // get the domain related code, all domains should have a value
                    siteMapKey = urlDomain.Code;
                }

                // get the domain settings
                urlDomain = _siteDomainService.GetByCode(siteMapKey);

                // do we auto generate a sitemap for this domain 
                if (urlDomain != null && urlDomain.ExcludeFromSiteMap)
                {
                    return false;
                }
            }

            // get a full resolved url
            url = GetUrl(url, ctx.LocaleId);

            if (_processedPages.Contains(url))
            {
                return false;
            }

            _processedPages.Add(url);

            var sb = new StringBuilder();

            sb.AppendLine("<url>");
            sb.AppendLine($"<loc>{url}</loc>");

            if (!_cfg.UseSiteDomains && !_cfg.UseLocaleDomains && _cfg.IncludeHrefLangs)
            {
                //for a single domain site expose hreflang entries based on the locales 
                if (locales != null && locales.Count(l => l.Id != 1) > 0)
                {
                    sb.AppendLine($"<xhtml:link rel=\"alternate\" hreflang=\"x-default\" href=\"{url}\" />");

                    foreach (var locale in locales.Where(l => l.Id != 1))
                    {
                        sb.AppendLine($"<xhtml:link rel=\"alternate\" hreflang=\"{locale.Code.ToLower()}\" href=\"{GetLocaleUrl(locale, url)}\" />");
                    }
                }
            }

            if (_cfg.IncludeImages && pageDocument.BodyCompressedBytes != null && pageDocument.BodyCompressedBytes.Length > 0)
            {
                var page = new HtmlDocument();
                page.Load(pageDocument.GetBodyStream());

                var images = page.DocumentNode.SelectNodes("//img");
                if (images != null)
                {
                    foreach (HtmlNode node in images)
                    {
                        var title = string.Empty;
                        var src = node.GetAttributeValue("src", null);

                        if (_cfg.ImageTitleAttributes != null && _cfg.ImageTitleAttributes.Any())
                        {
                            foreach (var attributeName in _cfg.ImageTitleAttributes)
                            {
                                if (node.Attributes[attributeName] != null && !string.IsNullOrWhiteSpace(node.Attributes[attributeName].Value))
                                {
                                    title = node.GetAttributeValue(attributeName, null);
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(src) && !string.IsNullOrWhiteSpace(title))
                        {
                            // (typically icons stored under /pack/static/...)
                            if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                            {
                                // always allow any static images with the sitemap class 
                                if (!_cfg.IncludeStaticImages && !HasClass(node,"sitemap"))
                                {
                                    continue;
                                }

                                // get the host of the current page
                                var parsedUrl = new Uri(url);
                                src = $"{parsedUrl.Scheme}://{parsedUrl.Host}{src}";
                            }

                            sb.AppendLine("<image:image>");
                            sb.AppendLine($"<image:loc>{src}</image:loc>");
                            sb.AppendLine($"<image:title>{title}</image:title>");
                            sb.AppendLine("</image:image>");
                        }
                    }
                }
            }

            sb.AppendLine("</url>");

            if (!_currentSiteMapContent.ContainsKey(siteMapKey))
            {
                _currentSiteMapContent.TryAdd(siteMapKey, new StringBuilder());
            }

            _currentSiteMapContent[siteMapKey].Append(sb.ToString());

            _pageCount++;

            if (_pageCount == _cfg.MaxSiteMapPages)
            {
                WriteSiteMapFile(siteMapKey, ctx.LocaleId);
            }

            return true;
        }

        private string GetUrl(string url, uint localeId)
        {
            url = url.ToLower().Trim();
            if (url.Length > 1 && url.EndsWith("/"))
            {
                url = url.TrimEnd(new[] { '/' }).Trim();
            }

            if (url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                return url;
            }

            if (_cfg.UseSiteDomains)
            {
                // add trailing '/' to cater for root pages such as /{domain}
                var urlDomain = _siteDomainService.GetByUrl(url + "/");
                if (urlDomain != null)
                {
                    // swap the domain prefix for the url
                    return UrlCombine($"https://{urlDomain.Domain}", url.Substring(urlDomain.Code.Length + 1));
                }

                urlDomain = _siteDomainService.GetByCode("core");
                if (urlDomain != null)
                {
                    // swap the domain prefix for the url
                    return UrlCombine($"https://{urlDomain.Domain}", url);
                }
            }

            return UrlCombine(_frontend.GetPublicUrl(localeId), url);
        }

        private void WriteSiteMapFile(string prefix, uint localeId)
        {
            if (!_currentSiteMapContent.ContainsKey(prefix) || _currentSiteMapContent[prefix].Length == 0)
            {
                return;
            }

            if (!_siteMapCount.ContainsKey(prefix))
            {
                _siteMapCount.TryAdd(prefix, 1);
            }
            else
            {
                _siteMapCount[prefix]++;
            }

            var publicPathName = "UI/public/";

            // if just a single set if sitemap files ignore any prefixes 
            var mapFilePrefix = prefix == "*" ? $"-{_siteMapCount[prefix]}" : $"-{prefix}-{_siteMapCount[prefix]}";

            // write to UI/public so that they are serviced directly
            using (StreamWriter file = new StreamWriter(publicPathName + $"sitemap{mapFilePrefix}.xml", false))
            {
                WrapSiteMapFile(_currentSiteMapContent[prefix]);
                file.WriteLine(_currentSiteMapContent[prefix].ToString());
            }

            _currentSiteMapContent[prefix] = new StringBuilder();

            if (!_masterSiteMapContent.ContainsKey(prefix))
            {
                _masterSiteMapContent.TryAdd(prefix, new StringBuilder());
            }

            // update master with link back to sub file
            _masterSiteMapContent[prefix].AppendLine("<url>");

            if (_cfg.UseSiteDomains)
            {
                var siteDomain = _siteDomainService.GetByCode(prefix);
                _masterSiteMapContent[prefix].AppendLine($"<loc>https://{siteDomain.Domain}/sitemap{mapFilePrefix}.xml</loc>");
            }
            else if (_cfg.UseLocaleDomains)
            {
                uint id = uint.Parse(prefix);
                _masterSiteMapContent[prefix].AppendLine($"<loc>{GetUrl($"sitemap{mapFilePrefix}.xml", id)}</loc>");
            }
            else
            {
                _masterSiteMapContent[prefix].AppendLine($"<loc>{GetUrl($"sitemap{mapFilePrefix}.xml", localeId)}</loc>");
            }

            _masterSiteMapContent[prefix].AppendLine("</url>");

            _pageCount = 0;
        }

        private void WriteMasterSiteMapFiles(uint localeId)
        {
            var publicPathName = "UI/public/";


            foreach (var prefix in _currentSiteMapContent.Keys)
            {
                // if just a single set if sitemap files ignore any prefixes 
                var mapFilePrefix = prefix == "*" ? "" : $"-{prefix}";

                // write to UI/public so that they are serviced directly
                using (StreamWriter file = new StreamWriter(publicPathName + $"sitemap{mapFilePrefix}.xml", false))
                {
                    if (!_masterSiteMapContent.ContainsKey(prefix))
                    {
                        WrapSiteMapFile(_currentSiteMapContent[prefix]);
                        file.WriteLine(_currentSiteMapContent[prefix].ToString());
                    }
                    else
                    {
                        // write out any remaining site entries into a final sub file
                        WriteSiteMapFile(prefix, localeId);

                        WrapSiteMapFile(_masterSiteMapContent[prefix]);
                        file.WriteLine(_masterSiteMapContent[prefix].ToString());
                    }
                }
            }

            _masterSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();
            _currentSiteMapContent = new ConcurrentDictionary<string, StringBuilder>();

            _siteMapCount = new ConcurrentDictionary<string, int>();

            _pageCount = 0;
        }

        private void WrapSiteMapFile(StringBuilder sb)
        {
            sb.Insert(0,
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
                + "<urlset xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd http://www.w3.org/1999/xhtml http://www.w3.org/2002/08/xhtml/xhtml1-strict.xsd\" xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">\n");
            sb.AppendLine("</urlset>");

        }

        private Locale GetUrlLocale(List<Locale> locales, string url)
        {
            Locale locale = null;

            locale = locales.FirstOrDefault(l => $"{url}/".StartsWith("/" + l.PagePath + "/", StringComparison.InvariantCultureIgnoreCase));

            return locale;
        }

        private string GetLocaleUrl(Locale locale, string url)
        {
            var parsedUrl = new Uri(url);

            return $"{parsedUrl.Scheme}://{parsedUrl.Host}/{locale.Code.ToLower()}{parsedUrl.PathAndQuery}";
        }

        /// <summary>
        /// True if this service is configured and active.
        /// </summary>
        /// <returns></returns>
        public bool IsConfigured()
        {
            return _isConfigured;
        }

		/// <summary>
		/// Sets _isConfigured
		/// </summary>
		private void UpdateIsConfigured()
        {
			_isConfigured = _cfg.MaxSiteMapPages > 0 && !_cfg.Disabled;
        }

        /// <summary>
        /// Get all the active locales 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private List<Locale> GetAllLocales(Context ctx)
        {
            if (_allLocales != null && _allLocales.Any())
            {
                return _allLocales;
            }

            // Get all the current locales:
            var locales = _locales.Where("").ListAll(ctx).Result;

            if (locales != null && locales.Any())
            {
                _allLocales = locales;
            }
            else
            {
                _allLocales = new List<Locale>();
            }

            return _allLocales;
        }


        /// <summary>
        /// Check if a node has a specific class 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private static bool HasClass(HtmlNode node, string className)
        {
            if (node == null)
            {
                return false;
            }

            var classValue = node.GetAttributeValue("class", string.Empty);

            if (string.IsNullOrWhiteSpace(classValue))
            {
                return false;
            }

            var classList = classValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return classList.Any(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Combine segments of a URL, ensuring no double slashes.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string UrlCombine(params string[] items)
        {
            return string.Join("/", items.Where(u => !string.IsNullOrWhiteSpace(u)).Select(u => u.Trim('/', '\\')));
        }
    }
}

