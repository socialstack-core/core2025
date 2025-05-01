### Using this API

```typescript
import SubscriptionApi, { Subscription } from 'Api/Subscription';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={SubscriptionApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return SubscriptionApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={SubscriptionApi} filter={ /* filters */ }>
    {(entity: Subscription) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Subscription

*Full Type:* `Api.Payments.Subscription`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Subscription.

---



# Fields

The following fields are available on this entity:

| Name              | Type       | Nullable | Summary                  |
| ----------------- | ---------- | -------- | ------------------------ |
| LastChargeUtc     | `DateTime` | No       | No description available |
| NextChargeUtc     | `DateTime` | No       | No description available |
| WillCancel        | `Boolean`  | No       | No description available |
| TimeslotFrequency | `UInt32`   | No       | No description available |
| Status            | `UInt32`   | No       | No description available |
| PaymentMethodId   | `UInt32`   | No       | No description available |
| LocaleId          | `UInt32`   | No       | No description available |

# Subscription API

This controller provides API methods for the `Subscription` entity.

Base URL: `/api/subscription`

---

### `GET /api/subscription/revision/{id}`

No description available.

```ts
await SubscriptionApi.loadRevision();
```

### `DELETE /api/subscription/revision/{id}`

No description available.

```ts
await SubscriptionApi.deleteRevision();
```

### `GET /api/subscription/revision/list`

No description available.

```ts
await SubscriptionApi.revisionList();
```

### `POST /api/subscription/revision/list`

No description available.

```ts
await SubscriptionApi.revisionList({ filters: ListFilter });
```

### `POST /api/subscription/revision/{id}`

No description available.

```ts
await SubscriptionApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/subscription/publish/{id}`

No description available.

```ts
await SubscriptionApi.publishRevision();
```

### `POST /api/subscription/publish/{id}`

No description available.

```ts
await SubscriptionApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/subscription/draft`

No description available.

```ts
await SubscriptionApi.createDraft({ body: Partial<T> });
```

### `GET /api/subscription/{id}`

No description available.

```ts
await SubscriptionApi.load();
```

### `DELETE /api/subscription/{id}`

No description available.

```ts
await SubscriptionApi.delete();
```

### `GET /api/subscription/cache/invalidate/{id}`

No description available.

```ts
await SubscriptionApi.invalidateCachedItem();
```

### `GET /api/subscription/cache/invalidate`

No description available.

```ts
await SubscriptionApi.invalidateCache();
```

### `GET /api/subscription/list`

No description available.

```ts
await SubscriptionApi.listAll();
```

### `POST /api/subscription/list`

No description available.

```ts
await SubscriptionApi.list({ filters: ListFilter });
```

### `POST /api/subscription/create`

No description available.

```ts
await SubscriptionApi.create({ body: Partial<T> });
```

### `POST /api/subscription/{id}`

No description available.

```ts
await SubscriptionApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/subscription/list.pot`

No description available.

```ts
await SubscriptionApi.listPOTUpdate();
```

### `GET /api/subscription/list.pot`

No description available.

```ts
await SubscriptionApi.listPOT();
```

### `POST /api/subscription/list.pot`

No description available.

```ts
await SubscriptionApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

