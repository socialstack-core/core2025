### Using this API

```typescript
import SiteDomainApi, { SiteDomain } from 'Api/SiteDomain';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={SiteDomainApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return SiteDomainApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={SiteDomainApi} filter={ /* filters */ }>
    {(entity: SiteDomain) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# SiteDomain

*Full Type:* `Api.SiteDomains.SiteDomain`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A SiteDomain

---



# Fields

The following fields are available on this entity:

| Name               | Type      | Nullable | Summary                  |
| ------------------ | --------- | -------- | ------------------------ |
| ExcludeFromSiteMap | `Boolean` | No       | No description available |
| Name               | `String`  | Yes      | No description available |
| Code               | `String`  | Yes      | No description available |
| IsDisabled         | `Boolean` | No       | No description available |
| IsPrimary          | `Boolean` | No       | No description available |
| IsRoot             | `Boolean` | No       | No description available |
| Domain             | `String`  | Yes      | No description available |

# SiteDomain API

This controller provides API methods for the `SiteDomain` entity.

Base URL: `/api/sitedomain`

---

### `GET /api/sitedomain/revision/{id}`

No description available.

```ts
await SiteDomainApi.loadRevision();
```

### `DELETE /api/sitedomain/revision/{id}`

No description available.

```ts
await SiteDomainApi.deleteRevision();
```

### `GET /api/sitedomain/revision/list`

No description available.

```ts
await SiteDomainApi.revisionList();
```

### `POST /api/sitedomain/revision/list`

No description available.

```ts
await SiteDomainApi.revisionList({ filters: ListFilter });
```

### `POST /api/sitedomain/revision/{id}`

No description available.

```ts
await SiteDomainApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/sitedomain/publish/{id}`

No description available.

```ts
await SiteDomainApi.publishRevision();
```

### `POST /api/sitedomain/publish/{id}`

No description available.

```ts
await SiteDomainApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/sitedomain/draft`

No description available.

```ts
await SiteDomainApi.createDraft({ body: Partial<T> });
```

### `GET /api/sitedomain/{id}`

No description available.

```ts
await SiteDomainApi.load();
```

### `DELETE /api/sitedomain/{id}`

No description available.

```ts
await SiteDomainApi.delete();
```

### `GET /api/sitedomain/cache/invalidate/{id}`

No description available.

```ts
await SiteDomainApi.invalidateCachedItem();
```

### `GET /api/sitedomain/cache/invalidate`

No description available.

```ts
await SiteDomainApi.invalidateCache();
```

### `GET /api/sitedomain/list`

No description available.

```ts
await SiteDomainApi.listAll();
```

### `POST /api/sitedomain/list`

No description available.

```ts
await SiteDomainApi.list({ filters: ListFilter });
```

### `POST /api/sitedomain/create`

No description available.

```ts
await SiteDomainApi.create({ body: Partial<T> });
```

### `POST /api/sitedomain/{id}`

No description available.

```ts
await SiteDomainApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/sitedomain/list.pot`

No description available.

```ts
await SiteDomainApi.listPOTUpdate();
```

### `GET /api/sitedomain/list.pot`

No description available.

```ts
await SiteDomainApi.listPOT();
```

### `POST /api/sitedomain/list.pot`

No description available.

```ts
await SiteDomainApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

