### Using this API

```typescript
import ProductAttributeApi, { ProductAttribute } from 'Api/ProductAttribute';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductAttributeApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductAttributeApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductAttributeApi} filter={ /* filters */ }>
    {(entity: ProductAttribute) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductAttribute

*Full Type:* `Api.Payments.ProductAttribute`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ProductAttribute

---



# Fields

The following fields are available on this entity:

| Name                 | Type     | Nullable | Summary                  |
| -------------------- | -------- | -------- | ------------------------ |
| Name                 | `String` | Yes      | No description available |
| ProductAttributeType | `Int32`  | No       | No description available |

# ProductAttribute API

This controller provides API methods for the `ProductAttribute` entity.

Base URL: `/api/productattribute`

---

### `GET /api/productattribute/revision/{id}`

No description available.

```ts
await ProductAttributeApi.loadRevision();
```

### `DELETE /api/productattribute/revision/{id}`

No description available.

```ts
await ProductAttributeApi.deleteRevision();
```

### `GET /api/productattribute/revision/list`

No description available.

```ts
await ProductAttributeApi.revisionList();
```

### `POST /api/productattribute/revision/list`

No description available.

```ts
await ProductAttributeApi.revisionList({ filters: ListFilter });
```

### `POST /api/productattribute/revision/{id}`

No description available.

```ts
await ProductAttributeApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/productattribute/publish/{id}`

No description available.

```ts
await ProductAttributeApi.publishRevision();
```

### `POST /api/productattribute/publish/{id}`

No description available.

```ts
await ProductAttributeApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/productattribute/draft`

No description available.

```ts
await ProductAttributeApi.createDraft({ body: Partial<T> });
```

### `GET /api/productattribute/{id}`

No description available.

```ts
await ProductAttributeApi.load();
```

### `DELETE /api/productattribute/{id}`

No description available.

```ts
await ProductAttributeApi.delete();
```

### `GET /api/productattribute/cache/invalidate/{id}`

No description available.

```ts
await ProductAttributeApi.invalidateCachedItem();
```

### `GET /api/productattribute/cache/invalidate`

No description available.

```ts
await ProductAttributeApi.invalidateCache();
```

### `GET /api/productattribute/list`

No description available.

```ts
await ProductAttributeApi.listAll();
```

### `POST /api/productattribute/list`

No description available.

```ts
await ProductAttributeApi.list({ filters: ListFilter });
```

### `POST /api/productattribute/create`

No description available.

```ts
await ProductAttributeApi.create({ body: Partial<T> });
```

### `POST /api/productattribute/{id}`

No description available.

```ts
await ProductAttributeApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/productattribute/list.pot`

No description available.

```ts
await ProductAttributeApi.listPOTUpdate();
```

### `GET /api/productattribute/list.pot`

No description available.

```ts
await ProductAttributeApi.listPOT();
```

### `POST /api/productattribute/list.pot`

No description available.

```ts
await ProductAttributeApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

