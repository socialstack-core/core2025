### Using this API

```typescript
import ProductApi, { Product } from 'Api/Product';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductApi} filter={ /* filters */ }>
    {(entity: Product) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Product

*Full Type:* `Api.Payments.Product`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Product

---



# Fields

The following fields are available on this entity:

| Name             | Type      | Nullable | Summary                  |
| ---------------- | --------- | -------- | ------------------------ |
| Name             | `String`  | Yes      | No description available |
| IsBilledByUsage  | `Boolean` | No       | No description available |
| BillingFrequency | `UInt32`  | No       | No description available |
| MinQuantity      | `UInt64`  | No       | No description available |
| DescriptionJson  | `String`  | Yes      | No description available |
| FeatureRef       | `String`  | Yes      | No description available |
| PriceStrategy    | `UInt32`  | No       | No description available |
| PriceId          | `UInt32`  | No       | No description available |
| Stock            | `UInt32?` | Yes      | No description available |
| VariantOfId      | `UInt32`  | No       | No description available |
| TierOfId         | `UInt32`  | No       | No description available |
| AttributesJson   | `String`  | Yes      | No description available |

# Product API

This controller provides API methods for the `Product` entity.

Base URL: `/api/product`

---

### `GET /api/product/revision/{id}`

No description available.

```ts
await ProductApi.loadRevision();
```

### `DELETE /api/product/revision/{id}`

No description available.

```ts
await ProductApi.deleteRevision();
```

### `GET /api/product/revision/list`

No description available.

```ts
await ProductApi.revisionList();
```

### `POST /api/product/revision/list`

No description available.

```ts
await ProductApi.revisionList({ filters: ListFilter });
```

### `POST /api/product/revision/{id}`

No description available.

```ts
await ProductApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/product/publish/{id}`

No description available.

```ts
await ProductApi.publishRevision();
```

### `POST /api/product/publish/{id}`

No description available.

```ts
await ProductApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/product/draft`

No description available.

```ts
await ProductApi.createDraft({ body: Partial<T> });
```

### `GET /api/product/{id}`

No description available.

```ts
await ProductApi.load();
```

### `DELETE /api/product/{id}`

No description available.

```ts
await ProductApi.delete();
```

### `GET /api/product/cache/invalidate/{id}`

No description available.

```ts
await ProductApi.invalidateCachedItem();
```

### `GET /api/product/cache/invalidate`

No description available.

```ts
await ProductApi.invalidateCache();
```

### `GET /api/product/list`

No description available.

```ts
await ProductApi.listAll();
```

### `POST /api/product/list`

No description available.

```ts
await ProductApi.list({ filters: ListFilter });
```

### `POST /api/product/create`

No description available.

```ts
await ProductApi.create({ body: Partial<T> });
```

### `POST /api/product/{id}`

No description available.

```ts
await ProductApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/product/list.pot`

No description available.

```ts
await ProductApi.listPOTUpdate();
```

### `GET /api/product/list.pot`

No description available.

```ts
await ProductApi.listPOT();
```

### `POST /api/product/list.pot`

No description available.

```ts
await ProductApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

