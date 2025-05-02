### Using this API

```typescript
import RedirectApi, { Redirect } from 'Api/Redirect';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={RedirectApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return RedirectApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={RedirectApi} filter={ /* filters */ }>
    {(entity: Redirect) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Redirect

*Full Type:* `Api.Redirects.Redirect`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Redirect

---



# Fields

The following fields are available on this entity:

| Name              | Type      | Nullable | Summary                  |
| ----------------- | --------- | -------- | ------------------------ |
| From              | `String`  | Yes      | No description available |
| To                | `String`  | Yes      | No description available |
| PermanentRedirect | `Boolean` | No       | No description available |

# Redirect API

This controller provides API methods for the `Redirect` entity.

Base URL: `/api/redirect`

---

### `GET /api/redirect/revision/{id}`

No description available.

```ts
await RedirectApi.loadRevision();
```

### `DELETE /api/redirect/revision/{id}`

No description available.

```ts
await RedirectApi.deleteRevision();
```

### `GET /api/redirect/revision/list`

No description available.

```ts
await RedirectApi.revisionList();
```

### `POST /api/redirect/revision/list`

No description available.

```ts
await RedirectApi.revisionList({ filters: ListFilter });
```

### `POST /api/redirect/revision/{id}`

No description available.

```ts
await RedirectApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/redirect/publish/{id}`

No description available.

```ts
await RedirectApi.publishRevision();
```

### `POST /api/redirect/publish/{id}`

No description available.

```ts
await RedirectApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/redirect/draft`

No description available.

```ts
await RedirectApi.createDraft({ body: Partial<T> });
```

### `GET /api/redirect/{id}`

No description available.

```ts
await RedirectApi.load();
```

### `DELETE /api/redirect/{id}`

No description available.

```ts
await RedirectApi.delete();
```

### `GET /api/redirect/cache/invalidate/{id}`

No description available.

```ts
await RedirectApi.invalidateCachedItem();
```

### `GET /api/redirect/cache/invalidate`

No description available.

```ts
await RedirectApi.invalidateCache();
```

### `GET /api/redirect/list`

No description available.

```ts
await RedirectApi.listAll();
```

### `POST /api/redirect/list`

No description available.

```ts
await RedirectApi.list({ filters: ListFilter });
```

### `POST /api/redirect/create`

No description available.

```ts
await RedirectApi.create({ body: Partial<T> });
```

### `POST /api/redirect/{id}`

No description available.

```ts
await RedirectApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/redirect/list.pot`

No description available.

```ts
await RedirectApi.listPOTUpdate();
```

### `GET /api/redirect/list.pot`

No description available.

```ts
await RedirectApi.listPOT();
```

### `POST /api/redirect/list.pot`

No description available.

```ts
await RedirectApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

