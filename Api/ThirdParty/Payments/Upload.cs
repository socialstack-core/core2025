using Api.Payments;
using Api.Startup;

namespace Api.Uploader;


[ListAs("ProductImages", IsPrimary = false)]
[ImplicitFor("ProductImages", typeof(Product))]

[ListAs("ProductSpecifications", IsPrimary = false)]
[ImplicitFor("ProductSpecifications", typeof(Product))]

[ListAs("ProductManuals", IsPrimary = false)]
[ImplicitFor("ProductManuals", typeof(Product))]

public partial class Upload { }