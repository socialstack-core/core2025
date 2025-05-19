using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using System.Text.RegularExpressions;

namespace Api.Payments
{
	/// <summary>
	/// Handles productAttributes.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductAttributeService : AutoService<ProductAttribute>
    {
		private ProductAttributeGroupService _groups;
		private ProductAttributeValueService _attributeValues;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductAttributeService(ProductAttributeGroupService groups, ProductAttributeValueService attributeValues) : base(Events.ProductAttribute)
        {
			_groups = groups;
			_attributeValues = attributeValues;

			// Example admin page install:
			InstallAdminPages("Product Attributes", "fa:fa-rocket", new string[] { "id", "name" });

			// Install some default content.
			Events.Service.AfterStart.AddEventListener(async (Context context, object svc) => {

				// If it doesn't exist at all, it's created.
				var groupLookup = await InstallDefaultGroups(
					context,
					new ProductAttributeGroup()
					{
						Name = "Physical Attributes",
						Key = "physical"
					},
					new ProductAttributeGroup()
					{
						Name = "Dimensions",
						Key = "dimensions",
						ParentGroupKey = "physical"
					},
					new ProductAttributeGroup()
					{
						Name = "Weight",
						Key = "weight",
						ParentGroupKey = "physical"
					},
					new ProductAttributeGroup()
					{
						Name = "Material & Build",
						Key = "material_build",
						ParentGroupKey = "physical"
					},
					new ProductAttributeGroup()
					{
						Name = "Performance & Technicals",
						Key = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Power & Energy",
						Key = "power_energy",
						ParentGroupKey = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Computing",
						Key = "computing",
						ParentGroupKey = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Mechanical",
						Key = "mechanical",
						ParentGroupKey = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Load Ranges",
						Key = "load_ranges",
						ParentGroupKey = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Chemical",
						Key = "chemical",
						ParentGroupKey = "performance_technicals"
					},
					new ProductAttributeGroup()
					{
						Name = "Usage & Compatibility",
						Key = "usage_compatibility"
					},
					new ProductAttributeGroup()
					{
						Name = "Application",
						Key = "application",
						ParentGroupKey = "usage_compatibility"
					},
					new ProductAttributeGroup()
					{
						Name = "Compatibility",
						Key = "compatibility",
						ParentGroupKey = "usage_compatibility"
					},
					new ProductAttributeGroup()
					{
						Name = "Aesthetic & Style",
						Key = "aesthetic_style"
					},
					new ProductAttributeGroup()
					{
						Name = "Design",
						Key = "design",
						ParentGroupKey = "aesthetic_style"
					},
					new ProductAttributeGroup()
					{
						Name = "Branding",
						Key = "branding",
						ParentGroupKey = "aesthetic_style"
					},
					new ProductAttributeGroup()
					{
						Name = "Packaging & Logistics",
						Key = "packaging_logistics"
					},
					new ProductAttributeGroup()
					{
						Name = "Packaging",
						Key = "packaging",
						ParentGroupKey = "packaging_logistics"
					},
					new ProductAttributeGroup()
					{
						Name = "Shipping",
						Key = "shipping",
						ParentGroupKey = "packaging_logistics"
					},
					new ProductAttributeGroup()
					{
						Name = "Regulatory & Certifications",
						Key = "regulatory_certifications"
					},
					new ProductAttributeGroup()
					{
						Name = "Compliance",
						Key = "compliance",
						ParentGroupKey = "regulatory_certifications"
					},
					new ProductAttributeGroup()
					{
						Name = "Warranty",
						Key = "warranty",
						ParentGroupKey = "regulatory_certifications"
					}
				);

				// Next, install default attributes. 
				await InstallDefaults(context, groupLookup, 
					new ProductAttribute {
						Name = "Width",
						ProductAttributeType = 1, // long
						Units = "mm",
						ProductAttributeGroupKey = "dimensions"
					},
					new ProductAttribute
					{
						Name = "Height",
						ProductAttributeType = 2, // double
						Units = "mm",
						ProductAttributeGroupKey = "dimensions"
					},
					new ProductAttribute
					{
						Name = "Length",
						ProductAttributeType = 2, // double
						Units = "mm",
						ProductAttributeGroupKey = "dimensions"
					},
					new ProductAttribute
					{
						Name = "Depth",
						ProductAttributeType = 2, // double
						Units = "mm",
						ProductAttributeGroupKey = "dimensions"
					},
					new ProductAttribute
					{
						Name = "Net Weight",
						ProductAttributeType = 2, // double
						Units = "kg",
						ProductAttributeGroupKey = "weight"
					},
					new ProductAttribute
					{
						Name = "Gross Weight",
						ProductAttributeType = 2, // double
						Units = "kg",
						ProductAttributeGroupKey = "weight"
					},
					new ProductAttribute
					{
						Name = "Material",
						ProductAttributeType = 3, // text (cotton, plastic, aluminium, ..)
						Multiple = true,
						ProductAttributeGroupKey = "material_build"
					},
					new ProductAttribute
					{
						Name = "Finish",
						ProductAttributeType = 3, // text (glossy, matte, ..)
						ProductAttributeGroupKey = "material_build"
					},
					new ProductAttribute
					{
						Name = "Colour",
						ProductAttributeType = 3, // text (white, red, black, ..)
						ProductAttributeGroupKey = "material_build"
					},
					new ProductAttribute
					{
						Name = "Texture",
						ProductAttributeType = 3, // text (rough, smooth, ..)
						ProductAttributeGroupKey = "material_build"
					},
					new ProductAttribute
					{
						Name = "Power Consumption",
						ProductAttributeType = 2, // double
						Units = "W",
						ProductAttributeGroupKey = "power_energy"
					},
					new ProductAttribute
					{
						Name = "Voltage",
						ProductAttributeType = 2, // double
						Units = "V",
						ProductAttributeGroupKey = "power_energy"
					},
					new ProductAttribute
					{
						Name = "Current",
						ProductAttributeType = 2, // double
						Units = "A",
						ProductAttributeGroupKey = "power_energy"
					},
					new ProductAttribute
					{
						Name = "Battery Type",
						ProductAttributeType = 3, // text (Lithium ion, ..)
						ProductAttributeGroupKey = "power_energy"
					},
					new ProductAttribute
					{
						Name = "Battery Capacity",
						ProductAttributeType = 2, // double
						Units = "Ah",
						ProductAttributeGroupKey = "power_energy"
					},
					new ProductAttribute
					{
						Name = "Energy Efficiency Rating",
						ProductAttributeType = 3, // text (A, B, C, ..)
						ProductAttributeGroupKey = "power_energy"
					},

					new ProductAttribute
					{
						Name = "Processing Speed",
						ProductAttributeType = 2, // double
						Units = "GHz",
						ProductAttributeGroupKey = "computing"
					},

					new ProductAttribute
					{
						Name = "Storage Capacity",
						ProductAttributeType = 2, // double
						Units = "GB",
						ProductAttributeGroupKey = "computing"
					},

					new ProductAttribute
					{
						Name = "Acidity",
						ProductAttributeType = 2, // double
						Units = "pH",
						ProductAttributeGroupKey = "chemical"
					},

					new ProductAttribute
					{
						Name = "Rotor Speed",
						ProductAttributeType = 2, // double
						Units = "RPM",
						RangeType = 2,
						ProductAttributeGroupKey = "mechanical"
					},
					
					new ProductAttribute
					{
						Name = "Cylinder count",
						ProductAttributeType = 1, // long
						ProductAttributeGroupKey = "mechanical"
					},
					
					new ProductAttribute
					{
						Name = "Weight capacity",
						ProductAttributeType = 2, // double
						Units = "kg",
						RangeType = 2,
						ProductAttributeGroupKey = "load_ranges"
					},

					new ProductAttribute
					{
						Name = "Usable Outdoors",
						ProductAttributeType = 7, // bool
						ProductAttributeGroupKey = "application"
					},

					new ProductAttribute
					{
						Name = "Intended Use",
						ProductAttributeType = 3, // string (bedroom, kitchen, ..)
						Multiple = true,
						ProductAttributeGroupKey = "application"
					},

					new ProductAttribute
					{
						Name = "Target User",
						ProductAttributeType = 3, // string (adults, pets, children, ..)
						Multiple = true,
						ProductAttributeGroupKey = "application"
					},

					new ProductAttribute
					{
						Name = "Supported Platforms",
						ProductAttributeType = 3, // string (PC, Nintendo Switch, Xbox One, ..)
						Multiple = true,
						ProductAttributeGroupKey = "compatibility"
					},

					new ProductAttribute
					{
						Name = "Connection Types",
						ProductAttributeType = 3, // string (USB-C, Bluetooth, ..)
						Multiple = true,
						ProductAttributeGroupKey = "compatibility"
					},

					new ProductAttribute
					{
						Name = "Style",
						ProductAttributeType = 3, // string (Modern, Rustic, ..)
						ProductAttributeGroupKey = "design"
					},
					
					new ProductAttribute
					{
						Name = "Pattern",
						ProductAttributeType = 3, // string (Checkered, Dotted, ..)
						ProductAttributeGroupKey = "design"
					},
					
					new ProductAttribute
					{
						Name = "Brand Name",
						ProductAttributeType = 3, // string (Apple, Nvidia, ..)
						ProductAttributeGroupKey = "branding"
					},
					
					new ProductAttribute
					{
						Name = "Logo Presence",
						ProductAttributeType = 7, // boolean (yes, no)
						ProductAttributeGroupKey = "branding"
					},
					
					new ProductAttribute
					{
						Name = "Customizable",
						ProductAttributeType = 7, // boolean (yes, no)
						ProductAttributeGroupKey = "branding"
					},

					new ProductAttribute
					{
						Name = "Package Width",
						ProductAttributeType = 1, // long
						Units = "mm",
						ProductAttributeGroupKey = "packaging"
					},
					new ProductAttribute
					{
						Name = "Package Height",
						ProductAttributeType = 2, // double
						Units = "mm",
						ProductAttributeGroupKey = "packaging"
					},
					new ProductAttribute
					{
						Name = "Package Length",
						ProductAttributeType = 2, // double
						Units = "mm",
						ProductAttributeGroupKey = "packaging"
					},

					new ProductAttribute
					{
						Name = "Max Stack Height",
						ProductAttributeType = 2, // double
						Units = "m",
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Max Units Stacked",
						ProductAttributeType = 1, // long
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Stack Load Limit",
						ProductAttributeType = 2, // double
						Units = "kg",
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Pallet Quantity",
						ProductAttributeType = 1, // long
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Fragility",
						ProductAttributeType = 7, // bool
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Explosive",
						ProductAttributeType = 7, // bool
						ProductAttributeGroupKey = "shipping"
					},
					
					new ProductAttribute
					{
						Name = "Certifications",
						ProductAttributeType = 3, // text (CE, FCC, ..)
						ProductAttributeGroupKey = "compliance"
					},
					
					new ProductAttribute
					{
						Name = "Country of Origin",
						ProductAttributeType = 3, // text (United Kingdom, China, ..)
						ProductAttributeGroupKey = "compliance"
					},
					
					new ProductAttribute
					{
						// Just in case some warranties are in days or years - they're just different attributes.
						Key = "warranty_period_months",
						Name = "Warranty Period",
						ProductAttributeType = 1, // long
						Units = "months",
						ProductAttributeGroupKey = "warranty"
					}
				);

				Events.ProductAttribute.BeforeCreate.AddEventListener((Context context, ProductAttribute attrib) => {
					if (attrib == null)
					{
						return new ValueTask<ProductAttribute>(attrib);
					}

					if (string.IsNullOrEmpty(attrib.Key))
					{
						attrib.Key = ToAttributeKey(attrib.Name);
					}

					return new ValueTask<ProductAttribute>(attrib);
				});

				Events.ProductAttribute.AfterCreate.AddEventListener(async (Context context, ProductAttribute attrib) => {
					if (attrib == null)
					{
						return attrib;
					}

					// If it is a boolean (type 7) attribute, add the yes/ no values.
					if (attrib.ProductAttributeType == 7)
					{
						await _attributeValues.Create(context, new ProductAttributeValue() {
							Value = "Yes",
							ProductAttributeId = attrib.Id
						}, DataOptions.IgnorePermissions);
						
						await _attributeValues.Create(context, new ProductAttributeValue() {
							Value = "No",
							ProductAttributeId = attrib.Id
						}, DataOptions.IgnorePermissions);
					}

					return attrib;
				});

				return svc;
			});

			Cache();
		}

		/// <summary>
		/// Generates an attribute key from the given name.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string ToAttributeKey(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return string.Empty;

			// Convert to lowercase
			string result = input.ToLowerInvariant();

			// Remove all non-alphanumeric characters except spaces
			result = Regex.Replace(result, @"[^a-z0-9\s]", "");

			// Replace one or more spaces with a single underscore
			result = Regex.Replace(result, @"\s+", "_");

			return result;
		}

