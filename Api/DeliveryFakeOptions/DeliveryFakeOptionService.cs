using Api.Contexts;
using Api.Eventing;
using Api.Pages;
using Api.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.DeliveryFakeOptions;


/// <summary>
/// Fake delivery options to enable checking out physical products.
/// </summary>
public class DeliveryFakeOptionService : AutoService
{
	
	/// <summary>
	/// Fake delivery options to enable checking out physical products.
	/// </summary>
	public DeliveryFakeOptionService()
	{

		Events.PaymentMethod.AuthoriseBuyNowPayLater.AddEventListener((Context context, BuyNowPayLater bnpl) => {
			// Authorized for all logged in users here.
			bnpl.Authorized = true;
			return new ValueTask<BuyNowPayLater>(bnpl);
		});

		Events.Page.BeforePageInstall.AddEventListener((Context context, PageBuilder builder) =>
		{
			if (builder == null)
			{
				return new ValueTask<PageBuilder>(builder);
			}

			// - Add the container for all frontend pages -
			if (!builder.IsAdmin)
			{
				builder.SetTemplate("site_default_contained");
			}

			return new ValueTask<PageBuilder>(builder);
		});

		Events.DeliveryOption.Estimate.AddEventListener(async (Context context, DeliveryEstimates estimates) => {

			if (estimates.Options != null)
			{
				// Another handler got it.
				return estimates;
			}

			var locale = await context.GetLocale();

			// Add fake options.
			var today = DateTime.UtcNow;

			// 2pm UTC tomorrow and in 48h
			var tomorrow2 = new DateTime(today.Year, today.Month, today.Day + 1, 14, 00, 00);
			var tomorrowAfter2 = new DateTime(today.Year, today.Month, today.Day + 2, 14, 00, 00);
			var nextWeek = new DateTime(today.Year, today.Month, today.Day + 7, 00, 00, 00);

			estimates.Options = new List<DeliveryEstimate>()
			{
				new DeliveryEstimate() {
					Price = 350,
					Currency = locale.CurrencyCode,
					Deliveries = [
						new DeliveryInfo(){
							DeliveryName = "Royal Mail",
							DeliveryNotes = "Express 24h",
							SlotStartUtc = tomorrow2,
							TimeWindowLength = 30
						}
					]
				},
				new DeliveryEstimate() {
					Price = 100,
					Currency = locale.CurrencyCode,
					Deliveries = [
						new DeliveryInfo(){
							DeliveryName = "Royal Mail",
							DeliveryNotes = "48h",
							SlotStartUtc = tomorrowAfter2,
							TimeWindowLength = 30
						},
						new DeliveryInfo() {
							DeliveryName = "Long range pack mule",
							DeliveryNotes = "Please consider providing water",
							SlotStartUtc = nextWeek,
							TimeWindowLength = 0
						}
					]
				}
			};

			return estimates;
		}, 11);

	}
	
}