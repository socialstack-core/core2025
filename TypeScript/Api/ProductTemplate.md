### Using this API

```typescript
import ProductTemplateApi, { ProductTemplate } from 'Api/ProductTemplate';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductTemplateApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductTemplateApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductTemplateApi} filter={ /* filters */ }>
    {(entity: ProductTemplate) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductTemplate

*Full Type:* `Api.Payments.ProductTemplate`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ProductTemplate

---



# Fields

The following fields are available on this entity:

| Name | Type | Nullable | Summary |
| ---- | ---- | -------- | ------- |

# ProductTemplate API

This controller provides API methods for the `ProductTemplate` entity.

Base URL: `/api/producttemplate`

---

### `GET /api/producttemplate/revision/{id}`

No description available.

```ts
await ProductTemplateApi.loadRevision();
```

### `DELETE /api/producttemplate/revision/{id}`

No description available.

```ts
await ProductTemplateApi.deleteRevision();
```

### `GET /api/producttemplate/revision/list`

No description available.

```ts
await ProductTemplateApi.revisionList();
```

### `POST /api/producttemplate/revision/list`

No description available.

```ts
await ProductTemplateApi.revisionList({ filters: ListFilter });
```

### `POST /api/producttemplate/revision/{id}`

No description available.

```ts
await ProductTemplateApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/producttemplate/publish/{id}`

No description available.

```ts
await ProductTemplateApi.publishRevision();
```

### `POST /api/producttemplate/publish/{id}`

No description available.

```ts
await ProductTemplateApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/producttemplate/draft`

No description available.

```ts
await ProductTemplateApi.createDraft({ body: Partial<T> });
```

### `GET /api/producttemplate/{id}`

No description available.

```ts
await ProductTemplateApi.load();
```

### `DELETE /api/producttemplate/{id}`

No description available.

```ts
await ProductTemplateApi.delete();
```

### `GET /api/producttemplate/cache/invalidate/{id}`

No description available.

```ts
await ProductTemplateApi.invalidateCachedItem();
```

### `GET /api/producttemplate/cache/invalidate`

No description available.

```ts
await ProductTemplateApi.invalidateCache();
```

### `GET /api/producttemplate/list`

No description available.

```ts
await ProductTemplateApi.listAll();
```

### `POST /api/producttemplate/list`

No description available.

```ts
await ProductTemplateApi.list({ filters: ListFilter });
```

### `POST /api/producttemplate/create`

No description available.

```ts
await ProductTemplateApi.create({ body: Partial<T> });
```

### `POST /api/producttemplate/{id}`

No description available.

```ts
await ProductTemplateApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/producttemplate/list.pot`

No description available.

```ts
await ProductTemplateApi.listPOTUpdate();
```

### `GET /api/producttemplate/list.pot`

No description available.

```ts
await ProductTemplateApi.listPOT();
```

### `POST /api/producttemplate/list.pot`

No description available.

```ts
await ProductTemplateApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

