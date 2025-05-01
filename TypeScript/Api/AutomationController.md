### Using this API

```typescript
import AutomationControllerApi, { AutomationController } from 'Api/AutomationController';
```

# AutomationController API

This controller provides API methods for the `AutomationController` entity.

Base URL: `/api/automation`

---

### `GET /api/automation/list`

GET /v1/automation/list
            Returns meta about automations available from this API. Includes endpoints and content types.

```ts
await AutomationControllerApi.get();
```

### `GET /api/automation/{name}/run`

GET /v1/automation/{name}/run
            Runs the named automation and waits for it to complete.

```ts
await AutomationControllerApi.execute();
```

