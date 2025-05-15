### Using this API

```typescript
import PageApi, { Page } from 'Api/Page';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PageApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PageApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PageApi} filter={ /* filters */ }>
    {(entity: Page) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Page

*Full Type:* `Api.Pages.Page`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A page. Pages are accessed via associated permalink(s).

---



# Fields

The following fields are available on this entity:

| Name             | Type      | Nullable | Summary                                                                                    |
| ---------------- | --------- | -------- | ------------------------------------------------------------------------------------------ |
| Title            | `String`  | Yes      | No description available                                                                   |
| Key              | `String`  | Yes      | No description available                                                                   |
| BodyJson         | `String`  | Yes      | No description available                                                                   |
| Description      | `String`  | Yes      | No description available                                                                   |
| CanIndex         | `Boolean` | No       | No description available                                                                   |
| NoFollow         | `Boolean` | No       | No description available                                                                   |
| PreferIfLoggedIn | `Boolean` | No       | No description available                                                                   |
| Url              | `String`  | Yes      | A temporarily held URL value which is used during page creation to create a new permalink. |

# Page API

This controller provides API methods for the `Page` entity.

Base URL: `/api/page`

---

### `GET /api/page/revision/{id}`

No description available.

```ts
await PageApi.loadRevision();
```

### `DELETE /api/page/revision/{id}`

No description available.

```ts
await PageApi.deleteRevision();
```

### `GET /api/page/revision/list`

No description available.

```ts
await PageApi.revisionList();
```

### `POST /api/page/revision/list`

No description available.

```ts
await PageApi.revisionList({ filters: ListFilter });
```

### `POST /api/page/revision/{id}`

No description available.

```ts
await PageApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/page/publish/{id}`

No description available.

```ts
await PageApi.publishRevision();
```

### `POST /api/page/publish/{id}`

No description available.

```ts
await PageApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/page/draft`

No description available.

```ts
await PageApi.createDraft({ body: Partial<T> });
```

### `GET /api/page/{id}`

No description available.

```ts
await PageApi.load();
```

### `DELETE /api/page/{id}`

No description available.

```ts
await PageApi.delete();
```

### `GET /api/page/cache/invalidate/{id}`

No description available.

```ts
await PageApi.invalidateCachedItem();
```

### `GET /api/page/cache/invalidate`

No description available.

```ts
await PageApi.invalidateCache();
```

### `GET /api/page/list`

No description available.

```ts
await PageApi.listAll();
```

### `POST /api/page/list`

No description available.

```ts
await PageApi.list({ filters: ListFilter });
```

### `POST /api/page/create`

No description available.

```ts
await PageApi.create({ body: Partial<T> });
```

### `POST /api/page/{id}`

No description available.

```ts
await PageApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/page/list.pot`

No description available.

```ts
await PageApi.listPOTUpdate();
```

### `GET /api/page/list.pot`

No description available.

```ts
await PageApi.listPOT();
```

### `POST /api/page/list.pot`

No description available.

```ts
await PageApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

