### Using this API

```typescript
import DomainSiteMapControllerApi, { DomainSiteMapController } from 'Api/DomainSiteMapController';
```

# DomainSiteMapController API

This controller provides API methods for the `DomainSiteMapController` entity.

Base URL: `/api/domainsitemapcontroller`

---

### `GET /api/domainsitemapcontroller//sitemap.xml`

Exposes the dynamic site map file

```ts
await DomainSiteMapControllerApi.siteMapXML();
```

