### Using this API

```typescript
import DomainCertificateApi, { DomainCertificate } from 'Api/DomainCertificate';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={DomainCertificateApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return DomainCertificateApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={DomainCertificateApi} filter={ /* filters */ }>
    {(entity: DomainCertificate) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# DomainCertificate

*Full Type:* `Api.CloudHosts.DomainCertificate`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A DomainCertificate

---



# Fields

The following fields are available on this entity:

| Name        | Type        | Nullable | Summary                  |
| ----------- | ----------- | -------- | ------------------------ |
| Domain      | `String`    | Yes      | No description available |
| Ready       | `Boolean`   | No       | No description available |
| FileKey     | `String`    | Yes      | No description available |
| ExpiryUtc   | `DateTime?` | Yes      | No description available |
| ServerId    | `UInt32`    | No       | No description available |
| OrderUrl    | `String`    | Yes      | No description available |
| Status      | `UInt32`    | No       | No description available |
| LastPingUtc | `DateTime`  | No       | No description available |

# DomainCertificate API

This controller provides API methods for the `DomainCertificate` entity.

Base URL: `/api/domaincertificate`

---

### `GET /api/domaincertificate/revision/{id}`

No description available.

```ts
await DomainCertificateApi.loadRevision();
```

### `DELETE /api/domaincertificate/revision/{id}`

No description available.

```ts
await DomainCertificateApi.deleteRevision();
```

### `GET /api/domaincertificate/revision/list`

No description available.

```ts
await DomainCertificateApi.revisionList();
```

### `POST /api/domaincertificate/revision/list`

No description available.

```ts
await DomainCertificateApi.revisionList({ filters: ListFilter });
```

### `POST /api/domaincertificate/revision/{id}`

No description available.

```ts
await DomainCertificateApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/domaincertificate/publish/{id}`

No description available.

```ts
await DomainCertificateApi.publishRevision();
```

### `POST /api/domaincertificate/publish/{id}`

No description available.

```ts
await DomainCertificateApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/domaincertificate/draft`

No description available.

```ts
await DomainCertificateApi.createDraft({ body: Partial<T> });
```

### `GET /api/domaincertificate/{id}`

No description available.

```ts
await DomainCertificateApi.load();
```

### `DELETE /api/domaincertificate/{id}`

No description available.

```ts
await DomainCertificateApi.delete();
```

### `GET /api/domaincertificate/cache/invalidate/{id}`

No description available.

```ts
await DomainCertificateApi.invalidateCachedItem();
```

### `GET /api/domaincertificate/cache/invalidate`

No description available.

```ts
await DomainCertificateApi.invalidateCache();
```

### `GET /api/domaincertificate/list`

No description available.

```ts
await DomainCertificateApi.listAll();
```

### `POST /api/domaincertificate/list`

No description available.

```ts
await DomainCertificateApi.list({ filters: ListFilter });
```

### `POST /api/domaincertificate/create`

No description available.

```ts
await DomainCertificateApi.create({ body: Partial<T> });
```

### `POST /api/domaincertificate/{id}`

No description available.

```ts
await DomainCertificateApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/domaincertificate/list.pot`

No description available.

```ts
await DomainCertificateApi.listPOTUpdate();
```

### `GET /api/domaincertificate/list.pot`

No description available.

```ts
await DomainCertificateApi.listPOT();
```

### `POST /api/domaincertificate/list.pot`

No description available.

```ts
await DomainCertificateApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

