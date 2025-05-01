### Using this API

```typescript
import RoleApi, { Role } from 'Api/Role';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={RoleApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return RoleApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={RoleApi} filter={ /* filters */ }>
    {(entity: Role) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Role

*Full Type:* `Api.Permissions.Role`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A role which defines a set of capabilities to a user who is granted this particular role.

---



# Fields

The following fields are available on this entity:

| Name               | Type      | Nullable | Summary                                                                                                          |
| ------------------ | --------- | -------- | ---------------------------------------------------------------------------------------------------------------- |
| Name               | `String`  | Yes      | No description available                                                                                         |
| Key                | `String`  | Yes      | No description available                                                                                         |
| CanViewAdmin       | `Boolean` | No       | No description available                                                                                         |
| IsComposite        | `Boolean` | No       | No description available                                                                                         |
| AdminDashboardJson | `String`  | Yes      | No description available                                                                                         |
| GrantRuleJson      | `String`  | Yes      | No description available                                                                                         |
| InheritedRoleId    | `UInt32`  | No       | No description available                                                                                         |
| GrantRules         | `List`1`  | Yes      | The raw grant rules, sorted by priority (weakest first). Evaluated against only when new capabilities are added. |

# Role API

This controller provides API methods for the `Role` entity.

Base URL: `/api/role`

---

### `GET /api/role/revision/{id}`

No description available.

```ts
await RoleApi.loadRevision();
```

### `DELETE /api/role/revision/{id}`

No description available.

```ts
await RoleApi.deleteRevision();
```

### `GET /api/role/revision/list`

No description available.

```ts
await RoleApi.revisionList();
```

### `POST /api/role/revision/list`

No description available.

```ts
await RoleApi.revisionList({ filters: ListFilter });
```

### `POST /api/role/revision/{id}`

No description available.

```ts
await RoleApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/role/publish/{id}`

No description available.

```ts
await RoleApi.publishRevision();
```

### `POST /api/role/publish/{id}`

No description available.

```ts
await RoleApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/role/draft`

No description available.

```ts
await RoleApi.createDraft({ body: Partial<T> });
```

### `GET /api/role/{id}`

No description available.

```ts
await RoleApi.load();
```

### `DELETE /api/role/{id}`

No description available.

```ts
await RoleApi.delete();
```

### `GET /api/role/cache/invalidate/{id}`

No description available.

```ts
await RoleApi.invalidateCachedItem();
```

### `GET /api/role/cache/invalidate`

No description available.

```ts
await RoleApi.invalidateCache();
```

### `GET /api/role/list`

No description available.

```ts
await RoleApi.listAll();
```

### `POST /api/role/list`

No description available.

```ts
await RoleApi.list({ filters: ListFilter });
```

### `POST /api/role/create`

No description available.

```ts
await RoleApi.create({ body: Partial<T> });
```

### `POST /api/role/{id}`

No description available.

```ts
await RoleApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/role/list.pot`

No description available.

```ts
await RoleApi.listPOTUpdate();
```

### `GET /api/role/list.pot`

No description available.

```ts
await RoleApi.listPOT();
```

### `POST /api/role/list.pot`

No description available.

```ts
await RoleApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

