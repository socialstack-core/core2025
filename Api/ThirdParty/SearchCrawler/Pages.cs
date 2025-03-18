﻿using Api.AutoForms;

namespace Api.Pages
{
    public partial class Page
    {
        /// <summary>
        /// Exclude this page from search indexing
        /// </summary>
        [Data("hint", "Exclude this page from search indexing")]
		[Order(1)]
		public bool ExcludeFromSearch; 

    }
}