		/// <summary>
		/// Install default attributes.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="groups"></param>
		/// <param name="attribs"></param>
		/// <returns></returns>
		private async ValueTask InstallDefaults(
			Context context,
			Dictionary<string, ProductAttributeGroup> groups,
			params ProductAttribute[] attribs)
		{
			var all = await Where("", DataOptions.IgnorePermissions).ListAll(context);

			// Build a key dict:
			var lookup = new Dictionary<string, ProductAttribute>();

			foreach (var entry in all)
			{
				if (string.IsNullOrEmpty(entry.Key))
				{
					continue;
				}

				lookup[entry.Key] = entry;
			}

			foreach (var attrib in attribs)
			{
				if (string.IsNullOrEmpty(attrib.Key))
				{
					attrib.Key = ToAttributeKey(attrib.Name);
				}

				if (lookup.ContainsKey(attrib.Key))
				{
					// It already exists.
					continue;
				}

				// Create it but only if the group can be located.
				if (
					string.IsNullOrEmpty(attrib.ProductAttributeGroupKey) || 
					!groups.TryGetValue(attrib.ProductAttributeGroupKey, out ProductAttributeGroup parent))
				{
					throw new System.Exception("Unable to install attribute '" + attrib.Name + "' as it has no identifiable parent group. " +
						"Check that the ProductAttributeGroupKey is set and exists.");
				}

				attrib.ProductAttributeGroupId = parent.Id;
				lookup[attrib.Key] = await Create(context, attrib, DataOptions.IgnorePermissions);
			}

		}

