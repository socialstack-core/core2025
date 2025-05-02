### Using this API

```typescript
import CustomContentTypeFieldApi, { CustomContentTypeField } from 'Api/CustomContentTypeField';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={CustomContentTypeFieldApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return CustomContentTypeFieldApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={CustomContentTypeFieldApi} filter={ /* filters */ }>
    {(entity: CustomContentTypeField) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# CustomContentTypeField

*Full Type:* `Api.CustomContentTypes.CustomContentTypeField`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A custom content type field.

---



# Fields

The following fields are available on this entity:

| Name                | Type      | Nullable | Summary                  |
| ------------------- | --------- | -------- | ------------------------ |
| CustomContentTypeId | `UInt32`  | No       | No description available |
| DefaultValue        | `String`  | Yes      | No description available |
| DataType            | `String`  | Yes      | No description available |
| LinkedEntity        | `String`  | Yes      | No description available |
| Name                | `String`  | Yes      | No description available |
| NickName            | `String`  | Yes      | No description available |
| Localised           | `Boolean` | No       | No description available |
| UrlEncoded          | `Boolean` | No       | No description available |
| IsHidden            | `Boolean` | No       | No description available |
| HideSeconds         | `Boolean` | No       | No description available |
| RoundMinutes        | `Boolean` | No       | No description available |
| Validation          | `String`  | Yes      | No description available |
| Order               | `UInt32`  | No       | No description available |
| Group               | `String`  | Yes      | No description available |
| OptionsArePrices    | `Boolean` | No       | No description available |
| Deleted             | `Boolean` | No       | No description available |

# CustomContentTypeField API

This controller provides API methods for the `CustomContentTypeField` entity.

Base URL: `/api/customcontenttypefield`

---

### `GET /api/customcontenttypefield/revision/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.loadRevision();
```

### `DELETE /api/customcontenttypefield/revision/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.deleteRevision();
```

### `GET /api/customcontenttypefield/revision/list`

No description available.

```ts
await CustomContentTypeFieldApi.revisionList();
```

### `POST /api/customcontenttypefield/revision/list`

No description available.

```ts
await CustomContentTypeFieldApi.revisionList({ filters: ListFilter });
```

### `POST /api/customcontenttypefield/revision/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/customcontenttypefield/publish/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.publishRevision();
```

### `POST /api/customcontenttypefield/publish/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/customcontenttypefield/draft`

No description available.

```ts
await CustomContentTypeFieldApi.createDraft({ body: Partial<T> });
```

### `GET /api/customcontenttypefield/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.load();
```

### `DELETE /api/customcontenttypefield/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.delete();
```

### `GET /api/customcontenttypefield/cache/invalidate/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.invalidateCachedItem();
```

### `GET /api/customcontenttypefield/cache/invalidate`

No description available.

```ts
await CustomContentTypeFieldApi.invalidateCache();
```

### `GET /api/customcontenttypefield/list`

No description available.

```ts
await CustomContentTypeFieldApi.listAll();
```

### `POST /api/customcontenttypefield/list`

No description available.

```ts
await CustomContentTypeFieldApi.list({ filters: ListFilter });
```

### `POST /api/customcontenttypefield/create`

No description available.

```ts
await CustomContentTypeFieldApi.create({ body: Partial<T> });
```

### `POST /api/customcontenttypefield/{id}`

No description available.

```ts
await CustomContentTypeFieldApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/customcontenttypefield/list.pot`

No description available.

```ts
await CustomContentTypeFieldApi.listPOTUpdate();
```

### `GET /api/customcontenttypefield/list.pot`

No description available.

```ts
await CustomContentTypeFieldApi.listPOT();
```

### `POST /api/customcontenttypefield/list.pot`

No description available.

```ts
await CustomContentTypeFieldApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

