### Using this API

```typescript
import ShoppingCartApi, { ShoppingCart } from 'Api/ShoppingCart';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ShoppingCartApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ShoppingCartApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ShoppingCartApi} filter={ /* filters */ }>
    {(entity: ShoppingCart) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ShoppingCart

*Full Type:* `Api.Payments.ShoppingCart`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ShoppingCart contains a list of productQuantities.
            A user has a "current" shopping cart associated to them, and when they checkout, the shopping cart is converted to a payment.

---



# Fields

The following fields are available on this entity:

| Name   | Type     | Nullable | Summary                  |
| ------ | -------- | -------- | ------------------------ |
| Status | `UInt32` | No       | No description available |

# ShoppingCart API

This controller provides API methods for the `ShoppingCart` entity.

Base URL: `/api/shoppingcart`

---

### `GET /api/shoppingcart/revision/{id}`

No description available.

```ts
await ShoppingCartApi.loadRevision();
```

### `DELETE /api/shoppingcart/revision/{id}`

No description available.

```ts
await ShoppingCartApi.deleteRevision();
```

### `GET /api/shoppingcart/revision/list`

No description available.

```ts
await ShoppingCartApi.revisionList();
```

### `POST /api/shoppingcart/revision/list`

No description available.

```ts
await ShoppingCartApi.revisionList({ filters: ListFilter });
```

### `POST /api/shoppingcart/revision/{id}`

No description available.

```ts
await ShoppingCartApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/shoppingcart/publish/{id}`

No description available.

```ts
await ShoppingCartApi.publishRevision();
```

### `POST /api/shoppingcart/publish/{id}`

No description available.

```ts
await ShoppingCartApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/shoppingcart/draft`

No description available.

```ts
await ShoppingCartApi.createDraft({ body: Partial<T> });
```

### `GET /api/shoppingcart/{id}`

No description available.

```ts
await ShoppingCartApi.load();
```

### `DELETE /api/shoppingcart/{id}`

No description available.

```ts
await ShoppingCartApi.delete();
```

### `GET /api/shoppingcart/cache/invalidate/{id}`

No description available.

```ts
await ShoppingCartApi.invalidateCachedItem();
```

### `GET /api/shoppingcart/cache/invalidate`

No description available.

```ts
await ShoppingCartApi.invalidateCache();
```

### `GET /api/shoppingcart/list`

No description available.

```ts
await ShoppingCartApi.listAll();
```

### `POST /api/shoppingcart/list`

No description available.

```ts
await ShoppingCartApi.list({ filters: ListFilter });
```

### `POST /api/shoppingcart/create`

No description available.

```ts
await ShoppingCartApi.create({ body: Partial<T> });
```

### `POST /api/shoppingcart/{id}`

No description available.

```ts
await ShoppingCartApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/shoppingcart/list.pot`

No description available.

```ts
await ShoppingCartApi.listPOTUpdate();
```

### `GET /api/shoppingcart/list.pot`

No description available.

```ts
await ShoppingCartApi.listPOT();
```

### `POST /api/shoppingcart/list.pot`

No description available.

```ts
await ShoppingCartApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

