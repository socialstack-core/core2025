using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.PasswordResetRequests;
using System;

namespace Api.Payments
{
	/// <summary>
	/// Handles delivery times.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class DeliveryService : AutoService
    {
		private DeliveryOptionService _options;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public DeliveryService(DeliveryOptionService options)
        {
			_options = options;
		}

		/// <summary>
		/// Collects a potentially cached set of delivery estimates for the given shopping cart.
		/// </summary>
		public async ValueTask<List<DeliveryOption>> EstimateDelivery(Context context, ShoppingCart cart)
		{
			// Get the current estimate set for this cart:
			var currentOpts = await _options
				.Where("ShoppingCartId=?", DataOptions.IgnorePermissions)
				.Bind(cart.Id).ListAll(context);

			// If the cart itself has not changed since the last estimate
			// then return the prior set.
			var lastModified = cart.EditedUtc;
			var anHourAgo = DateTime.UtcNow.AddHours(-1);

			if (currentOpts != null && currentOpts.Count > 0)
			{
				var stale = false;

				foreach (var item in currentOpts)
				{
					if (item.CreatedUtc < lastModified || item.CreatedUtc < anHourAgo)
					{
						// It's older than the cart is, or more than an hour old. Regen them all.
						stale = true;
						break;
					}
				}

				if (stale)
				{
					// Cull them all.
					foreach (var item in currentOpts)
					{
						await _options.Delete(context, item, DataOptions.IgnorePermissions);
					}
				}
				else
				{
					// As-is
					return currentOpts;
				}
			}

			// Collect the estimates now. This is very site specific so you'll need to create a handler for this event.
			var estimate = await Events.DeliveryOption.Estimate.Dispatch(context, new DeliveryEstimates() {
				Cart = cart
			});

			if (estimate.Options == null)
			{
				// Not physically delivered or deliverable.
				return null;
			}

			// Store them:
			var opts = new List<DeliveryOption>();

			foreach (var item in estimate.Options)
			{
				var estimateJson = Newtonsoft.Json.JsonConvert.SerializeObject(item);
				var opt = await _options.Create(context, new DeliveryOption()
				{
					ShoppingCartId = cart.Id,
					InformationJson = estimateJson
				}, DataOptions.IgnorePermissions);

				opts.Add(opt);
			}

			return opts;
		}
		
	}
    
}
