### Using this API

```typescript
import PublishGroupApi, { PublishGroup } from 'Api/PublishGroup';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PublishGroupApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PublishGroupApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PublishGroupApi} filter={ /* filters */ }>
    {(entity: PublishGroup) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# PublishGroup

*Full Type:* `Api.PublishGroups.PublishGroup`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A PublishGroup. Created and manipulated by users but doesn't have revisions itself.

---



# Fields

The following fields are available on this entity:

| Name               | Type        | Nullable | Summary                  |
| ------------------ | ----------- | -------- | ------------------------ |
| Name               | `String`    | Yes      | No description available |
| IsPublished        | `Boolean`   | No       | No description available |
| ReadyForPublishing | `Boolean`   | No       | No description available |
| AutoPublishTimeUtc | `DateTime?` | Yes      | No description available |

# PublishGroup API

This controller provides API methods for the `PublishGroup` entity.

Base URL: `/api/publishgroup`

---

### `GET /api/publishgroup/revision/{id}`

No description available.

```ts
await PublishGroupApi.loadRevision();
```

### `DELETE /api/publishgroup/revision/{id}`

No description available.

```ts
await PublishGroupApi.deleteRevision();
```

### `GET /api/publishgroup/revision/list`

No description available.

```ts
await PublishGroupApi.revisionList();
```

### `POST /api/publishgroup/revision/list`

No description available.

```ts
await PublishGroupApi.revisionList({ filters: ListFilter });
```

### `POST /api/publishgroup/revision/{id}`

No description available.

```ts
await PublishGroupApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/publishgroup/publish/{id}`

No description available.

```ts
await PublishGroupApi.publishRevision();
```

### `POST /api/publishgroup/publish/{id}`

No description available.

```ts
await PublishGroupApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/publishgroup/draft`

No description available.

```ts
await PublishGroupApi.createDraft({ body: Partial<T> });
```

### `GET /api/publishgroup/{id}`

No description available.

```ts
await PublishGroupApi.load();
```

### `DELETE /api/publishgroup/{id}`

No description available.

```ts
await PublishGroupApi.delete();
```

### `GET /api/publishgroup/cache/invalidate/{id}`

No description available.

```ts
await PublishGroupApi.invalidateCachedItem();
```

### `GET /api/publishgroup/cache/invalidate`

No description available.

```ts
await PublishGroupApi.invalidateCache();
```

### `GET /api/publishgroup/list`

No description available.

```ts
await PublishGroupApi.listAll();
```

### `POST /api/publishgroup/list`

No description available.

```ts
await PublishGroupApi.list({ filters: ListFilter });
```

### `POST /api/publishgroup/create`

No description available.

```ts
await PublishGroupApi.create({ body: Partial<T> });
```

### `POST /api/publishgroup/{id}`

No description available.

```ts
await PublishGroupApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/publishgroup/list.pot`

No description available.

```ts
await PublishGroupApi.listPOTUpdate();
```

### `GET /api/publishgroup/list.pot`

No description available.

```ts
await PublishGroupApi.listPOT();
```

### `POST /api/publishgroup/list.pot`

No description available.

```ts
await PublishGroupApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

