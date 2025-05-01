### Using this API

```typescript
import AvailableEndpointControllerApi, { AvailableEndpointController } from 'Api/AvailableEndpointController';
```

# AvailableEndpointController API

This controller provides API methods for the `AvailableEndpointController` entity.

Base URL: `/api/v1`

---

### `GET /api/v1/uptime`

Gets the time (in both ticks and as a timestamp) that the service last started at.

```ts
await AvailableEndpointControllerApi.uptime();
```

### `GET /api/v1/get`

GET /v1/
            Returns meta about what's available from this API. Includes endpoints and content types.

```ts
await AvailableEndpointControllerApi.get();
```

