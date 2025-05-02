### Using this API

```typescript
import ComponentGroupApi, { ComponentGroup } from 'Api/ComponentGroup';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ComponentGroupApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ComponentGroupApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ComponentGroupApi} filter={ /* filters */ }>
    {(entity: ComponentGroup) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ComponentGroup

*Full Type:* `Api.Components.ComponentGroup`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ComponentGroup

---



# Fields

The following fields are available on this entity:

| Name              | Type     | Nullable | Summary                  |
| ----------------- | -------- | -------- | ------------------------ |
| Name              | `String` | Yes      | No description available |
| AllowedComponents | `String` | Yes      | No description available |
| Role              | `UInt32` | No       | No description available |

# ComponentGroup API

This controller provides API methods for the `ComponentGroup` entity.

Base URL: `/api/componentgroup`

---

### `GET /api/componentgroup/revision/{id}`

No description available.

```ts
await ComponentGroupApi.loadRevision();
```

### `DELETE /api/componentgroup/revision/{id}`

No description available.

```ts
await ComponentGroupApi.deleteRevision();
```

### `GET /api/componentgroup/revision/list`

No description available.

```ts
await ComponentGroupApi.revisionList();
```

### `POST /api/componentgroup/revision/list`

No description available.

```ts
await ComponentGroupApi.revisionList({ filters: ListFilter });
```

### `POST /api/componentgroup/revision/{id}`

No description available.

```ts
await ComponentGroupApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/componentgroup/publish/{id}`

No description available.

```ts
await ComponentGroupApi.publishRevision();
```

### `POST /api/componentgroup/publish/{id}`

No description available.

```ts
await ComponentGroupApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/componentgroup/draft`

No description available.

```ts
await ComponentGroupApi.createDraft({ body: Partial<T> });
```

### `GET /api/componentgroup/{id}`

No description available.

```ts
await ComponentGroupApi.load();
```

### `DELETE /api/componentgroup/{id}`

No description available.

```ts
await ComponentGroupApi.delete();
```

### `GET /api/componentgroup/cache/invalidate/{id}`

No description available.

```ts
await ComponentGroupApi.invalidateCachedItem();
```

### `GET /api/componentgroup/cache/invalidate`

No description available.

```ts
await ComponentGroupApi.invalidateCache();
```

### `GET /api/componentgroup/list`

No description available.

```ts
await ComponentGroupApi.listAll();
```

### `POST /api/componentgroup/list`

No description available.

```ts
await ComponentGroupApi.list({ filters: ListFilter });
```

### `POST /api/componentgroup/create`

No description available.

```ts
await ComponentGroupApi.create({ body: Partial<T> });
```

### `POST /api/componentgroup/{id}`

No description available.

```ts
await ComponentGroupApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/componentgroup/list.pot`

No description available.

```ts
await ComponentGroupApi.listPOTUpdate();
```

### `GET /api/componentgroup/list.pot`

No description available.

```ts
await ComponentGroupApi.listPOT();
```

### `POST /api/componentgroup/list.pot`

No description available.

```ts
await ComponentGroupApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

