### Using this API

```typescript
import AdminNavMenuItemApi, { AdminNavMenuItem } from 'Api/AdminNavMenuItem';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={AdminNavMenuItemApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return AdminNavMenuItemApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={AdminNavMenuItemApi} filter={ /* filters */ }>
    {(entity: AdminNavMenuItem) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# AdminNavMenuItem

*Full Type:* `Api.NavMenus.AdminNavMenuItem`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A particular entry within a navigation menu.

---



# Fields

The following fields are available on this entity:

| Name               | Type     | Nullable | Summary                  |
| ------------------ | -------- | -------- | ------------------------ |
| Title              | `String` | Yes      | No description available |
| Target             | `String` | Yes      | No description available |
| IconRef            | `String` | Yes      | No description available |
| VisibilityRuleJson | `String` | Yes      | No description available |

# AdminNavMenuItem API

This controller provides API methods for the `AdminNavMenuItem` entity.

Base URL: `/api/adminnavmenuitem`

---

### `GET /api/adminnavmenuitem/revision/{id}`

No description available.

```ts
await AdminNavMenuItemApi.loadRevision();
```

### `DELETE /api/adminnavmenuitem/revision/{id}`

No description available.

```ts
await AdminNavMenuItemApi.deleteRevision();
```

### `GET /api/adminnavmenuitem/revision/list`

No description available.

```ts
await AdminNavMenuItemApi.revisionList();
```

### `POST /api/adminnavmenuitem/revision/list`

No description available.

```ts
await AdminNavMenuItemApi.revisionList({ filters: ListFilter });
```

### `POST /api/adminnavmenuitem/revision/{id}`

No description available.

```ts
await AdminNavMenuItemApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/adminnavmenuitem/publish/{id}`

No description available.

```ts
await AdminNavMenuItemApi.publishRevision();
```

### `POST /api/adminnavmenuitem/publish/{id}`

No description available.

```ts
await AdminNavMenuItemApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/adminnavmenuitem/draft`

No description available.

```ts
await AdminNavMenuItemApi.createDraft({ body: Partial<T> });
```

### `GET /api/adminnavmenuitem/{id}`

No description available.

```ts
await AdminNavMenuItemApi.load();
```

### `DELETE /api/adminnavmenuitem/{id}`

No description available.

```ts
await AdminNavMenuItemApi.delete();
```

### `GET /api/adminnavmenuitem/cache/invalidate/{id}`

No description available.

```ts
await AdminNavMenuItemApi.invalidateCachedItem();
```

### `GET /api/adminnavmenuitem/cache/invalidate`

No description available.

```ts
await AdminNavMenuItemApi.invalidateCache();
```

### `GET /api/adminnavmenuitem/list`

No description available.

```ts
await AdminNavMenuItemApi.listAll();
```

### `POST /api/adminnavmenuitem/list`

No description available.

```ts
await AdminNavMenuItemApi.list({ filters: ListFilter });
```

### `POST /api/adminnavmenuitem/create`

No description available.

```ts
await AdminNavMenuItemApi.create({ body: Partial<T> });
```

### `POST /api/adminnavmenuitem/{id}`

No description available.

```ts
await AdminNavMenuItemApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/adminnavmenuitem/list.pot`

No description available.

```ts
await AdminNavMenuItemApi.listPOTUpdate();
```

### `GET /api/adminnavmenuitem/list.pot`

No description available.

```ts
await AdminNavMenuItemApi.listPOT();
```

### `POST /api/adminnavmenuitem/list.pot`

No description available.

```ts
await AdminNavMenuItemApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

