### Using this API

```typescript
import ConfigurationApi, { Configuration } from 'Api/Configuration';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ConfigurationApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ConfigurationApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ConfigurationApi} filter={ /* filters */ }>
    {(entity: Configuration) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Configuration

*Full Type:* `Api.Configuration.Configuration`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Configuration entry in the database. Do not derive from this! 
            Use :Config instead when declaring the config options for your thing.

---



# Fields

The following fields are available on this entity:

| Name         | Type     | Nullable | Summary                  |
| ------------ | -------- | -------- | ------------------------ |
| Name         | `String` | Yes      | No description available |
| Environments | `String` | Yes      | No description available |
| Key          | `String` | Yes      | No description available |
| ConfigJson   | `String` | Yes      | No description available |

# Configuration API

This controller provides API methods for the `Configuration` entity.

Base URL: `/api/configuration`

---

### `GET /api/configuration/revision/{id}`

No description available.

```ts
await ConfigurationApi.loadRevision();
```

### `DELETE /api/configuration/revision/{id}`

No description available.

```ts
await ConfigurationApi.deleteRevision();
```

### `GET /api/configuration/revision/list`

No description available.

```ts
await ConfigurationApi.revisionList();
```

### `POST /api/configuration/revision/list`

No description available.

```ts
await ConfigurationApi.revisionList({ filters: ListFilter });
```

### `POST /api/configuration/revision/{id}`

No description available.

```ts
await ConfigurationApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/configuration/publish/{id}`

No description available.

```ts
await ConfigurationApi.publishRevision();
```

### `POST /api/configuration/publish/{id}`

No description available.

```ts
await ConfigurationApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/configuration/draft`

No description available.

```ts
await ConfigurationApi.createDraft({ body: Partial<T> });
```

### `GET /api/configuration/{id}`

No description available.

```ts
await ConfigurationApi.load();
```

### `DELETE /api/configuration/{id}`

No description available.

```ts
await ConfigurationApi.delete();
```

### `GET /api/configuration/cache/invalidate/{id}`

No description available.

```ts
await ConfigurationApi.invalidateCachedItem();
```

### `GET /api/configuration/cache/invalidate`

No description available.

```ts
await ConfigurationApi.invalidateCache();
```

### `GET /api/configuration/list`

No description available.

```ts
await ConfigurationApi.listAll();
```

### `POST /api/configuration/list`

No description available.

```ts
await ConfigurationApi.list({ filters: ListFilter });
```

### `POST /api/configuration/create`

No description available.

```ts
await ConfigurationApi.create({ body: Partial<T> });
```

### `POST /api/configuration/{id}`

No description available.

```ts
await ConfigurationApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/configuration/list.pot`

No description available.

```ts
await ConfigurationApi.listPOTUpdate();
```

### `GET /api/configuration/list.pot`

No description available.

```ts
await ConfigurationApi.listPOT();
```

### `POST /api/configuration/list.pot`

No description available.

```ts
await ConfigurationApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

