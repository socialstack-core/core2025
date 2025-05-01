### Using this API

```typescript
import CategoryApi, { Category } from 'Api/Category';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={CategoryApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return CategoryApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={CategoryApi} filter={ /* filters */ }>
    {(entity: Category) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Category

*Full Type:* `Api.Categories.Category`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A category.
            These are the primary taxonomy mechanism; any site content can be grouped up in multiple categories.

---



# Fields

The following fields are available on this entity:

| Name        | Type     | Nullable | Summary                  |
| ----------- | -------- | -------- | ------------------------ |
| Name        | `String` | Yes      | No description available |
| Description | `String` | Yes      | No description available |
| FeatureRef  | `String` | Yes      | No description available |
| IconRef     | `String` | Yes      | No description available |

# Category API

This controller provides API methods for the `Category` entity.

Base URL: `/api/category`

---

### `GET /api/category/revision/{id}`

No description available.

```ts
await CategoryApi.loadRevision();
```

### `DELETE /api/category/revision/{id}`

No description available.

```ts
await CategoryApi.deleteRevision();
```

### `GET /api/category/revision/list`

No description available.

```ts
await CategoryApi.revisionList();
```

### `POST /api/category/revision/list`

No description available.

```ts
await CategoryApi.revisionList({ filters: ListFilter });
```

### `POST /api/category/revision/{id}`

No description available.

```ts
await CategoryApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/category/publish/{id}`

No description available.

```ts
await CategoryApi.publishRevision();
```

### `POST /api/category/publish/{id}`

No description available.

```ts
await CategoryApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/category/draft`

No description available.

```ts
await CategoryApi.createDraft({ body: Partial<T> });
```

### `GET /api/category/{id}`

No description available.

```ts
await CategoryApi.load();
```

### `DELETE /api/category/{id}`

No description available.

```ts
await CategoryApi.delete();
```

### `GET /api/category/cache/invalidate/{id}`

No description available.

```ts
await CategoryApi.invalidateCachedItem();
```

### `GET /api/category/cache/invalidate`

No description available.

```ts
await CategoryApi.invalidateCache();
```

### `GET /api/category/list`

No description available.

```ts
await CategoryApi.listAll();
```

### `POST /api/category/list`

No description available.

```ts
await CategoryApi.list({ filters: ListFilter });
```

### `POST /api/category/create`

No description available.

```ts
await CategoryApi.create({ body: Partial<T> });
```

### `POST /api/category/{id}`

No description available.

```ts
await CategoryApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/category/list.pot`

No description available.

```ts
await CategoryApi.listPOTUpdate();
```

### `GET /api/category/list.pot`

No description available.

```ts
await CategoryApi.listPOT();
```

### `POST /api/category/list.pot`

No description available.

```ts
await CategoryApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

