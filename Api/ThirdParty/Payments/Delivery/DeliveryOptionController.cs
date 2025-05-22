using Microsoft.AspNetCore.Mvc;

namespace Api.Payments;

/// <summary>Handles delivery option endpoints.</summary>
[Route("v1/deliveryoption")]
public partial class DeliveryOptionController : AutoController<DeliveryOption>
{
}