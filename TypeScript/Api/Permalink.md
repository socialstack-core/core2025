### Using this API

```typescript
import PermalinkApi, { Permalink } from 'Api/Permalink';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PermalinkApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PermalinkApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PermalinkApi} filter={ /* filters */ }>
    {(entity: Permalink) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Permalink

*Full Type:* `Api.Pages.Permalink`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Permalink

---



# Fields

The following fields are available on this entity:

| Name | Type | Nullable | Summary |
| ---- | ---- | -------- | ------- |

# Permalink API

This controller provides API methods for the `Permalink` entity.

Base URL: `/api/permalink`

---

### `GET /api/permalink/revision/{id}`

No description available.

```ts
await PermalinkApi.loadRevision();
```

### `DELETE /api/permalink/revision/{id}`

No description available.

```ts
await PermalinkApi.deleteRevision();
```

### `GET /api/permalink/revision/list`

No description available.

```ts
await PermalinkApi.revisionList();
```

### `POST /api/permalink/revision/list`

No description available.

```ts
await PermalinkApi.revisionList({ filters: ListFilter });
```

### `POST /api/permalink/revision/{id}`

No description available.

```ts
await PermalinkApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/permalink/publish/{id}`

No description available.

```ts
await PermalinkApi.publishRevision();
```

### `POST /api/permalink/publish/{id}`

No description available.

```ts
await PermalinkApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/permalink/draft`

No description available.

```ts
await PermalinkApi.createDraft({ body: Partial<T> });
```

### `GET /api/permalink/{id}`

No description available.

```ts
await PermalinkApi.load();
```

### `DELETE /api/permalink/{id}`

No description available.

```ts
await PermalinkApi.delete();
```

### `GET /api/permalink/cache/invalidate/{id}`

No description available.

```ts
await PermalinkApi.invalidateCachedItem();
```

### `GET /api/permalink/cache/invalidate`

No description available.

```ts
await PermalinkApi.invalidateCache();
```

### `GET /api/permalink/list`

No description available.

```ts
await PermalinkApi.listAll();
```

### `POST /api/permalink/list`

No description available.

```ts
await PermalinkApi.list({ filters: ListFilter });
```

### `POST /api/permalink/create`

No description available.

```ts
await PermalinkApi.create({ body: Partial<T> });
```

### `POST /api/permalink/{id}`

No description available.

```ts
await PermalinkApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/permalink/list.pot`

No description available.

```ts
await PermalinkApi.listPOTUpdate();
```

### `GET /api/permalink/list.pot`

No description available.

```ts
await PermalinkApi.listPOT();
```

### `POST /api/permalink/list.pot`

No description available.

```ts
await PermalinkApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

