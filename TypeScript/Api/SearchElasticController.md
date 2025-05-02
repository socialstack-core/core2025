### Using this API

```typescript
import SearchElasticControllerApi, { SearchElasticController } from 'Api/SearchElasticController';
```

# SearchElasticController API

This controller provides API methods for the `SearchElasticController` entity.

Base URL: `/api/sitesearch`

---

### `POST /api/sitesearch/query`

Exposes the site search

```ts
await SearchElasticControllerApi.query({ filters: Partial<T> });
```

### `POST /api/sitesearch/taxonomy`

Exposes the taxonomy values based on categories

```ts
await SearchElasticControllerApi.taxonomy({ filters: Partial<T> });
```

### `GET /api/sitesearch/reset`

Reset the indexer

```ts
await SearchElasticControllerApi.reset();
```

### `GET /api/sitesearch/reset/index/{indexname}`

Reset a single index (delete all documents)

```ts
await SearchElasticControllerApi.resetIndex();
```

### `GET /api/sitesearch/delete/index/{indexname}`

Reset a single index (delete all documents)

```ts
await SearchElasticControllerApi.deleteIndex();
```

### `GET /api/sitesearch/health`

Exposes the current status of the elastic store

```ts
await SearchElasticControllerApi.health();
```

### `GET /api/sitesearch/shards`

Exposes details of the current shards in the elastic store

```ts
await SearchElasticControllerApi.shards();
```

