
using Api.Contexts;
using Api.Eventing;
using Api.Pages;
using System.Threading.Tasks;

namespace Api.Addresses;

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

        InstallAdminPages("Addresses", "fa:fa-address-book", ["id", "name", "line1"]);

        pages.Install(
			new PageBuilder()
			{
				Url="/address_book",
				Key = "address_book",
				Title = "My addresses",
				BuildBody = (PageBuilder builder) =>
				{
					return builder.AddTemplate(
						new CanvasRenderer.CanvasNode("UI/Address/Book")
                        .With("addressType", 0)
                    );
				}
			}
		);

	}

	/// <summary>
	/// Gets the ISO 3166 tax jurisdiction of the given address by ID.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="addressId"></param>
	/// <returns></returns>
	public async ValueTask<string> GetTaxJurisdiction(Context context, uint addressId)
	{
		var addr = await Get(context, addressId);

		// Todo! Can be e.g. specific US states etc.
		return "GB";
	}
}
