### Using this API

```typescript
import StripeControllerApi, { StripeController } from 'Api/StripeController';
```

# StripeController API

This controller provides API methods for the `StripeController` entity.

Base URL: `/api/stripe-gateway`

---

### `GET /api/stripe-gateway/setup`

Create a setup intent.

```ts
await StripeControllerApi.setupIntent();
```

### `POST /api/stripe-gateway/webhook`

Updates a purchase based on a webhook event from a stripe payment.

```ts
await StripeControllerApi.webhook();
```

