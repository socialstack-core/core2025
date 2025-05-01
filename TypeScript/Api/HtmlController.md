### Using this API

```typescript
import HtmlControllerApi, { HtmlController } from 'Api/HtmlController';
```

# HtmlController API

This controller provides API methods for the `HtmlController` entity.

Base URL: `/api/htmlcontroller`

---

### `POST /api/htmlcontroller//pack/static-assets/mobile-html`

Lists all available static files.

```ts
await HtmlControllerApi.getMobileHtml({ mobileMeta: MobilePageMeta });
```

### `GET /api/htmlcontroller//pack/rte.html`

RTE config popup base HTML.

```ts
await HtmlControllerApi.getRteConfigPage();
```

