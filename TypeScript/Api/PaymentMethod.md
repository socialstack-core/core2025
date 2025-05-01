### Using this API

```typescript
import PaymentMethodApi, { PaymentMethod } from 'Api/PaymentMethod';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PaymentMethodApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PaymentMethodApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PaymentMethodApi} filter={ /* filters */ }>
    {(entity: PaymentMethod) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# PaymentMethod

*Full Type:* `Api.Payments.PaymentMethod`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Payment method - this is generally a saved tokenised card.

---



# Fields

The following fields are available on this entity:

| Name                 | Type       | Nullable | Summary                  |
| -------------------- | ---------- | -------- | ------------------------ |
| PaymentGatewayId     | `UInt32`   | No       | No description available |
| GatewayToken         | `String`   | Yes      | No description available |
| Issuer               | `String`   | Yes      | No description available |
| ExpiryUtc            | `DateTime` | No       | No description available |
| LastUsedUtc          | `DateTime` | No       | No description available |
| PaymentMethodTypeId  | `UInt32`   | No       | No description available |
| Name                 | `String`   | Yes      | No description available |
| OneMonthExpiryNotice | `Boolean`  | No       | No description available |

# PaymentMethod API

This controller provides API methods for the `PaymentMethod` entity.

Base URL: `/api/paymentmethod`

---

### `GET /api/paymentmethod/revision/{id}`

No description available.

```ts
await PaymentMethodApi.loadRevision();
```

### `DELETE /api/paymentmethod/revision/{id}`

No description available.

```ts
await PaymentMethodApi.deleteRevision();
```

### `GET /api/paymentmethod/revision/list`

No description available.

```ts
await PaymentMethodApi.revisionList();
```

### `POST /api/paymentmethod/revision/list`

No description available.

```ts
await PaymentMethodApi.revisionList({ filters: ListFilter });
```

### `POST /api/paymentmethod/revision/{id}`

No description available.

```ts
await PaymentMethodApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/paymentmethod/publish/{id}`

No description available.

```ts
await PaymentMethodApi.publishRevision();
```

### `POST /api/paymentmethod/publish/{id}`

No description available.

```ts
await PaymentMethodApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/paymentmethod/draft`

No description available.

```ts
await PaymentMethodApi.createDraft({ body: Partial<T> });
```

### `GET /api/paymentmethod/{id}`

No description available.

```ts
await PaymentMethodApi.load();
```

### `DELETE /api/paymentmethod/{id}`

No description available.

```ts
await PaymentMethodApi.delete();
```

### `GET /api/paymentmethod/cache/invalidate/{id}`

No description available.

```ts
await PaymentMethodApi.invalidateCachedItem();
```

### `GET /api/paymentmethod/cache/invalidate`

No description available.

```ts
await PaymentMethodApi.invalidateCache();
```

### `GET /api/paymentmethod/list`

No description available.

```ts
await PaymentMethodApi.listAll();
```

### `POST /api/paymentmethod/list`

No description available.

```ts
await PaymentMethodApi.list({ filters: ListFilter });
```

### `POST /api/paymentmethod/create`

No description available.

```ts
await PaymentMethodApi.create({ body: Partial<T> });
```

### `POST /api/paymentmethod/{id}`

No description available.

```ts
await PaymentMethodApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/paymentmethod/list.pot`

No description available.

```ts
await PaymentMethodApi.listPOTUpdate();
```

### `GET /api/paymentmethod/list.pot`

No description available.

```ts
await PaymentMethodApi.listPOT();
```

### `POST /api/paymentmethod/list.pot`

No description available.

```ts
await PaymentMethodApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

