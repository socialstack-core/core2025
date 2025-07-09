using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.PasswordResetRequests;
using System;
using Api.Startup;
using Api.Addresses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.Payments
{
	/// <summary>
	/// Handles delivery times.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class DeliveryService : AutoService
    {
		private DeliveryOptionService _options;
		private AddressService _addresses;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public DeliveryService(DeliveryOptionService options, AddressService addresses)
        {
			_options = options;
			_addresses = addresses;
		}

		/// <summary>
		/// Collects a potentially cached set of delivery estimates for the given shopping cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cart"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public async ValueTask<List<DeliveryOption>> EstimateDelivery(Context context, ShoppingCart cart, CartEstimation options)
		{
			uint addressId = options.DeliveryAddressId;

			var address = await _addresses.Get(context, addressId, DataOptions.IgnorePermissions);

			if (address == null)
			{
				throw new PublicException("A target delivery address has not been set.", "delivery/no_address");
			}

			return await EstimateDelivery(context, cart, address);
		}

		/// <summary>
		/// Collects a potentially cached set of delivery estimates for the given shopping cart and target address.
		/// </summary>
		private async ValueTask<List<DeliveryOption>> EstimateDelivery(Context context, ShoppingCart cart, Address targetAddress)
		{
			// Get the current estimate set for this cart:
			var currentOpts = await _options
				.Where("ShoppingCartId=?", DataOptions.IgnorePermissions)
				.Bind(cart.Id).ListAll(context);

			if (cart.CheckedOut)
			{
				// Readonly
				return currentOpts;
			}

			// If the cart itself has not changed since the last estimate
			// then return the prior set.
			var lastModified = cart.EditedUtc;
			var anHourAgo = DateTime.UtcNow.AddHours(-1);

			if (currentOpts != null && currentOpts.Count > 0)
			{
				var stale = false;

				foreach (var item in currentOpts)
				{
					if (item.CreatedUtc < lastModified || item.CreatedUtc < anHourAgo || item.AddressId != targetAddress.Id)
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
				Cart = cart,
				Target = targetAddress
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
				var estimateJson = Newtonsoft.Json.JsonConvert.SerializeObject(item, jsonSettings);
				var opt = await _options.Create(context, new DeliveryOption()
				{
					AddressId = targetAddress.Id,
					ShoppingCartId = cart.Id,
					InformationJson = estimateJson
				}, DataOptions.IgnorePermissions);

				opts.Add(opt);
			}

			return opts;
		}

		/// <summary>
		/// Json serialization settings for delivery options
		/// </summary>
		private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			},
			Formatting = Formatting.None
		};

	}

}