		/// <summary>
		/// Install default attribute groups.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="groups"></param>
		/// <returns></returns>
		private async ValueTask<Dictionary<string, ProductAttributeGroup>> InstallDefaultGroups(Context context, params ProductAttributeGroup[] groups)
		{
			// Get all current cats (fast - from the cache):
			var all = await _groups.Where("", DataOptions.IgnorePermissions).ListAll(context);

			// Build a key dict:
			var lookup = new Dictionary<string, ProductAttributeGroup>();

			foreach (var entry in all)
			{
				if (string.IsNullOrEmpty(entry.Key))
				{
					continue;
				}

				lookup[entry.Key] = entry;
			}

			foreach (var group in groups)
			{
				if (string.IsNullOrEmpty(group.Key))
				{
					group.Key = ToAttributeKey(group.Name);
				}

				if (lookup.ContainsKey(group.Key))
				{
					// It already exists.
					continue;
				}

				// Creating it now. Get the parent first if it has one:
				if (!string.IsNullOrEmpty(group.ParentGroupKey))
				{
					if (!lookup.TryGetValue(group.ParentGroupKey, out ProductAttributeGroup parent))
					{
						throw new System.Exception("Unable to install attribute groups: Please put parent groups in the array first. " +
							"'" + group.Key + "' group tried to use parent '" + group.ParentGroupKey +  "' but it does not exist at this point.");
					}

					group.ParentGroupId = parent.Id;
				}

				lookup[group.Key] = await _groups.Create(context, group, DataOptions.IgnorePermissions);
			}

			return lookup;
		}
	}
    
}
