using Nest;
using System;
using System.Collections.Generic;

namespace Api.SearchElastic
{
    /// <summary>
    /// The definition of an indedxed document within elastic
    /// </summary>
    /// 

    public partial class Document
    {
        /// <summary>
        /// An ID for this document.
        /// </summary>
        [Text]
        public string Id { get; set; }

        /// <summary>
        /// The author/creator for the document content
        /// </summary>
        // No nest attribute so will be indexed as text for searching with a keyword sub field for sorting
        public string Author { get; set; }

        /// <summary>
        /// The hash of a pages content.
        /// </summary>
        [Text]
        public string Hash { get; set; }

        /// <summary>
        /// The title of a page
        /// </summary>
        // No nest attribute so will be indexed as text for searching with a keyword sub field for sorting
        public string Title { get; set; }

        /// <summary>
        /// The content of the page
        /// </summary>
        [Text]
        public string Content { get; set; }

        /// <summary>
        /// The extracted text from any associated metadata (includes)
        /// </summary>
        [Text]
        public string MetaDataText { get; set; }

        /// <summary>
        /// Page keywords
        /// </summary>
        [Text]
        public string Keywords { get; set; }

        /// <summary>
        /// Headings on a page
        /// </summary>
        [Text]
        public IEnumerable<string> Headings { get; set; }

        /// <summary>
        /// The URL of a page
        /// </summary>
        [Text]
        public string Url { get; set; }

        /// <summary>
        /// Page checksum
        /// </summary>
        [Text]
        public string CheckSum { get; set; }

        /// <summary>
        /// The content type of the page.
        /// </summary>
        // keyword fields to allow for filtering only no text search
        [Keyword]
        public string ContentType { get; set; }

        /// <summary>
        /// The icon/image to associate with the document, usually a Ref value to icon or image
        /// </summary>
        // non indexed field used when rendering
        [Text(Index = false)]
        public string Image { get; set; }

        /// <summary>
        /// The tags of the page
        /// </summary>
        [Keyword]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The taxonomy of the page.
        /// </summary>
        // dictionary to store categorised tags
        [Object]
        public Dictionary<string, List<string>> Taxonomy { get; set; }

        /// <summary>
        /// The timestamp of the last index on this document.
        /// </summary>
        [Keyword]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The timestamp of the last change on this document.
        /// </summary>
        [Keyword]
        public DateTime? EditedUtc { get; set; }

        /// <summary>
        /// Highlights on the page.
        /// </summary>
        // non indexed field used when rendering
        [Text(Index = false)]
        public string Highlights { get; set; }

        /// <summary>
        /// Search score
        /// </summary>
        [Text(Ignore = true)]
        public double? Score { get; set; }
        
        /// <summary>
        /// The primary object of the page.
        /// </summary>
        // dynamic object to hold primary data
        [Object]
        public dynamic PrimaryObject { get; set; }

        /// <summary>
        /// metadata/includes added by the current application
        /// </summary>
        [Object]
        public Dictionary<string, List<object>> MetaData { get; set; }
    }
}
