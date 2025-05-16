### Using this API

```typescript
import ProductCategoryApi, { ProductCategory } from 'Api/ProductCategory';
```

### Usage

This entity works with some core components, such as Form & Loop

#### Form

##### Create

```tsx

<Form action={ProductCategoryApi.create}>
    {/* Add children here */}
</Form>
```

##### Update

```tsx

<Form action={(values) => { 
    return ProductCategoryApi.update(id, values); 
}}>
    {/* Add children here */}
</Form>
```

#### Loop

```tsx

<Loop over={ProductCategoryApi} filter={ /* filters */ }>
    {(entity: ProductCategory) => {
        return (
            // Loop logic here
        );
    }
</Loop>
```

# ProductCategory

*Full Type:* `Api.Payments.ProductCategory`



---

*Auto-generated from a .NET entity inside the Api/ directory*

A ProductCategory

---



# Fields

The following fields are available on this entity:

| Name        | Type      | Nullable | Summary                  |
| ----------- | --------- | -------- | ------------------------ |
| Name        | `String`  | Yes      | No description available |
| Slug        | `String`  | Yes      | No description available |
| Description | `String`  | Yes      | No description available |
| FeatureRef  | `String`  | Yes      | No description available |
| IconRef     | `String`  | Yes      | No description available |
| ParentId    | `UInt32?` | Yes      | No description available |

# ProductCategory API

This controller provides API methods for the `ProductCategory` entity.

Base URL: `/api/productcategory`

---

### `GET /api/productcategory/revision/{id}`

No description available.

```ts
await ProductCategoryApi.loadRevision();
```

### `DELETE /api/productcategory/revision/{id}`

No description available.

```ts
await ProductCategoryApi.deleteRevision();
```

### `GET /api/productcategory/revision/list`

No description available.

```ts
await ProductCategoryApi.revisionList();
```

### `POST /api/productcategory/revision/list`

No description available.

```ts
await ProductCategoryApi.revisionList({ filters: ListFilter });
```

### `POST /api/productcategory/revision/{id}`

No description available.

```ts
await ProductCategoryApi.updateRevision({ id: number, body: Partial<T> });
```

### `GET /api/productcategory/publish/{id}`

No description available.

```ts
await ProductCategoryApi.publishRevision();
```

### `POST /api/productcategory/publish/{id}`

No description available.

```ts
await ProductCategoryApi.publishRevision({ id: number, body: Partial<T> });
```

### `POST /api/productcategory/draft`

No description available.

```ts
await ProductCategoryApi.createDraft({ body: Partial<T> });
```

### `GET /api/productcategory/{id}`

No description available.

```ts
await ProductCategoryApi.load();
```

### `DELETE /api/productcategory/{id}`

No description available.

```ts
await ProductCategoryApi.delete();
```

### `GET /api/productcategory/cache/invalidate/{id}`

No description available.

```ts
await ProductCategoryApi.invalidateCachedItem();
```

### `GET /api/productcategory/cache/invalidate`

No description available.

```ts
await ProductCategoryApi.invalidateCache();
```

### `GET /api/productcategory/list`

No description available.

```ts
await ProductCategoryApi.listAll();
```

### `POST /api/productcategory/list`

No description available.

```ts
await ProductCategoryApi.list({ filters: ListFilter });
```

### `POST /api/productcategory/create`

No description available.

```ts
await ProductCategoryApi.create({ body: Partial<T> });
```

### `POST /api/productcategory/{id}`

No description available.

```ts
await ProductCategoryApi.update({ id: number, body: Partial<T> });
```

### `PUT /api/productcategory/list.pot`

No description available.

```ts
await ProductCategoryApi.listPOTUpdate();
```

### `GET /api/productcategory/list.pot`

No description available.

```ts
await ProductCategoryApi.listPOT();
```

### `POST /api/productcategory/list.pot`

No description available.

```ts
await ProductCategoryApi.listPOT({ filters: Partial<T>, includes: string, ignoreFields: string });
```

