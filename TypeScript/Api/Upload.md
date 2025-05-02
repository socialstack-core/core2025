### Using this API

```typescript
import UploadApi, { Upload } from 'Api/Upload';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={UploadApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return UploadApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={UploadApi} filter={ /* filters */ }>
    {(entity: Upload) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Upload

*Full Type:* `Api.Uploader.Upload`



---

*Auto-generated from a .NET entity inside the Api/ directory*

Meta for uploaded files.

---



# Fields

The following fields are available on this entity:

| Name           | Type      | Nullable | Summary                                                                                                                    |
| -------------- | --------- | -------- | -------------------------------------------------------------------------------------------------------------------------- |
| OriginalName   | `String`  | Yes      | No description available                                                                                                   |
| FileType       | `String`  | Yes      | No description available                                                                                                   |
| Variants       | `String`  | Yes      | No description available                                                                                                   |
| Blurhash       | `String`  | Yes      | No description available                                                                                                   |
| Width          | `Int32?`  | Yes      | No description available                                                                                                   |
| Height         | `Int32?`  | Yes      | No description available                                                                                                   |
| FocalX         | `Int32?`  | Yes      | No description available                                                                                                   |
| FocalY         | `Int32?`  | Yes      | No description available                                                                                                   |
| Alt            | `String`  | Yes      | No description available                                                                                                   |
| Author         | `String`  | Yes      | No description available                                                                                                   |
| UsageCount     | `Int32?`  | Yes      | No description available                                                                                                   |
| IsImage        | `Boolean` | No       | No description available                                                                                                   |
| IsPrivate      | `Boolean` | No       | No description available                                                                                                   |
| IsVideo        | `Boolean` | No       | No description available                                                                                                   |
| IsAudio        | `Boolean` | No       | No description available                                                                                                   |
| TranscodeState | `Int32`   | No       | No description available                                                                                                   |
| Subdirectory   | `String`  | Yes      | No description available                                                                                                   |
| TemporaryPath  | `String`  | Yes      | Working memory only temporary filesystem path. Can be null if something has already relocated the upload and it is "done". |
| Ref            | `String`  | Yes      | Gets a ref which may be signed.
            The HMAC is for the complete string "private:ID.FILETYPE?t=TIMESTAMP&s="      |

# Upload API

This controller provides API methods for the `Upload` entity.

Base URL: `/api/upload`

---

### `GET /api/upload/revision/{id}`

No description available.

```ts
await UploadApi.loadRevision();
```

### `DELETE /api/upload/revision/{id}`

No description available.

```ts
await UploadApi.deleteRevision();
```

### `GET /api/upload/revision/list`

No description available.

```ts
await UploadApi.revisionList();
```

### `POST /api/upload/revision/list`

No description available.

```ts
await UploadApi.revisionList({ filters: ListFilter });
```

### `POST /api/upload/revision/{id}`

No description available.

```ts
await UploadApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/upload/publish/{id}`

No description available.

```ts
await UploadApi.publishRevision();
```

### `POST /api/upload/publish/{id}`

No description available.

```ts
await UploadApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/upload/draft`

No description available.

```ts
await UploadApi.createDraft({ body: Partial<T> });
```

### `GET /api/upload/{id}`

No description available.

```ts
await UploadApi.load();
```

### `DELETE /api/upload/{id}`

No description available.

```ts
await UploadApi.delete();
```

### `GET /api/upload/cache/invalidate/{id}`

No description available.

```ts
await UploadApi.invalidateCachedItem();
```

### `GET /api/upload/cache/invalidate`

No description available.

```ts
await UploadApi.invalidateCache();
```

### `GET /api/upload/list`

No description available.

```ts
await UploadApi.listAll();
```

### `POST /api/upload/list`

No description available.

```ts
await UploadApi.list({ filters: ListFilter });
```

### `POST /api/upload/create`

No description available.

```ts
await UploadApi.create({ body: Partial<T> });
```

### `POST /api/upload/{id}`

No description available.

```ts
await UploadApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/upload/list.pot`

No description available.

```ts
await UploadApi.listPOTUpdate();
```

### `GET /api/upload/list.pot`

No description available.

```ts
await UploadApi.listPOT();
```

### `POST /api/upload/list.pot`

No description available.

```ts
await UploadApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

