### Using this API

```typescript
import ContentFieldAccessRuleApi, { ContentFieldAccessRule } from 'Api/ContentFieldAccessRule';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ContentFieldAccessRuleApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ContentFieldAccessRuleApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ContentFieldAccessRuleApi} filter={ /* filters */ }>
    {(entity: ContentFieldAccessRule) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ContentFieldAccessRule

*Full Type:* `Api.Permissions.ContentFieldAccessRule`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ContentFieldAccessRule

---



# Fields

The following fields are available on this entity:

| Name          | Type      | Nullable | Summary                  |
| ------------- | --------- | -------- | ------------------------ |
| EntityName    | `String`  | Yes      | No description available |
| IsVirtualType | `Boolean` | No       | No description available |
| FieldName     | `String`  | Yes      | No description available |
| CanRead       | `String`  | Yes      | No description available |
| CanWrite      | `String`  | Yes      | No description available |
| RoleId        | `UInt32`  | No       | No description available |

# ContentFieldAccessRule API

This controller provides API methods for the `ContentFieldAccessRule` entity.

Base URL: `/api/contentfieldaccessrule`

---

### `GET /api/contentfieldaccessrule/revision/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.loadRevision();
```

### `DELETE /api/contentfieldaccessrule/revision/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.deleteRevision();
```

### `GET /api/contentfieldaccessrule/revision/list`

No description available.

```ts
await ContentFieldAccessRuleApi.revisionList();
```

### `POST /api/contentfieldaccessrule/revision/list`

No description available.

```ts
await ContentFieldAccessRuleApi.revisionList({ filters: ListFilter });
```

### `POST /api/contentfieldaccessrule/revision/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/contentfieldaccessrule/publish/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.publishRevision();
```

### `POST /api/contentfieldaccessrule/publish/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/contentfieldaccessrule/draft`

No description available.

```ts
await ContentFieldAccessRuleApi.createDraft({ body: Partial<T> });
```

### `GET /api/contentfieldaccessrule/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.load();
```

### `DELETE /api/contentfieldaccessrule/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.delete();
```

### `GET /api/contentfieldaccessrule/cache/invalidate/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.invalidateCachedItem();
```

### `GET /api/contentfieldaccessrule/cache/invalidate`

No description available.

```ts
await ContentFieldAccessRuleApi.invalidateCache();
```

### `GET /api/contentfieldaccessrule/list`

No description available.

```ts
await ContentFieldAccessRuleApi.listAll();
```

### `POST /api/contentfieldaccessrule/list`

No description available.

```ts
await ContentFieldAccessRuleApi.list({ filters: ListFilter });
```

### `POST /api/contentfieldaccessrule/create`

No description available.

```ts
await ContentFieldAccessRuleApi.create({ body: Partial<T> });
```

### `POST /api/contentfieldaccessrule/{id}`

No description available.

```ts
await ContentFieldAccessRuleApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/contentfieldaccessrule/list.pot`

No description available.

```ts
await ContentFieldAccessRuleApi.listPOTUpdate();
```

### `GET /api/contentfieldaccessrule/list.pot`

No description available.

```ts
await ContentFieldAccessRuleApi.listPOT();
```

### `POST /api/contentfieldaccessrule/list.pot`

No description available.

```ts
await ContentFieldAccessRuleApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

