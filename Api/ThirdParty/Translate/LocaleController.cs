﻿using System.Threading.Tasks;
using Api.Contexts;
using Api.Eventing;
using Microsoft.AspNetCore.Mvc;


namespace Api.Translate
{
    /// <summary>
    /// Handles locale endpoints.
    /// </summary>

    [Route("v1/locale")]
	public partial class LocaleController : AutoController<Locale>
	{
		
		/// <summary>
		/// GET /v1/locale/set/2/
		/// Sets locale by its ID.
		/// </summary>
		[HttpGet("set/{id}")]
		public virtual async ValueTask<Context> Set(Context context, [FromRoute] uint id)
		{
			// Set locale ID:
			context.LocaleId = id;
			await Events.Locale.SetLocale.Dispatch(context, id);
			return context;
		}

    }

}
