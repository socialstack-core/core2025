### Using this API

```typescript
import CustomContentTypeSelectOptionApi, { CustomContentTypeSelectOption } from 'Api/CustomContentTypeSelectOption';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={CustomContentTypeSelectOptionApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return CustomContentTypeSelectOptionApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={CustomContentTypeSelectOptionApi} filter={ /* filters */ }>
    {(entity: CustomContentTypeSelectOption) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# CustomContentTypeSelectOption

*Full Type:* `Api.CustomContentTypes.CustomContentTypeSelectOption`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A CustomContentTypeSelectOption

---



# Fields

The following fields are available on this entity:

| Name                     | Type     | Nullable | Summary                  |
| ------------------------ | -------- | -------- | ------------------------ |
| CustomContentTypeFieldId | `UInt32` | No       | No description available |
| Value                    | `String` | Yes      | No description available |
| Order                    | `UInt32` | No       | No description available |

# CustomContentTypeSelectOption API

This controller provides API methods for the `CustomContentTypeSelectOption` entity.

Base URL: `/api/customcontenttypeselectoption`

---

### `GET /api/customcontenttypeselectoption/revision/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.loadRevision();
```

### `DELETE /api/customcontenttypeselectoption/revision/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.deleteRevision();
```

### `GET /api/customcontenttypeselectoption/revision/list`

No description available.

```ts
await CustomContentTypeSelectOptionApi.revisionList();
```

### `POST /api/customcontenttypeselectoption/revision/list`

No description available.

```ts
await CustomContentTypeSelectOptionApi.revisionList({ filters: ListFilter });
```

### `POST /api/customcontenttypeselectoption/revision/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/customcontenttypeselectoption/publish/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.publishRevision();
```

### `POST /api/customcontenttypeselectoption/publish/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/customcontenttypeselectoption/draft`

No description available.

```ts
await CustomContentTypeSelectOptionApi.createDraft({ body: Partial<T> });
```

### `GET /api/customcontenttypeselectoption/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.load();
```

### `DELETE /api/customcontenttypeselectoption/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.delete();
```

### `GET /api/customcontenttypeselectoption/cache/invalidate/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.invalidateCachedItem();
```

### `GET /api/customcontenttypeselectoption/cache/invalidate`

No description available.

```ts
await CustomContentTypeSelectOptionApi.invalidateCache();
```

### `GET /api/customcontenttypeselectoption/list`

No description available.

```ts
await CustomContentTypeSelectOptionApi.listAll();
```

### `POST /api/customcontenttypeselectoption/list`

No description available.

```ts
await CustomContentTypeSelectOptionApi.list({ filters: ListFilter });
```

### `POST /api/customcontenttypeselectoption/create`

No description available.

```ts
await CustomContentTypeSelectOptionApi.create({ body: Partial<T> });
```

### `POST /api/customcontenttypeselectoption/{id}`

No description available.

```ts
await CustomContentTypeSelectOptionApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/customcontenttypeselectoption/list.pot`

No description available.

```ts
await CustomContentTypeSelectOptionApi.listPOTUpdate();
```

### `GET /api/customcontenttypeselectoption/list.pot`

No description available.

```ts
await CustomContentTypeSelectOptionApi.listPOT();
```

### `POST /api/customcontenttypeselectoption/list.pot`

No description available.

```ts
await CustomContentTypeSelectOptionApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

