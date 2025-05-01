### Using this API

```typescript
import NavMenuApi, { NavMenu } from 'Api/NavMenu';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={NavMenuApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return NavMenuApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={NavMenuApi} filter={ /* filters */ }>
    {(entity: NavMenu) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# NavMenu

*Full Type:* `Api.NavMenus.NavMenu`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A particular nav menu.

---



# Fields

The following fields are available on this entity:

| Name   | Type     | Nullable | Summary                  |
| ------ | -------- | -------- | ------------------------ |
| Key    | `String` | Yes      | No description available |
| Name   | `String` | Yes      | No description available |
| Target | `String` | Yes      | No description available |
| Order  | `Int32`  | No       | No description available |

# NavMenu API

This controller provides API methods for the `NavMenu` entity.

Base URL: `/api/navmenu`

---

### `GET /api/navmenu/revision/{id}`

No description available.

```ts
await NavMenuApi.loadRevision();
```

### `DELETE /api/navmenu/revision/{id}`

No description available.

```ts
await NavMenuApi.deleteRevision();
```

### `GET /api/navmenu/revision/list`

No description available.

```ts
await NavMenuApi.revisionList();
```

### `POST /api/navmenu/revision/list`

No description available.

```ts
await NavMenuApi.revisionList({ filters: ListFilter });
```

### `POST /api/navmenu/revision/{id}`

No description available.

```ts
await NavMenuApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/navmenu/publish/{id}`

No description available.

```ts
await NavMenuApi.publishRevision();
```

### `POST /api/navmenu/publish/{id}`

No description available.

```ts
await NavMenuApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/navmenu/draft`

No description available.

```ts
await NavMenuApi.createDraft({ body: Partial<T> });
```

### `GET /api/navmenu/{id}`

No description available.

```ts
await NavMenuApi.load();
```

### `DELETE /api/navmenu/{id}`

No description available.

```ts
await NavMenuApi.delete();
```

### `GET /api/navmenu/cache/invalidate/{id}`

No description available.

```ts
await NavMenuApi.invalidateCachedItem();
```

### `GET /api/navmenu/cache/invalidate`

No description available.

```ts
await NavMenuApi.invalidateCache();
```

### `GET /api/navmenu/list`

No description available.

```ts
await NavMenuApi.listAll();
```

### `POST /api/navmenu/list`

No description available.

```ts
await NavMenuApi.list({ filters: ListFilter });
```

### `POST /api/navmenu/create`

No description available.

```ts
await NavMenuApi.create({ body: Partial<T> });
```

### `POST /api/navmenu/{id}`

No description available.

```ts
await NavMenuApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/navmenu/list.pot`

No description available.

```ts
await NavMenuApi.listPOTUpdate();
```

### `GET /api/navmenu/list.pot`

No description available.

```ts
await NavMenuApi.listPOT();
```

### `POST /api/navmenu/list.pot`

No description available.

```ts
await NavMenuApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

