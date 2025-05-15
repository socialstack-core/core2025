### Using this API

```typescript
import HtmlControllerApi, { HtmlController } from 'Api/HtmlController';
```

# HtmlController API

This controller provides API methods for the `HtmlController` entity.

Base URL: `/api/htmlcontroller`

---

### `GET /api/htmlcontroller//pack/rte.html`

RTE config popup base HTML.

```ts
await HtmlControllerApi.getRteConfigPage();
```

