using Microsoft.AspNetCore.Mvc;

namespace Api.Payments;

/// <summary>Handles address endpoints.</summary>
[Route("v1/address")]
public partial class AddressController : AutoController<Address>
{
}