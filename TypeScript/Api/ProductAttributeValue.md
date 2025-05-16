### Using this API

```typescript
import ProductAttributeValueApi, { ProductAttributeValue } from 'Api/ProductAttributeValue';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductAttributeValueApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductAttributeValueApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductAttributeValueApi} filter={ /* filters */ }>
    {(entity: ProductAttributeValue) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductAttributeValue

*Full Type:* `Api.Payments.ProductAttributeValue`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ProductAttributeValue

---



# Fields

The following fields are available on this entity:

| Name               | Type     | Nullable | Summary                  |
| ------------------ | -------- | -------- | ------------------------ |
| ProductAttributeId | `UInt32` | No       | No description available |
| Value              | `String` | Yes      | No description available |

# ProductAttributeValue API

This controller provides API methods for the `ProductAttributeValue` entity.

Base URL: `/api/productattributevalue`

---

### `GET /api/productattributevalue/revision/{id}`

No description available.

```ts
await ProductAttributeValueApi.loadRevision();
```

### `DELETE /api/productattributevalue/revision/{id}`

No description available.

```ts
await ProductAttributeValueApi.deleteRevision();
```

### `GET /api/productattributevalue/revision/list`

No description available.

```ts
await ProductAttributeValueApi.revisionList();
```

### `POST /api/productattributevalue/revision/list`

No description available.

```ts
await ProductAttributeValueApi.revisionList({ filters: ListFilter });
```

### `POST /api/productattributevalue/revision/{id}`

No description available.

```ts
await ProductAttributeValueApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/productattributevalue/publish/{id}`

No description available.

```ts
await ProductAttributeValueApi.publishRevision();
```

### `POST /api/productattributevalue/publish/{id}`

No description available.

```ts
await ProductAttributeValueApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/productattributevalue/draft`

No description available.

```ts
await ProductAttributeValueApi.createDraft({ body: Partial<T> });
```

### `GET /api/productattributevalue/{id}`

No description available.

```ts
await ProductAttributeValueApi.load();
```

### `DELETE /api/productattributevalue/{id}`

No description available.

```ts
await ProductAttributeValueApi.delete();
```

### `GET /api/productattributevalue/cache/invalidate/{id}`

No description available.

```ts
await ProductAttributeValueApi.invalidateCachedItem();
```

### `GET /api/productattributevalue/cache/invalidate`

No description available.

```ts
await ProductAttributeValueApi.invalidateCache();
```

### `GET /api/productattributevalue/list`

No description available.

```ts
await ProductAttributeValueApi.listAll();
```

### `POST /api/productattributevalue/list`

No description available.

```ts
await ProductAttributeValueApi.list({ filters: ListFilter });
```

### `POST /api/productattributevalue/create`

No description available.

```ts
await ProductAttributeValueApi.create({ body: Partial<T> });
```

### `POST /api/productattributevalue/{id}`

No description available.

```ts
await ProductAttributeValueApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/productattributevalue/list.pot`

No description available.

```ts
await ProductAttributeValueApi.listPOTUpdate();
```

### `GET /api/productattributevalue/list.pot`

No description available.

```ts
await ProductAttributeValueApi.listPOT();
```

### `POST /api/productattributevalue/list.pot`

No description available.

```ts
await ProductAttributeValueApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

