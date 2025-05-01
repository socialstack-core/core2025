### Using this API

```typescript
import PasswordResetRequestApi, { PasswordResetRequest } from 'Api/PasswordResetRequest';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PasswordResetRequestApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PasswordResetRequestApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PasswordResetRequestApi} filter={ /* filters */ }>
    {(entity: PasswordResetRequest) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# PasswordResetRequest

*Full Type:* `Api.PasswordResetRequests.PasswordResetRequest`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A password reset request.

---



# Fields

The following fields are available on this entity:

| Name       | Type       | Nullable | Summary                  |
| ---------- | ---------- | -------- | ------------------------ |
| Token      | `String`   | Yes      | No description available |
| IsUsed     | `Boolean`  | No       | No description available |
| Email      | `String`   | Yes      | No description available |
| CreatedUtc | `DateTime` | No       | No description available |
| UserId     | `UInt32`   | No       | No description available |

# PasswordResetRequest API

This controller provides API methods for the `PasswordResetRequest` entity.

Base URL: `/api/passwordresetrequest`

---

### `GET /api/passwordresetrequest/revision/{id}`

No description available.

```ts
await PasswordResetRequestApi.loadRevision();
```

### `DELETE /api/passwordresetrequest/revision/{id}`

No description available.

```ts
await PasswordResetRequestApi.deleteRevision();
```

### `GET /api/passwordresetrequest/revision/list`

No description available.

```ts
await PasswordResetRequestApi.revisionList();
```

### `POST /api/passwordresetrequest/revision/list`

No description available.

```ts
await PasswordResetRequestApi.revisionList({ filters: ListFilter });
```

### `POST /api/passwordresetrequest/revision/{id}`

No description available.

```ts
await PasswordResetRequestApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/passwordresetrequest/publish/{id}`

No description available.

```ts
await PasswordResetRequestApi.publishRevision();
```

### `POST /api/passwordresetrequest/publish/{id}`

No description available.

```ts
await PasswordResetRequestApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/passwordresetrequest/draft`

No description available.

```ts
await PasswordResetRequestApi.createDraft({ body: Partial<T> });
```

### `GET /api/passwordresetrequest/{id}`

No description available.

```ts
await PasswordResetRequestApi.load();
```

### `DELETE /api/passwordresetrequest/{id}`

No description available.

```ts
await PasswordResetRequestApi.delete();
```

### `GET /api/passwordresetrequest/cache/invalidate/{id}`

No description available.

```ts
await PasswordResetRequestApi.invalidateCachedItem();
```

### `GET /api/passwordresetrequest/cache/invalidate`

No description available.

```ts
await PasswordResetRequestApi.invalidateCache();
```

### `GET /api/passwordresetrequest/list`

No description available.

```ts
await PasswordResetRequestApi.listAll();
```

### `POST /api/passwordresetrequest/list`

No description available.

```ts
await PasswordResetRequestApi.list({ filters: ListFilter });
```

### `POST /api/passwordresetrequest/create`

No description available.

```ts
await PasswordResetRequestApi.create({ body: Partial<T> });
```

### `POST /api/passwordresetrequest/{id}`

No description available.

```ts
await PasswordResetRequestApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/passwordresetrequest/list.pot`

No description available.

```ts
await PasswordResetRequestApi.listPOTUpdate();
```

### `GET /api/passwordresetrequest/list.pot`

No description available.

```ts
await PasswordResetRequestApi.listPOT();
```

### `POST /api/passwordresetrequest/list.pot`

No description available.

```ts
await PasswordResetRequestApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

