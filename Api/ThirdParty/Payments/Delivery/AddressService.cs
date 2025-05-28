using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Pages;

namespace Api.Payments;

/// <summary>
/// Handles addresses.
/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
/// </summary>
public partial class AddressService : AutoService<Address>
{
	/// <summary>
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public AddressService(PageService pages) : base(Events.Address)
	{

		Events.Address.BeforeCreate.AddEventListener(async (Context context, Address addr) => {

			if (addr == null)
			{
				return addr;
			}

			// Get all the other addresses, if any:
			var allAddresses = await Where("UserId=?").Bind(context.UserId).ListAll(context);

			if (allAddresses == null || allAddresses.Count == 0)
			{
				// Force set this first one as the default for both:
				addr.IsDefaultBillingAddress = true;
				addr.IsDefaultDeliveryAddress = true;
			}

			if (!addr.IsDefaultBillingAddress && !addr.IsDefaultDeliveryAddress)
			{
				return addr;
			}

			if (allAddresses != null)
			{
				// Replace their default status with the new one.
				foreach (var prevAddr in allAddresses)
				{
					var clearDelivery = prevAddr.IsDefaultDeliveryAddress && addr.IsDefaultDeliveryAddress;
					var clearBilling = prevAddr.IsDefaultBillingAddress && addr.IsDefaultBillingAddress;

					if (clearDelivery || clearBilling)
					{
						await Update(context, prevAddr, (Context context, Address toUpdate, Address orig) =>
						{

							if (clearDelivery)
							{
								toUpdate.IsDefaultDeliveryAddress = false;
							}

							if (clearBilling)
							{
								toUpdate.IsDefaultBillingAddress = false;
							}

						}, DataOptions.IgnorePermissions);
					}
				}
			}

			return addr;
		});

		pages.Install(
			new PageBuilder()
			{
				Url="/cart/address_book",
				Key = "cart_address_book",
				Title = "My addresses",
				BuildBody = (PageBuilder builder) =>
				{
					return builder.AddTemplate(
						new CanvasRenderer.CanvasNode("UI/Payments/AddressBook")
					);
				}
			}
		);


	}
}
