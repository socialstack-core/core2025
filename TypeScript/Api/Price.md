### Using this API

```typescript
import PriceApi, { Price } from 'Api/Price';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={PriceApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return PriceApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={PriceApi} filter={ /* filters */ }>
    {(entity: Price) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# Price

*Full Type:* `Api.Payments.Price`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A Price

---



# Fields

The following fields are available on this entity:

| Name         | Type     | Nullable | Summary                  |
| ------------ | -------- | -------- | ------------------------ |
| Name         | `String` | Yes      | No description available |
| Amount       | `UInt32` | No       | No description available |
| CurrencyCode | `String` | Yes      | No description available |

# Price API

This controller provides API methods for the `Price` entity.

Base URL: `/api/price`

---

### `GET /api/price/revision/{id}`

No description available.

```ts
await PriceApi.loadRevision();
```

### `DELETE /api/price/revision/{id}`

No description available.

```ts
await PriceApi.deleteRevision();
```

### `GET /api/price/revision/list`

No description available.

```ts
await PriceApi.revisionList();
```

### `POST /api/price/revision/list`

No description available.

```ts
await PriceApi.revisionList({ filters: ListFilter });
```

### `POST /api/price/revision/{id}`

No description available.

```ts
await PriceApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/price/publish/{id}`

No description available.

```ts
await PriceApi.publishRevision();
```

### `POST /api/price/publish/{id}`

No description available.

```ts
await PriceApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/price/draft`

No description available.

```ts
await PriceApi.createDraft({ body: Partial<T> });
```

### `GET /api/price/{id}`

No description available.

```ts
await PriceApi.load();
```

### `DELETE /api/price/{id}`

No description available.

```ts
await PriceApi.delete();
```

### `GET /api/price/cache/invalidate/{id}`

No description available.

```ts
await PriceApi.invalidateCachedItem();
```

### `GET /api/price/cache/invalidate`

No description available.

```ts
await PriceApi.invalidateCache();
```

### `GET /api/price/list`

No description available.

```ts
await PriceApi.listAll();
```

### `POST /api/price/list`

No description available.

```ts
await PriceApi.list({ filters: ListFilter });
```

### `POST /api/price/create`

No description available.

```ts
await PriceApi.create({ body: Partial<T> });
```

### `POST /api/price/{id}`

No description available.

```ts
await PriceApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/price/list.pot`

No description available.

```ts
await PriceApi.listPOTUpdate();
```

### `GET /api/price/list.pot`

No description available.

```ts
await PriceApi.listPOT();
```

### `POST /api/price/list.pot`

No description available.

```ts
await PriceApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

