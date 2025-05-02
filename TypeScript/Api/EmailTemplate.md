### Using this API

```typescript
import EmailTemplateApi, { EmailTemplate } from 'Api/EmailTemplate';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={EmailTemplateApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return EmailTemplateApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={EmailTemplateApi} filter={ /* filters */ }>
    {(entity: EmailTemplate) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# EmailTemplate

*Full Type:* `Api.Emails.EmailTemplate`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A particular email template.

---



# Fields

The following fields are available on this entity:

| Name      | Type     | Nullable | Summary                  |
| --------- | -------- | -------- | ------------------------ |
| Key       | `String` | Yes      | No description available |
| Name      | `String` | Yes      | No description available |
| Subject   | `String` | Yes      | No description available |
| BodyJson  | `String` | Yes      | No description available |
| Notes     | `String` | Yes      | No description available |
| SendFrom  | `String` | Yes      | No description available |
| EmailType | `Int32`  | No       | No description available |

# EmailTemplate API

This controller provides API methods for the `EmailTemplate` entity.

Base URL: `/api/emailtemplate`

---

### `GET /api/emailtemplate/revision/{id}`

No description available.

```ts
await EmailTemplateApi.loadRevision();
```

### `DELETE /api/emailtemplate/revision/{id}`

No description available.

```ts
await EmailTemplateApi.deleteRevision();
```

### `GET /api/emailtemplate/revision/list`

No description available.

```ts
await EmailTemplateApi.revisionList();
```

### `POST /api/emailtemplate/revision/list`

No description available.

```ts
await EmailTemplateApi.revisionList({ filters: ListFilter });
```

### `POST /api/emailtemplate/revision/{id}`

No description available.

```ts
await EmailTemplateApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/emailtemplate/publish/{id}`

No description available.

```ts
await EmailTemplateApi.publishRevision();
```

### `POST /api/emailtemplate/publish/{id}`

No description available.

```ts
await EmailTemplateApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/emailtemplate/draft`

No description available.

```ts
await EmailTemplateApi.createDraft({ body: Partial<T> });
```

### `GET /api/emailtemplate/{id}`

No description available.

```ts
await EmailTemplateApi.load();
```

### `DELETE /api/emailtemplate/{id}`

No description available.

```ts
await EmailTemplateApi.delete();
```

### `GET /api/emailtemplate/cache/invalidate/{id}`

No description available.

```ts
await EmailTemplateApi.invalidateCachedItem();
```

### `GET /api/emailtemplate/cache/invalidate`

No description available.

```ts
await EmailTemplateApi.invalidateCache();
```

### `GET /api/emailtemplate/list`

No description available.

```ts
await EmailTemplateApi.listAll();
```

### `POST /api/emailtemplate/list`

No description available.

```ts
await EmailTemplateApi.list({ filters: ListFilter });
```

### `POST /api/emailtemplate/create`

No description available.

```ts
await EmailTemplateApi.create({ body: Partial<T> });
```

### `POST /api/emailtemplate/{id}`

No description available.

```ts
await EmailTemplateApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/emailtemplate/list.pot`

No description available.

```ts
await EmailTemplateApi.listPOTUpdate();
```

### `GET /api/emailtemplate/list.pot`

No description available.

```ts
await EmailTemplateApi.listPOT();
```

### `POST /api/emailtemplate/list.pot`

No description available.

```ts
await EmailTemplateApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

