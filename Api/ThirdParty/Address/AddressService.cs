
using Api.Eventing;
using Api.Pages;

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
}
