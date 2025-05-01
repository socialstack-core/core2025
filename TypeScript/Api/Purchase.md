### Using this API

```typescript
import PurchaseApi, { Purchase } from 'Api/Purchase';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PurchaseApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PurchaseApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PurchaseApi} filter={ /* filters */ }>
    {(entity: Purchase) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Purchase

*Full Type:* `Api.Payments.Purchase`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A purchase for a set of products which are attached as a set of ProductQuantities.

---



# Fields

The following fields are available on this entity:

| Name                     | Type      | Nullable | Summary                  |
| ------------------------ | --------- | -------- | ------------------------ |
| Status                   | `UInt32`  | No       | No description available |
| CouponId                 | `UInt32`  | No       | No description available |
| Authorise                | `Boolean` | No       | No description available |
| MultiExecute             | `Boolean` | No       | No description available |
| LocaleId                 | `UInt32`  | No       | No description available |
| PaymentGatewayInternalId | `String`  | Yes      | No description available |
| PaymentGatewayId         | `UInt32`  | No       | No description available |
| PaymentMethodId          | `UInt32`  | No       | No description available |
| CurrencyCode             | `String`  | Yes      | No description available |
| TotalCost                | `UInt64`  | No       | No description available |
| ContentAntiDuplication   | `UInt64`  | No       | No description available |
| ContentType              | `String`  | Yes      | No description available |
| ContentId                | `UInt32`  | No       | No description available |

# Purchase API

This controller provides API methods for the `Purchase` entity.

Base URL: `/api/purchase`

---

### `GET /api/purchase/revision/{id}`

No description available.

```ts
await PurchaseApi.loadRevision();
```

### `DELETE /api/purchase/revision/{id}`

No description available.

```ts
await PurchaseApi.deleteRevision();
```

### `GET /api/purchase/revision/list`

No description available.

```ts
await PurchaseApi.revisionList();
```

### `POST /api/purchase/revision/list`

No description available.

```ts
await PurchaseApi.revisionList({ filters: ListFilter });
```

### `POST /api/purchase/revision/{id}`

No description available.

```ts
await PurchaseApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/purchase/publish/{id}`

No description available.

```ts
await PurchaseApi.publishRevision();
```

### `POST /api/purchase/publish/{id}`

No description available.

```ts
await PurchaseApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/purchase/draft`

No description available.

```ts
await PurchaseApi.createDraft({ body: Partial<T> });
```

### `GET /api/purchase/{id}`

No description available.

```ts
await PurchaseApi.load();
```

### `DELETE /api/purchase/{id}`

No description available.

```ts
await PurchaseApi.delete();
```

### `GET /api/purchase/cache/invalidate/{id}`

No description available.

```ts
await PurchaseApi.invalidateCachedItem();
```

### `GET /api/purchase/cache/invalidate`

No description available.

```ts
await PurchaseApi.invalidateCache();
```

### `GET /api/purchase/list`

No description available.

```ts
await PurchaseApi.listAll();
```

### `POST /api/purchase/list`

No description available.

```ts
await PurchaseApi.list({ filters: ListFilter });
```

### `POST /api/purchase/create`

No description available.

```ts
await PurchaseApi.create({ body: Partial<T> });
```

### `POST /api/purchase/{id}`

No description available.

```ts
await PurchaseApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/purchase/list.pot`

No description available.

```ts
await PurchaseApi.listPOTUpdate();
```

### `GET /api/purchase/list.pot`

No description available.

```ts
await PurchaseApi.listPOT();
```

### `POST /api/purchase/list.pot`

No description available.

```ts
await PurchaseApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

