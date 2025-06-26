using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using Api.Emails;
using System;
using Api.Pages;
using Api.CanvasRenderer;
using Api.Translate;

namespace Api.Payments
{
	/// <summary>
	/// Handles purchases.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class PurchaseService : AutoService<Purchase>
    {
		private PaymentMethodService _paymentMethods;
		private PaymentGatewayService _gateways;
		private ProductQuantityService _prodQuantities;
		private ProductService _products;
		private SubscriptionService _subscriptions;
		private PriceService _prices;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public PurchaseService(PaymentMethodService paymentMethods, PageService pages, 
			PaymentGatewayService gateways, ProductQuantityService prodQuantities, 
			ProductService products, EmailTemplateService emails, PriceService prices, LocaleService locales) : base(Events.Purchase)
        {
			_paymentMethods = paymentMethods;
			_gateways = gateways;
			_prodQuantities = prodQuantities;
			_products = products;
			_prices = prices;
			
			pages.Install(
				new PageBuilder()
				{
					Url = "/cart/",
					Key = "cart_view",
					Title = "View your shopping cart",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Payments/Cart")
						);
					}
				},
				new PageBuilder()
				{
					Url = "/cart/checkout",
					Key = "cart_checkout",
					Title = "Review your cart",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Payments/Checkout")
						);
					}
				},
				new PageBuilder()
				{
					Url = "/cart/purchases/${purchase.id}",
					Key = "primary:purchase",
					Title = "Viewing purchase",
					PrimaryContentIncludes = "productQuantities",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Payments/Purchase/View").WithPrimaryLink("purchase")
						);
					}
				}
			);
			
			InstallEmails(
				new EmailBuilder()
				{
					Name = "Your payment receipt",
					Subject = "Your payment receipt",
					Key = "payment_receipt",
					BuildBody = (EmailBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("Email/Centered")
							.AppendChild(
								"Thank you! A payment was successfully made for "
							)
							.AppendChild(
								new CanvasNode("UI/Token")
									.With("mode", "customdata")
									.With("fields", new string[] { "printablePrice" })
									.AppendChild("${customData.printablePrice}")
							)
							.AppendChild(
								new CanvasNode("Email/PrimaryButton")
									.With("label", "View payment details")
									.With("target", "/checkout/payment/${customData.paymentId}")
							)
						);
					}
				},
				new EmailBuilder()
				{
					Name = "A payment issue occurred",
					Subject = "A payment issue occurred",
					Key = "payment_fault",
					BuildBody = (EmailBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("Email/Centered")
							.AppendChild(
								"We tried to request a payment of "
							)
							.AppendChild(
								new CanvasNode("UI/Token")
									.With("mode", "customdata")
									.With("fields", new string[] { "printablePrice" })
									.AppendChild("${customData.printablePrice}")
							)
							.AppendChild(
								" but it was unable to go through. This can be because the card used was cancelled, has expired, or there are insufficient funds. If you're not sure, please check with your bank."
							)
							.AppendChild(
								new CanvasNode("Email/PrimaryButton")
									.With("label", "View payment details")
									.With("target", "/checkout/payment/${customData.paymentId}")
							)
						);
					}
				}
			);
			
			Events.Purchase.BeforeCreate.AddEventListener(async (Context context, Purchase purchase) => {

				// Ensure paymentGatewayId is set:
				await EnsureGatewayId(context, purchase);

				// Ensure a locale is set:
				if (purchase.LocaleId == 0)
				{
					purchase.LocaleId = context.LocaleId;
				}

				return purchase;
			});

			Events.Purchase.BeforeUpdate.AddEventListener((Context context, Purchase purchase, Purchase original) => {

				// State change - is it now a successful payment?
				if (purchase.Status != original.Status)
				{
					if (purchase.Status == 202)
					{
						// Send success email:
						var userRecipient = new Recipient(purchase.UserId, purchase.LocaleId);
						userRecipient.CustomData = new
						{
							Purchase = purchase,
							PrintablePrice = PrintPrice(purchase.TotalCost, purchase.CurrencyCode),
							PaymentId = purchase.Id
						};
						var recipients = new List<Recipient>();
						recipients.Add(userRecipient);
						emails.Send(recipients, "payment_receipt");
					}
					else if (purchase.Status > 299)
					{
						// Send failure email:
						var userRecipient = new Recipient(purchase.UserId, purchase.LocaleId);
						userRecipient.CustomData = new
						{
							Purchase = purchase,
							PrintablePrice = PrintPrice(purchase.TotalCost, purchase.CurrencyCode),
							PaymentId = purchase.Id
						};
						var recipients = new List<Recipient>();
						recipients.Add(userRecipient);
						emails.Send(recipients, "payment_fault");
					}
				}

				return new ValueTask<Purchase>(purchase);

			});

		}

		private async ValueTask EnsureGatewayId(Context context, Purchase purchase)
		{
			if (purchase.PaymentMethodId == 0)
			{
				throw new PublicException("No payment gateway specified.", "payment_method_required");
			}

			var dOpts = DataOptions.Default;

			if (context.RoleId == 1)
			{
				// Offline purchases
				dOpts = DataOptions.IgnorePermissions;
			}

			// Get the payment method (must be reachable by the context):
			var paymentMethod = await _paymentMethods.Get(context, purchase.PaymentMethodId, dOpts);

			if (paymentMethod == null)
			{
				// Probably tried to use some other payment method ID.
				throw new PublicException("No payment method specified.", "payment_method_required");
			}

			var gatewayId = paymentMethod.PaymentGatewayId;
			purchase.PaymentGatewayId = gatewayId;

			var gateway = _gateways.Get(gatewayId);

			if (gateway == null)
			{
				throw new PublicException(
					"The gateway that your payment method is through is currently unavailable. If this keeps happening, please let us know.",
					"payment_method_unavailable"
				);
			}
		}

		/// <summary>
		/// Adds the given product quantities to the given purchase. Does not check if they have already been added.
		/// This happens via duplicating the given product quantities to avoid any risk of cart manipulation during checkout.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="toPurchase"></param>
		/// <param name="productQuantities"></param>
		/// <returns>True if at least 1 was added.</returns>
		public async ValueTask<bool> AddProductsUnsaved(Context context, Purchase toPurchase, IEnumerable<ProductQuantity> productQuantities)
		{
			if (productQuantities == null || toPurchase == null)
			{
				return false;
			}

			var atLeastOne = false;

			foreach (var pq in productQuantities)
			{
				// Must clone it:
				var purchaseQuantity = new ProductQuantity()
				{
					ProductId = pq.ProductId,
					Quantity = pq.Quantity,
				};

				// Inform that the given product quantity is being added to a purchase and is ready to be charged.
				// This is the place to inject usage based on reading some other dataset(s).
				purchaseQuantity = await Events.ProductQuantity.BeforeAddToPurchase.Dispatch(context, purchaseQuantity, toPurchase);

				if (purchaseQuantity == null)
				{
					continue;
				}

				var result = await _prodQuantities.Create(context, purchaseQuantity, DataOptions.IgnorePermissions);
				
				// Add to mapping (unsaved):
				toPurchase.Mappings.Add("ProductQuantities", result);

				atLeastOne = true;
			}

			return atLeastOne;
		}
		
		/// <summary>
		/// Gets the products in the given purchase.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="purchase"></param>
		/// <returns></returns>
		public async ValueTask<List<ProductQuantity>> GetProducts(Context context, Purchase purchase)
		{
			return await _prodQuantities.Where("PurchaseId=?", DataOptions.IgnorePermissions).Bind(purchase.Id).ListAll(context);
		}

		/*
		/// <summary>
		/// Exceutes potentially multiple subscriptions in one transaction. For example if someone wants to buy an annual and monthly subscription at the same time.
		/// This could also be whilst paying a one off amount too (in the provided purchase, which can be null).
		/// If a purchase is provided, the items from the subscription(s) will be copied to it and executed together.
		/// Otherwise, a purchase will be created and everything will be added to it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="subscriptions"></param>
		/// <param name="paymentMethod"></param>
		/// <returns></returns>
		public async ValueTask<PurchaseAndAction> MultiExecute(Context context, List<Subscription> subscriptions, PaymentMethod paymentMethod)
		{
			if (subscriptions == null || subscriptions.Count == 0)
			{
				throw new Exception("No subs!");
			}

			if (_subscriptions == null)
			{
				_subscriptions = Services.Get<SubscriptionService>();
			}

			var locale = await context.GetLocale();

			// Create a purchase:
			var purchase = new Purchase()
			{
				LocaleId = locale.Id,
				MultiExecute = true,
				CurrencyCode = locale.CurrencyCode,
				TaxJurisdiction = subscriptions[0].TaxJurisdiction,
				PaymentGatewayId = paymentMethod.PaymentGatewayId,
				PaymentMethodId = paymentMethod.Id,
				UserId = context.UserId
			};
			
			if (subscriptions != null)
			{
				purchase.Mappings.Set("subscriptions", GetSubscriptionIds(subscriptions));
			}

			// Copy the items from the subs to the purchase:
			foreach (var subscription in subscriptions)
			{
				if (subscription == null)
				{
					continue;
				}

				// Get its items, clone to purchase:
				var inSub = await _subscriptions.GetProducts(context, subscription);
				await AddProductsUnsaved(context, purchase, inSub);
			}

			// Save & create:
			purchase = await Create(context, purchase, DataOptions.IgnorePermissions);
			
			// Attempt to fulfil the purchase now:
			return await Execute(context, purchase, paymentMethod);
		}
		
		/// <summary>
		/// Gets the set of subscription IDs suitable for a mapping.
		/// </summary>
		/// <param name="subscriptions"></param>
		/// <returns></returns>
		private List<ulong> GetSubscriptionIds(List<Subscription> subscriptions)
		{
			var set = new List<ulong>();

			foreach (var subscription in subscriptions)
			{
				set.Add(subscription.Id);
			}

			return set;
		}
		*/

		/// <summary>
		/// Creates a purchase and immediately proceeds to executing it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="items"></param>
		/// <param name="pricingInfo"></param>
		/// <param name="contentTypeName"></param>
		/// <param name="contentId"></param>
		/// <param name="paymentMethod"></param>
		/// <param name="excludeTax">Only usable if the customers location is zero rated relative to the shop. 
		/// For example, a UK VAT registered business selling to another VAT registered business in the EU can exclude tax but must use the VEIS system.
		/// UK B2B is not tax exempt in this way.</param>
		/// <param name="dupeKey"></param>
		/// <returns></returns>
		public async ValueTask<PurchaseAndAction> CreateAndExecute(Context context, IEnumerable<ProductQuantity> items, ProductQuantityPricing pricingInfo, string contentTypeName, uint contentId, PaymentMethod paymentMethod, bool excludeTax, ulong dupeKey = 0)
		{
			var locale = await context.GetLocale();

			// Throw if the info has any error info in it (such as invalid coupons, pricing issues etc).
			_prodQuantities.RequireNoErrors(pricingInfo);
			
			var purchase = new Purchase()
			{
				TaxJurisdiction = pricingInfo.TaxJurisdiction,
				CouponId = pricingInfo.CouponId,
				CurrencyCode = pricingInfo.CurrencyCode,
				ContentType = contentTypeName,
				ContentId = contentId,
				ContentAntiDuplication = dupeKey,
				HasSubscriptions = pricingInfo.HasSubscriptionProducts,
				// Tax exclusions are highly regulated. Do not use unless you know the relevant laws.
				TotalCost = excludeTax ? pricingInfo.TotalLessTax : pricingInfo.Total,
				ExcludeTax = excludeTax,
				PaymentMethodId = paymentMethod == null ? 0 : paymentMethod.Id
			};

			// Copying in the product set - this creates new ones, it *does not* use the same PQ Id.
			// This is to avoid modding the quantity during the payment being processed:
			await AddProductsUnsaved(context, purchase, items);

			// Save & create:
			purchase = await Create(context, purchase, DataOptions.IgnorePermissions);

			// Attempt to fulfil it immediately:
			return await Execute(context, purchase, paymentMethod);
		}

		/// <summary>
		/// Requests execution of the given payment. Internally calculates the total.
		/// This is triggered by the frontend after the given purchase has had a payment method attached to it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="purchase"></param>
		/// <param name="paymentMethod"></param>
		/// <returns></returns>
		private async ValueTask<PurchaseAndAction> Execute(Context context, Purchase purchase, PaymentMethod paymentMethod = null)
		{
			// Event to indicate the purchase is about to execute:
			await Events.Purchase.BeforeExecute.Dispatch(context, purchase);

			// If the total is free, we complete immediately, unless it's the first of a subscription payment.
			// If first subscription payment, must authorise the card.
			PaymentGateway gateway;

			if (paymentMethod == null)
			{
				// Deferred execution.
				return new PurchaseAndAction()
				{
					Purchase = purchase
				};
			}

			if (purchase.TotalCost == 0)
			{
				if (purchase.HasSubscriptions)
				{
					// Get the gateway:
					gateway = _gateways.Get(purchase.PaymentGatewayId);

					if (gateway == null)
					{
						throw new PublicException(
							"The gateway providing your payment method is currently unavailable. If this keeps happening please let us know.",
							"gateway_unavailable"
						);
					}

					if (purchase.CurrencyCode == null)
					{
						throw new PublicException(
							"Whoops! Sorry, we messed up. A currency code was missing from a free subscription purchase. It's required to make sure your bank knows what currency we'll be using. If this keeps happening, please let us know.",
							"currency_missing"
						);
					}

					// Ask the gateway to authorise:
					return await gateway.AuthorisePurchase(purchase, new ProductCost() { 
						Amount = purchase.TotalCost,
						CurrencyCode = purchase.CurrencyCode
					}, paymentMethod);
				}
				else
				{
					purchase = await Update(context, purchase, (Context ctx, Purchase toUpdate, Purchase orig) =>
					{
						// 202 for payment success:
						toUpdate.Status = 202;
						toUpdate.PaymentGatewayInternalId = "";

					}, DataOptions.IgnorePermissions);
				}

				return new PurchaseAndAction()
				{
					Purchase = purchase
				};
			}

			// Get the gateway:
			gateway = _gateways.Get(purchase.PaymentGatewayId);

			if (gateway == null)
			{
				throw new PublicException(
					"The gateway providing your payment method is currently unavailable. If this keeps happening please let us know.",
					"gateway_unavailable"
				);
			}

			if (paymentMethod == null)
			{
				// Get the payment method:
				paymentMethod = await _paymentMethods.Get(context, purchase.PaymentMethodId);
			}

			// Ask the gateway to do the thing:
			return await gateway.ExecutePurchase(purchase, new ProductCost() {
				Amount = purchase.TotalCost,
				CurrencyCode = purchase.CurrencyCode
			}, paymentMethod);
		}

	}
    
}
