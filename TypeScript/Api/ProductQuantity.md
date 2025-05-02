### Using this API

```typescript
import ProductQuantityApi, { ProductQuantity } from 'Api/ProductQuantity';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductQuantityApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductQuantityApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductQuantityApi} filter={ /* filters */ }>
    {(entity: ProductQuantity) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductQuantity

*Full Type:* `Api.Payments.ProductQuantity`



---

*Auto-generated from a .NET entity inside the Api/ directory*

Tracks a product and a quantity of the product.

---



# Fields

The following fields are available on this entity:

| Name           | Type     | Nullable | Summary                  |
| -------------- | -------- | -------- | ------------------------ |
| ProductId      | `UInt32` | No       | No description available |
| Quantity       | `UInt64` | No       | No description available |
| ShoppingCartId | `UInt32` | No       | No description available |
| SubscriptionId | `UInt32` | No       | No description available |
| PurchaseId     | `UInt32` | No       | No description available |

# ProductQuantity API

This controller provides API methods for the `ProductQuantity` entity.

Base URL: `/api/productquantity`

---

### `GET /api/productquantity/revision/{id}`

No description available.

```ts
await ProductQuantityApi.loadRevision();
```

### `DELETE /api/productquantity/revision/{id}`

No description available.

```ts
await ProductQuantityApi.deleteRevision();
```

### `GET /api/productquantity/revision/list`

No description available.

```ts
await ProductQuantityApi.revisionList();
```

### `POST /api/productquantity/revision/list`

No description available.

```ts
await ProductQuantityApi.revisionList({ filters: ListFilter });
```

### `POST /api/productquantity/revision/{id}`

No description available.

```ts
await ProductQuantityApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/productquantity/publish/{id}`

No description available.

```ts
await ProductQuantityApi.publishRevision();
```

### `POST /api/productquantity/publish/{id}`

No description available.

```ts
await ProductQuantityApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/productquantity/draft`

No description available.

```ts
await ProductQuantityApi.createDraft({ body: Partial<T> });
```

### `GET /api/productquantity/{id}`

No description available.

```ts
await ProductQuantityApi.load();
```

### `DELETE /api/productquantity/{id}`

No description available.

```ts
await ProductQuantityApi.delete();
```

### `GET /api/productquantity/cache/invalidate/{id}`

No description available.

```ts
await ProductQuantityApi.invalidateCachedItem();
```

### `GET /api/productquantity/cache/invalidate`

No description available.

```ts
await ProductQuantityApi.invalidateCache();
```

### `GET /api/productquantity/list`

No description available.

```ts
await ProductQuantityApi.listAll();
```

### `POST /api/productquantity/list`

No description available.

```ts
await ProductQuantityApi.list({ filters: ListFilter });
```

### `POST /api/productquantity/create`

No description available.

```ts
await ProductQuantityApi.create({ body: Partial<T> });
```

### `POST /api/productquantity/{id}`

No description available.

```ts
await ProductQuantityApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/productquantity/list.pot`

No description available.

```ts
await ProductQuantityApi.listPOTUpdate();
```

### `GET /api/productquantity/list.pot`

No description available.

```ts
await ProductQuantityApi.listPOT();
```

### `POST /api/productquantity/list.pot`

No description available.

```ts
await ProductQuantityApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

