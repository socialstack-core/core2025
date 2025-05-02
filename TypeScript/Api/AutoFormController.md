### Using this API

```typescript
import AutoFormControllerApi, { AutoFormController } from 'Api/AutoFormController';
```

# AutoFormController API

This controller provides API methods for the `AutoFormController` entity.

Base URL: `/api/autoform`

---

### `GET /api/autoform/{type}/{name}`

Gets the autoform info for a particular form by type and name. Type is usually content, component or config.

```ts
await AutoFormControllerApi.get();
```

### `GET /api/autoform/all`

GET /v1/autoform/all
            Returns meta about all content autoforms in this API.

```ts
await AutoFormControllerApi.allContentForms();
```

