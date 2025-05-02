### Using this API

```typescript
import UserApi, { User } from 'Api/User';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={UserApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return UserApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={UserApi} filter={ /* filters */ }>
    {(entity: User) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# User

*Full Type:* `Api.Users.User`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A particular user account.

---



# Fields

The following fields are available on this entity:

| Name               | Type        | Nullable | Summary                                                                                                                                                 |
| ------------------ | ----------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Email              | `String`    | Yes      | No description available                                                                                                                                |
| EmailOptOutFlags   | `UInt32`    | No       | No description available                                                                                                                                |
| EmailVerifyToken   | `String`    | Yes      | No description available                                                                                                                                |
| PasswordHash       | `String`    | Yes      | No description available                                                                                                                                |
| LoginAttempts      | `Int32`     | No       | No description available                                                                                                                                |
| FailedLoginTimeUtc | `DateTime?` | Yes      | No description available                                                                                                                                |
| FirstName          | `String`    | Yes      | No description available                                                                                                                                |
| LastName           | `String`    | Yes      | No description available                                                                                                                                |
| FullName           | `String`    | Yes      | No description available                                                                                                                                |
| LastVisitedUtc     | `DateTime`  | No       | No description available                                                                                                                                |
| LoginRevokeCount   | `UInt32`    | No       | No description available                                                                                                                                |
| Role               | `UInt32`    | No       | No description available                                                                                                                                |
| PrivateVerify      | `Int64`     | No       | No description available                                                                                                                                |
| FeatureRef         | `String`    | Yes      | No description available                                                                                                                                |
| AvatarRef          | `String`    | Yes      | No description available                                                                                                                                |
| Username           | `String`    | Yes      | No description available                                                                                                                                |
| LocaleId           | `UInt32?`   | Yes      | No description available                                                                                                                                |
| PasswordReset      | `String`    | Yes      | The token provided in the password reset email. Useful for checking whether
            a user update event was triggered by a password reset request. |
| JoinedUtc          | `DateTime`  | No       | The UTC date this user was created.                                                                                                                     |

# User API

This controller provides API methods for the `User` entity.

Base URL: `/api/user`

---

### `GET /api/user/revision/{id}`

No description available.

```ts
await UserApi.loadRevision();
```

### `DELETE /api/user/revision/{id}`

No description available.

```ts
await UserApi.deleteRevision();
```

### `GET /api/user/revision/list`

No description available.

```ts
await UserApi.revisionList();
```

### `POST /api/user/revision/list`

No description available.

```ts
await UserApi.revisionList({ filters: ListFilter });
```

### `POST /api/user/revision/{id}`

No description available.

```ts
await UserApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/user/publish/{id}`

No description available.

```ts
await UserApi.publishRevision();
```

### `POST /api/user/publish/{id}`

No description available.

```ts
await UserApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/user/draft`

No description available.

```ts
await UserApi.createDraft({ body: Partial<T> });
```

### `GET /api/user/{id}`

No description available.

```ts
await UserApi.load();
```

### `DELETE /api/user/{id}`

No description available.

```ts
await UserApi.delete();
```

### `GET /api/user/cache/invalidate/{id}`

No description available.

```ts
await UserApi.invalidateCachedItem();
```

### `GET /api/user/cache/invalidate`

No description available.

```ts
await UserApi.invalidateCache();
```

### `GET /api/user/list`

No description available.

```ts
await UserApi.listAll();
```

### `POST /api/user/list`

No description available.

```ts
await UserApi.list({ filters: ListFilter });
```

### `POST /api/user/create`

No description available.

```ts
await UserApi.create({ body: Partial<T> });
```

### `POST /api/user/{id}`

No description available.

```ts
await UserApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/user/list.pot`

No description available.

```ts
await UserApi.listPOTUpdate();
```

### `GET /api/user/list.pot`

No description available.

```ts
await UserApi.listPOT();
```

### `POST /api/user/list.pot`

No description available.

```ts
await UserApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

