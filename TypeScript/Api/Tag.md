### Using this API

```typescript
import TagApi, { Tag } from 'Api/Tag';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={TagApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return TagApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={TagApi} filter={ /* filters */ }>
    {(entity: Tag) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Tag

*Full Type:* `Api.Tags.Tag`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A tag.
            These are the primary taxonomy mechanism; any site content can be grouped up in multiple tags.

---



# Fields

The following fields are available on this entity:

| Name        | Type      | Nullable | Summary                  |
| ----------- | --------- | -------- | ------------------------ |
| Order       | `Int32`   | No       | No description available |
| IsPrice     | `Boolean` | No       | No description available |
| Name        | `String`  | Yes      | No description available |
| Description | `String`  | Yes      | No description available |
| FeatureRef  | `String`  | Yes      | No description available |
| IconRef     | `String`  | Yes      | No description available |
| HexColor    | `String`  | Yes      | No description available |

# Tag API

This controller provides API methods for the `Tag` entity.

Base URL: `/api/tag`

---

### `GET /api/tag/revision/{id}`

No description available.

```ts
await TagApi.loadRevision();
```

### `DELETE /api/tag/revision/{id}`

No description available.

```ts
await TagApi.deleteRevision();
```

### `GET /api/tag/revision/list`

No description available.

```ts
await TagApi.revisionList();
```

### `POST /api/tag/revision/list`

No description available.

```ts
await TagApi.revisionList({ filters: ListFilter });
```

### `POST /api/tag/revision/{id}`

No description available.

```ts
await TagApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/tag/publish/{id}`

No description available.

```ts
await TagApi.publishRevision();
```

### `POST /api/tag/publish/{id}`

No description available.

```ts
await TagApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/tag/draft`

No description available.

```ts
await TagApi.createDraft({ body: Partial<T> });
```

### `GET /api/tag/{id}`

No description available.

```ts
await TagApi.load();
```

### `DELETE /api/tag/{id}`

No description available.

```ts
await TagApi.delete();
```

### `GET /api/tag/cache/invalidate/{id}`

No description available.

```ts
await TagApi.invalidateCachedItem();
```

### `GET /api/tag/cache/invalidate`

No description available.

```ts
await TagApi.invalidateCache();
```

### `GET /api/tag/list`

No description available.

```ts
await TagApi.listAll();
```

### `POST /api/tag/list`

No description available.

```ts
await TagApi.list({ filters: ListFilter });
```

### `POST /api/tag/create`

No description available.

```ts
await TagApi.create({ body: Partial<T> });
```

### `POST /api/tag/{id}`

No description available.

```ts
await TagApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/tag/list.pot`

No description available.

```ts
await TagApi.listPOTUpdate();
```

### `GET /api/tag/list.pot`

No description available.

```ts
await TagApi.listPOT();
```

### `POST /api/tag/list.pot`

No description available.

```ts
await TagApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

