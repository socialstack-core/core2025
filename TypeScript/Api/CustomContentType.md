### Using this API

```typescript
import CustomContentTypeApi, { CustomContentType } from 'Api/CustomContentType';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={CustomContentTypeApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return CustomContentTypeApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={CustomContentTypeApi} filter={ /* filters */ }>
    {(entity: CustomContentType) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# CustomContentType

*Full Type:* `Api.CustomContentTypes.CustomContentType`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A custom content type. Has a list of fields.

---



# Fields

The following fields are available on this entity:

| Name     | Type      | Nullable | Summary                  |
| -------- | --------- | -------- | ------------------------ |
| Name     | `String`  | Yes      | No description available |
| NickName | `String`  | Yes      | No description available |
| Summary  | `String`  | Yes      | No description available |
| IconRef  | `String`  | Yes      | No description available |
| Deleted  | `Boolean` | No       | No description available |
| Fields   | `List`1`  | Yes      | The fields in this type  |

# CustomContentType API

This controller provides API methods for the `CustomContentType` entity.

Base URL: `/api/customcontenttype`

---

### `GET /api/customcontenttype/revision/{id}`

No description available.

```ts
await CustomContentTypeApi.loadRevision();
```

### `DELETE /api/customcontenttype/revision/{id}`

No description available.

```ts
await CustomContentTypeApi.deleteRevision();
```

### `GET /api/customcontenttype/revision/list`

No description available.

```ts
await CustomContentTypeApi.revisionList();
```

### `POST /api/customcontenttype/revision/list`

No description available.

```ts
await CustomContentTypeApi.revisionList({ filters: ListFilter });
```

### `POST /api/customcontenttype/revision/{id}`

No description available.

```ts
await CustomContentTypeApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/customcontenttype/publish/{id}`

No description available.

```ts
await CustomContentTypeApi.publishRevision();
```

### `POST /api/customcontenttype/publish/{id}`

No description available.

```ts
await CustomContentTypeApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/customcontenttype/draft`

No description available.

```ts
await CustomContentTypeApi.createDraft({ body: Partial<T> });
```

### `GET /api/customcontenttype/{id}`

No description available.

```ts
await CustomContentTypeApi.load();
```

### `DELETE /api/customcontenttype/{id}`

No description available.

```ts
await CustomContentTypeApi.delete();
```

### `GET /api/customcontenttype/cache/invalidate/{id}`

No description available.

```ts
await CustomContentTypeApi.invalidateCachedItem();
```

### `GET /api/customcontenttype/cache/invalidate`

No description available.

```ts
await CustomContentTypeApi.invalidateCache();
```

### `GET /api/customcontenttype/list`

No description available.

```ts
await CustomContentTypeApi.listAll();
```

### `POST /api/customcontenttype/list`

No description available.

```ts
await CustomContentTypeApi.list({ filters: ListFilter });
```

### `POST /api/customcontenttype/create`

No description available.

```ts
await CustomContentTypeApi.create({ body: Partial<T> });
```

### `POST /api/customcontenttype/{id}`

No description available.

```ts
await CustomContentTypeApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/customcontenttype/list.pot`

No description available.

```ts
await CustomContentTypeApi.listPOTUpdate();
```

### `GET /api/customcontenttype/list.pot`

No description available.

```ts
await CustomContentTypeApi.listPOT();
```

### `POST /api/customcontenttype/list.pot`

No description available.

```ts
await CustomContentTypeApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

