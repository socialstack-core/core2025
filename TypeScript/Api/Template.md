### Using this API

```typescript
import TemplateApi, { Template } from 'Api/Template';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={TemplateApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return TemplateApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={TemplateApi} filter={ /* filters */ }>
    {(entity: Template) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Template

*Full Type:* `Api.Templates.Template`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A template.

---



# Fields

The following fields are available on this entity:

| Name           | Type     | Nullable | Summary                  |
| -------------- | -------- | -------- | ------------------------ |
| Key            | `String` | Yes      | No description available |
| Title          | `String` | Yes      | No description available |
| Description    | `String` | Yes      | No description available |
| TemplateParent | `UInt32` | No       | No description available |
| BaseTemplate   | `String` | Yes      | No description available |
| TemplateType   | `UInt32` | No       | No description available |
| ModuleGroups   | `String` | Yes      | No description available |
| BodyJson       | `String` | Yes      | No description available |

# Template API

This controller provides API methods for the `Template` entity.

Base URL: `/api/template`

---

### `GET /api/template/revision/{id}`

No description available.

```ts
await TemplateApi.loadRevision();
```

### `DELETE /api/template/revision/{id}`

No description available.

```ts
await TemplateApi.deleteRevision();
```

### `GET /api/template/revision/list`

No description available.

```ts
await TemplateApi.revisionList();
```

### `POST /api/template/revision/list`

No description available.

```ts
await TemplateApi.revisionList({ filters: ListFilter });
```

### `POST /api/template/revision/{id}`

No description available.

```ts
await TemplateApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/template/publish/{id}`

No description available.

```ts
await TemplateApi.publishRevision();
```

### `POST /api/template/publish/{id}`

No description available.

```ts
await TemplateApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/template/draft`

No description available.

```ts
await TemplateApi.createDraft({ body: Partial<T> });
```

### `GET /api/template/{id}`

No description available.

```ts
await TemplateApi.load();
```

### `DELETE /api/template/{id}`

No description available.

```ts
await TemplateApi.delete();
```

### `GET /api/template/cache/invalidate/{id}`

No description available.

```ts
await TemplateApi.invalidateCachedItem();
```

### `GET /api/template/cache/invalidate`

No description available.

```ts
await TemplateApi.invalidateCache();
```

### `GET /api/template/list`

No description available.

```ts
await TemplateApi.listAll();
```

### `POST /api/template/list`

No description available.

```ts
await TemplateApi.list({ filters: ListFilter });
```

### `POST /api/template/create`

No description available.

```ts
await TemplateApi.create({ body: Partial<T> });
```

### `POST /api/template/{id}`

No description available.

```ts
await TemplateApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/template/list.pot`

No description available.

```ts
await TemplateApi.listPOTUpdate();
```

### `GET /api/template/list.pot`

No description available.

```ts
await TemplateApi.listPOT();
```

### `POST /api/template/list.pot`

No description available.

```ts
await TemplateApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

