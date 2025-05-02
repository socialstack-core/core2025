### Using this API

```typescript
import LocaleApi, { Locale } from 'Api/Locale';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={LocaleApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return LocaleApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={LocaleApi} filter={ /* filters */ }>
    {(entity: Locale) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Locale

*Full Type:* `Api.Translate.Locale`



# Fields

The following fields are available on this entity:

| Name              | Type      | Nullable | Summary                                                                                |
| ----------------- | --------- | -------- | -------------------------------------------------------------------------------------- |
| CurrencyCode      | `String`  | Yes      | No description available                                                               |
| Name              | `String`  | Yes      | No description available                                                               |
| Code              | `String`  | Yes      | No description available                                                               |
| FlagIconRef       | `String`  | Yes      | No description available                                                               |
| Aliases           | `String`  | Yes      | No description available                                                               |
| isDisabled        | `Boolean` | No       | No description available                                                               |
| isRedirected      | `Boolean` | No       | No description available                                                               |
| PermanentRedirect | `Boolean` | No       | No description available                                                               |
| RightToLeft       | `Boolean` | No       | No description available                                                               |
| PagePath          | `String`  | Yes      | No description available                                                               |
| Domains           | `String`  | Yes      | No description available                                                               |
| ShortCode         | `String`  | Yes      | If the code is e.g. en-GB, this is just en. It is internally cached for speed as well. |

# Locale API

This controller provides API methods for the `Locale` entity.

Base URL: `/api/locale`

---

### `GET /api/locale/revision/{id}`

No description available.

```ts
await LocaleApi.loadRevision();
```

### `DELETE /api/locale/revision/{id}`

No description available.

```ts
await LocaleApi.deleteRevision();
```

### `GET /api/locale/revision/list`

No description available.

```ts
await LocaleApi.revisionList();
```

### `POST /api/locale/revision/list`

No description available.

```ts
await LocaleApi.revisionList({ filters: ListFilter });
```

### `POST /api/locale/revision/{id}`

No description available.

```ts
await LocaleApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/locale/publish/{id}`

No description available.

```ts
await LocaleApi.publishRevision();
```

### `POST /api/locale/publish/{id}`

No description available.

```ts
await LocaleApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/locale/draft`

No description available.

```ts
await LocaleApi.createDraft({ body: Partial<T> });
```

### `GET /api/locale/{id}`

No description available.

```ts
await LocaleApi.load();
```

### `DELETE /api/locale/{id}`

No description available.

```ts
await LocaleApi.delete();
```

### `GET /api/locale/cache/invalidate/{id}`

No description available.

```ts
await LocaleApi.invalidateCachedItem();
```

### `GET /api/locale/cache/invalidate`

No description available.

```ts
await LocaleApi.invalidateCache();
```

### `GET /api/locale/list`

No description available.

```ts
await LocaleApi.listAll();
```

### `POST /api/locale/list`

No description available.

```ts
await LocaleApi.list({ filters: ListFilter });
```

### `POST /api/locale/create`

No description available.

```ts
await LocaleApi.create({ body: Partial<T> });
```

### `POST /api/locale/{id}`

No description available.

```ts
await LocaleApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/locale/list.pot`

No description available.

```ts
await LocaleApi.listPOTUpdate();
```

### `GET /api/locale/list.pot`

No description available.

```ts
await LocaleApi.listPOT();
```

### `POST /api/locale/list.pot`

No description available.

```ts
await LocaleApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

