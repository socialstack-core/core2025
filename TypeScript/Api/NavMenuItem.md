### Using this API

```typescript
import NavMenuItemApi, { NavMenuItem } from 'Api/NavMenuItem';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={NavMenuItemApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return NavMenuItemApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={NavMenuItemApi} filter={ /* filters */ }>
    {(entity: NavMenuItem) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# NavMenuItem

*Full Type:* `Api.NavMenus.NavMenuItem`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A particular entry within a navigation menu.

---



# Fields

The following fields are available on this entity:

| Name         | Type      | Nullable | Summary                  |
| ------------ | --------- | -------- | ------------------------ |
| NavMenuId    | `UInt32`  | No       | No description available |
| MenuKey      | `String`  | Yes      | No description available |
| ParentItemId | `UInt32?` | Yes      | No description available |
| BodyJson     | `String`  | Yes      | No description available |
| Target       | `String`  | Yes      | No description available |
| IconRef      | `String`  | Yes      | No description available |
| Order        | `Int32`   | No       | No description available |

# NavMenuItem API

This controller provides API methods for the `NavMenuItem` entity.

Base URL: `/api/navmenuitem`

---

### `GET /api/navmenuitem/revision/{id}`

No description available.

```ts
await NavMenuItemApi.loadRevision();
```

### `DELETE /api/navmenuitem/revision/{id}`

No description available.

```ts
await NavMenuItemApi.deleteRevision();
```

### `GET /api/navmenuitem/revision/list`

No description available.

```ts
await NavMenuItemApi.revisionList();
```

### `POST /api/navmenuitem/revision/list`

No description available.

```ts
await NavMenuItemApi.revisionList({ filters: ListFilter });
```

### `POST /api/navmenuitem/revision/{id}`

No description available.

```ts
await NavMenuItemApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/navmenuitem/publish/{id}`

No description available.

```ts
await NavMenuItemApi.publishRevision();
```

### `POST /api/navmenuitem/publish/{id}`

No description available.

```ts
await NavMenuItemApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/navmenuitem/draft`

No description available.

```ts
await NavMenuItemApi.createDraft({ body: Partial<T> });
```

### `GET /api/navmenuitem/{id}`

No description available.

```ts
await NavMenuItemApi.load();
```

### `DELETE /api/navmenuitem/{id}`

No description available.

```ts
await NavMenuItemApi.delete();
```

### `GET /api/navmenuitem/cache/invalidate/{id}`

No description available.

```ts
await NavMenuItemApi.invalidateCachedItem();
```

### `GET /api/navmenuitem/cache/invalidate`

No description available.

```ts
await NavMenuItemApi.invalidateCache();
```

### `GET /api/navmenuitem/list`

No description available.

```ts
await NavMenuItemApi.listAll();
```

### `POST /api/navmenuitem/list`

No description available.

```ts
await NavMenuItemApi.list({ filters: ListFilter });
```

### `POST /api/navmenuitem/create`

No description available.

```ts
await NavMenuItemApi.create({ body: Partial<T> });
```

### `POST /api/navmenuitem/{id}`

No description available.

```ts
await NavMenuItemApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/navmenuitem/list.pot`

No description available.

```ts
await NavMenuItemApi.listPOTUpdate();
```

### `GET /api/navmenuitem/list.pot`

No description available.

```ts
await NavMenuItemApi.listPOT();
```

### `POST /api/navmenuitem/list.pot`

No description available.

```ts
await NavMenuItemApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

