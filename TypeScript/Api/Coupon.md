### Using this API

```typescript
import CouponApi, { Coupon } from 'Api/Coupon';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={CouponApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return CouponApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={CouponApi} filter={ /* filters */ }>
    {(entity: Coupon) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Coupon

*Full Type:* `Api.Payments.Coupon`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Coupon

---



# Fields

The following fields are available on this entity:

| Name                  | Type        | Nullable | Summary                  |
| --------------------- | ----------- | -------- | ------------------------ |
| Token                 | `String`    | Yes      | No description available |
| MaxNumberOfPeople     | `UInt32`    | No       | No description available |
| Disabled              | `Boolean`   | No       | No description available |
| ExpiryDateUtc         | `DateTime?` | Yes      | No description available |
| SubscriptionDelayDays | `UInt32`    | No       | No description available |
| DiscountPercent       | `UInt32`    | No       | No description available |
| DiscountFixedAmount   | `UInt32`    | No       | No description available |
| FreeDelivery          | `Boolean`   | No       | No description available |
| MinimumSpendAmount    | `UInt32`    | No       | No description available |

# Coupon API

This controller provides API methods for the `Coupon` entity.

Base URL: `/api/coupon`

---

### `GET /api/coupon/revision/{id}`

No description available.

```ts
await CouponApi.loadRevision();
```

### `DELETE /api/coupon/revision/{id}`

No description available.

```ts
await CouponApi.deleteRevision();
```

### `GET /api/coupon/revision/list`

No description available.

```ts
await CouponApi.revisionList();
```

### `POST /api/coupon/revision/list`

No description available.

```ts
await CouponApi.revisionList({ filters: ListFilter });
```

### `POST /api/coupon/revision/{id}`

No description available.

```ts
await CouponApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/coupon/publish/{id}`

No description available.

```ts
await CouponApi.publishRevision();
```

### `POST /api/coupon/publish/{id}`

No description available.

```ts
await CouponApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/coupon/draft`

No description available.

```ts
await CouponApi.createDraft({ body: Partial<T> });
```

### `GET /api/coupon/{id}`

No description available.

```ts
await CouponApi.load();
```

### `DELETE /api/coupon/{id}`

No description available.

```ts
await CouponApi.delete();
```

### `GET /api/coupon/cache/invalidate/{id}`

No description available.

```ts
await CouponApi.invalidateCachedItem();
```

### `GET /api/coupon/cache/invalidate`

No description available.

```ts
await CouponApi.invalidateCache();
```

### `GET /api/coupon/list`

No description available.

```ts
await CouponApi.listAll();
```

### `POST /api/coupon/list`

No description available.

```ts
await CouponApi.list({ filters: ListFilter });
```

### `POST /api/coupon/create`

No description available.

```ts
await CouponApi.create({ body: Partial<T> });
```

### `POST /api/coupon/{id}`

No description available.

```ts
await CouponApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/coupon/list.pot`

No description available.

```ts
await CouponApi.listPOTUpdate();
```

### `GET /api/coupon/list.pot`

No description available.

```ts
await CouponApi.listPOT();
```

### `POST /api/coupon/list.pot`

No description available.

```ts
await CouponApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

