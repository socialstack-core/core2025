using Api.CanvasRenderer;
using Api.Contexts;
using Api.Pages;
using Api.SocketServerLibrary;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Api.SearchCrawler;

/// <summary>
/// Metadata for a page that just got crawled (and rendered in to HTML).
/// </summary>
public class CrawledPageMeta
{
	/// <summary>
	/// The page that got crawled. This gives you the BodyJson amongst other things.
	/// </summary>
	public Page Page;

	/// <summary>
	/// The complete html body, compressed with gzip.
	/// </summary>
	public byte[] BodyCompressedBytes;

	/// <summary>
	/// Decompresses the compressed bytes and returns it in a memorystream.
	/// </summary>
	/// <returns></returns>
	public GZipStream GetDecompressedBody()
	{
		var ms = new MemoryStream(BodyCompressedBytes);
		var gz = new GZipStream(ms, CompressionMode.Decompress);
		return gz;
	}

	/// <summary>
	/// Decompresses the bytes and returns it as a stream.
	/// </summary>
	/// <returns></returns>
	public StreamReader GetBodyStream()
	{
		return new StreamReader(GetDecompressedBody(), Encoding.UTF8);
	}

	/// <summary>
	/// The URL that was used when rendering the page. 
	/// Will always be a resolved URL (Url does not necessarily equal Page.Url) meaning any tokens have been given values here.
	/// Use Page.Url if you want the original with ${tokens} still in it.
	/// </summary>
	public string Url;

    /// <summary>
    /// The Title that was used when rendering the page. 
    /// Use Page.Title if you want the original with ${tokens} still in it.
    /// </summary>
    public string Title;
}
