### Using this API

```typescript
import PermissionControllerApi, { PermissionController } from 'Api/PermissionController';
```

# PermissionController API

This controller provides API methods for the `PermissionController` entity.

Base URL: `/api/permission`

---

### `GET /api/permission/list`

GET /v1/permission/list
            Returns meta about the list of available roles and their permission set.

```ts
await PermissionControllerApi.list();
```

