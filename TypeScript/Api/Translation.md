### Using this API

```typescript
import TranslationApi, { Translation } from 'Api/Translation';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={TranslationApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return TranslationApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={TranslationApi} filter={ /* filters */ }>
    {(entity: Translation) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Translation

*Full Type:* `Api.Translate.Translation`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Translation

---



# Fields

The following fields are available on this entity:

| Name       | Type     | Nullable | Summary                  |
| ---------- | -------- | -------- | ------------------------ |
| Module     | `String` | Yes      | No description available |
| Original   | `String` | Yes      | No description available |
| Translated | `String` | Yes      | No description available |

# Translation API

This controller provides API methods for the `Translation` entity.

Base URL: `/api/translation`

---

### `GET /api/translation/revision/{id}`

No description available.

```ts
await TranslationApi.loadRevision();
```

### `DELETE /api/translation/revision/{id}`

No description available.

```ts
await TranslationApi.deleteRevision();
```

### `GET /api/translation/revision/list`

No description available.

```ts
await TranslationApi.revisionList();
```

### `POST /api/translation/revision/list`

No description available.

```ts
await TranslationApi.revisionList({ filters: ListFilter });
```

### `POST /api/translation/revision/{id}`

No description available.

```ts
await TranslationApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/translation/publish/{id}`

No description available.

```ts
await TranslationApi.publishRevision();
```

### `POST /api/translation/publish/{id}`

No description available.

```ts
await TranslationApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/translation/draft`

No description available.

```ts
await TranslationApi.createDraft({ body: Partial<T> });
```

### `GET /api/translation/{id}`

No description available.

```ts
await TranslationApi.load();
```

### `DELETE /api/translation/{id}`

No description available.

```ts
await TranslationApi.delete();
```

### `GET /api/translation/cache/invalidate/{id}`

No description available.

```ts
await TranslationApi.invalidateCachedItem();
```

### `GET /api/translation/cache/invalidate`

No description available.

```ts
await TranslationApi.invalidateCache();
```

### `GET /api/translation/list`

No description available.

```ts
await TranslationApi.listAll();
```

### `POST /api/translation/list`

No description available.

```ts
await TranslationApi.list({ filters: ListFilter });
```

### `POST /api/translation/create`

No description available.

```ts
await TranslationApi.create({ body: Partial<T> });
```

### `POST /api/translation/{id}`

No description available.

```ts
await TranslationApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/translation/list.pot`

No description available.

```ts
await TranslationApi.listPOTUpdate();
```

### `GET /api/translation/list.pot`

No description available.

```ts
await TranslationApi.listPOT();
```

### `POST /api/translation/list.pot`

No description available.

```ts
await TranslationApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

