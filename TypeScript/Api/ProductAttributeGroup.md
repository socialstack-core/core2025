### Using this API

```typescript
import ProductAttributeGroupApi, { ProductAttributeGroup } from 'Api/ProductAttributeGroup';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductAttributeGroupApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductAttributeGroupApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductAttributeGroupApi} filter={ /* filters */ }>
    {(entity: ProductAttributeGroup) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductAttributeGroup

*Full Type:* `Api.Payments.ProductAttributeGroup`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A group of product attributes. For example, "Dimensions".
            Can be nested in a tree, and a group can be added to multiple parents.

---



# Fields

The following fields are available on this entity:

| Name | Type     | Nullable | Summary                  |
| ---- | -------- | -------- | ------------------------ |
| Name | `String` | Yes      | No description available |

# ProductAttributeGroup API

This controller provides API methods for the `ProductAttributeGroup` entity.

Base URL: `/api/productattributegroup`

---

### `GET /api/productattributegroup/revision/{id}`

No description available.

```ts
await ProductAttributeGroupApi.loadRevision();
```

### `DELETE /api/productattributegroup/revision/{id}`

No description available.

```ts
await ProductAttributeGroupApi.deleteRevision();
```

### `GET /api/productattributegroup/revision/list`

No description available.

```ts
await ProductAttributeGroupApi.revisionList();
```

### `POST /api/productattributegroup/revision/list`

No description available.

```ts
await ProductAttributeGroupApi.revisionList({ filters: ListFilter });
```

### `POST /api/productattributegroup/revision/{id}`

No description available.

```ts
await ProductAttributeGroupApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/productattributegroup/publish/{id}`

No description available.

```ts
await ProductAttributeGroupApi.publishRevision();
```

### `POST /api/productattributegroup/publish/{id}`

No description available.

```ts
await ProductAttributeGroupApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/productattributegroup/draft`

No description available.

```ts
await ProductAttributeGroupApi.createDraft({ body: Partial<T> });
```

### `GET /api/productattributegroup/{id}`

No description available.

```ts
await ProductAttributeGroupApi.load();
```

### `DELETE /api/productattributegroup/{id}`

No description available.

```ts
await ProductAttributeGroupApi.delete();
```

### `GET /api/productattributegroup/cache/invalidate/{id}`

No description available.

```ts
await ProductAttributeGroupApi.invalidateCachedItem();
```

### `GET /api/productattributegroup/cache/invalidate`

No description available.

```ts
await ProductAttributeGroupApi.invalidateCache();
```

### `GET /api/productattributegroup/list`

No description available.

```ts
await ProductAttributeGroupApi.listAll();
```

### `POST /api/productattributegroup/list`

No description available.

```ts
await ProductAttributeGroupApi.list({ filters: ListFilter });
```

### `POST /api/productattributegroup/create`

No description available.

```ts
await ProductAttributeGroupApi.create({ body: Partial<T> });
```

### `POST /api/productattributegroup/{id}`

No description available.

```ts
await ProductAttributeGroupApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/productattributegroup/list.pot`

No description available.

```ts
await ProductAttributeGroupApi.listPOTUpdate();
```

### `GET /api/productattributegroup/list.pot`

No description available.

```ts
await ProductAttributeGroupApi.listPOT();
```

### `POST /api/productattributegroup/list.pot`

No description available.

```ts
await ProductAttributeGroupApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

