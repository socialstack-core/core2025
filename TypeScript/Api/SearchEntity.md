### Using this API

```typescript
import SearchEntityApi, { SearchEntity } from 'Api/SearchEntity';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={SearchEntityApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return SearchEntityApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={SearchEntityApi} filter={ /* filters */ }>
    {(entity: SearchEntity) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# SearchEntity

*Full Type:* `Api.SearchEntities.SearchEntity`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A SearchEntity

---



# Fields

The following fields are available on this entity:

| Name        | Type     | Nullable | Summary                  |
| ----------- | -------- | -------- | ------------------------ |
| ContentType | `String` | Yes      | No description available |
| ContentId   | `UInt64` | No       | No description available |
| Action      | `String` | Yes      | No description available |

# SearchEntity API

This controller provides API methods for the `SearchEntity` entity.

Base URL: `/api/searchentity`

---

### `GET /api/searchentity/revision/{id}`

No description available.

```ts
await SearchEntityApi.loadRevision();
```

### `DELETE /api/searchentity/revision/{id}`

No description available.

```ts
await SearchEntityApi.deleteRevision();
```

### `GET /api/searchentity/revision/list`

No description available.

```ts
await SearchEntityApi.revisionList();
```

### `POST /api/searchentity/revision/list`

No description available.

```ts
await SearchEntityApi.revisionList({ filters: ListFilter });
```

### `POST /api/searchentity/revision/{id}`

No description available.

```ts
await SearchEntityApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/searchentity/publish/{id}`

No description available.

```ts
await SearchEntityApi.publishRevision();
```

### `POST /api/searchentity/publish/{id}`

No description available.

```ts
await SearchEntityApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/searchentity/draft`

No description available.

```ts
await SearchEntityApi.createDraft({ body: Partial<T> });
```

### `GET /api/searchentity/{id}`

No description available.

```ts
await SearchEntityApi.load();
```

### `DELETE /api/searchentity/{id}`

No description available.

```ts
await SearchEntityApi.delete();
```

### `GET /api/searchentity/cache/invalidate/{id}`

No description available.

```ts
await SearchEntityApi.invalidateCachedItem();
```

### `GET /api/searchentity/cache/invalidate`

No description available.

```ts
await SearchEntityApi.invalidateCache();
```

### `GET /api/searchentity/list`

No description available.

```ts
await SearchEntityApi.listAll();
```

### `POST /api/searchentity/list`

No description available.

```ts
await SearchEntityApi.list({ filters: ListFilter });
```

### `POST /api/searchentity/create`

No description available.

```ts
await SearchEntityApi.create({ body: Partial<T> });
```

### `POST /api/searchentity/{id}`

No description available.

```ts
await SearchEntityApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/searchentity/list.pot`

No description available.

```ts
await SearchEntityApi.listPOTUpdate();
```

### `GET /api/searchentity/list.pot`

No description available.

```ts
await SearchEntityApi.listPOT();
```

### `POST /api/searchentity/list.pot`

No description available.

```ts
await SearchEntityApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

