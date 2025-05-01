### Using this API

```typescript
import SubscriptionUsageApi, { SubscriptionUsage } from 'Api/SubscriptionUsage';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={SubscriptionUsageApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return SubscriptionUsageApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={SubscriptionUsageApi} filter={ /* filters */ }>
    {(entity: SubscriptionUsage) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# SubscriptionUsage

*Full Type:* `Api.Payments.SubscriptionUsage`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A SubscriptionUsage

---



# Fields

The following fields are available on this entity:

| Name              | Type       | Nullable | Summary                  |
| ----------------- | ---------- | -------- | ------------------------ |
| ProductId         | `UInt32`   | No       | No description available |
| SubscriptionId    | `UInt32`   | No       | No description available |
| MaximumUsageToday | `UInt32`   | No       | No description available |
| ChargedTimeslotId | `UInt32`   | No       | No description available |
| DateUtc           | `DateTime` | No       | No description available |

# SubscriptionUsage API

This controller provides API methods for the `SubscriptionUsage` entity.

Base URL: `/api/subscriptionusage`

---

### `GET /api/subscriptionusage/revision/{id}`

No description available.

```ts
await SubscriptionUsageApi.loadRevision();
```

### `DELETE /api/subscriptionusage/revision/{id}`

No description available.

```ts
await SubscriptionUsageApi.deleteRevision();
```

### `GET /api/subscriptionusage/revision/list`

No description available.

```ts
await SubscriptionUsageApi.revisionList();
```

### `POST /api/subscriptionusage/revision/list`

No description available.

```ts
await SubscriptionUsageApi.revisionList({ filters: ListFilter });
```

### `POST /api/subscriptionusage/revision/{id}`

No description available.

```ts
await SubscriptionUsageApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/subscriptionusage/publish/{id}`

No description available.

```ts
await SubscriptionUsageApi.publishRevision();
```

### `POST /api/subscriptionusage/publish/{id}`

No description available.

```ts
await SubscriptionUsageApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/subscriptionusage/draft`

No description available.

```ts
await SubscriptionUsageApi.createDraft({ body: Partial<T> });
```

### `GET /api/subscriptionusage/{id}`

No description available.

```ts
await SubscriptionUsageApi.load();
```

### `DELETE /api/subscriptionusage/{id}`

No description available.

```ts
await SubscriptionUsageApi.delete();
```

### `GET /api/subscriptionusage/cache/invalidate/{id}`

No description available.

```ts
await SubscriptionUsageApi.invalidateCachedItem();
```

### `GET /api/subscriptionusage/cache/invalidate`

No description available.

```ts
await SubscriptionUsageApi.invalidateCache();
```

### `GET /api/subscriptionusage/list`

No description available.

```ts
await SubscriptionUsageApi.listAll();
```

### `POST /api/subscriptionusage/list`

No description available.

```ts
await SubscriptionUsageApi.list({ filters: ListFilter });
```

### `POST /api/subscriptionusage/create`

No description available.

```ts
await SubscriptionUsageApi.create({ body: Partial<T> });
```

### `POST /api/subscriptionusage/{id}`

No description available.

```ts
await SubscriptionUsageApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/subscriptionusage/list.pot`

No description available.

```ts
await SubscriptionUsageApi.listPOTUpdate();
```

### `GET /api/subscriptionusage/list.pot`

No description available.

```ts
await SubscriptionUsageApi.listPOT();
```

### `POST /api/subscriptionusage/list.pot`

No description available.

```ts
await SubscriptionUsageApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

