### Using this API

```typescript
import StdOutControllerApi, { StdOutController } from 'Api/StdOutController';
```

# StdOutController API

This controller provides API methods for the `StdOutController` entity.

Base URL: `/api/monitoring`

---

### `GET /api/monitoring/v8/clear`

Attempts to purge V8 engines from the canvas renderer service.

```ts
await StdOutControllerApi.v8Clear();
```

### `GET /api/monitoring/certs/update`

Triggers a certificate update (admin only).

```ts
await StdOutControllerApi.updateCerts();
```

### `GET /api/monitoring/webserver/apply`

Triggers a webserver config file update (admin only).

```ts
await StdOutControllerApi.updateWebserverConfig();
```

### `GET /api/monitoring/whoami`

Indicates which server in a cluster this one is.

```ts
await StdOutControllerApi.whoAmI();
```

### `POST /api/monitoring/query`

Runs a query, returning the result set(s) as streaming JSON.
            Note that this will currently only work for MySQL database engines.

```ts
await StdOutControllerApi.runQuery({ queryBody: MonitoringQueryModel });
```

### `POST /api/monitoring/log`

Gets the latest block of text from the stdout.

```ts
await StdOutControllerApi.getLog({ filtering: LogFilteringModel });
```

### `GET /api/monitoring/helloworld`

Plaintext benchmark.

```ts
await StdOutControllerApi.plainTextBenchmark();
```

### `GET /api/monitoring/bufferpool/status`

V8 status.

```ts
await StdOutControllerApi.bufferPoolStatus();
```

### `GET /api/monitoring/bufferpool/clear`

Attempts to purge V8 engines from the canvas renderer service.

```ts
await StdOutControllerApi.bufferPoolClear();
```

### `GET /api/monitoring/gc`

Forces a GC run. Convenience for testing for memory leaks.

```ts
await StdOutControllerApi.gC();
```

### `POST /api/monitoring/exec`

Runs something on the command line. Super admin only (naturally).

```ts
await StdOutControllerApi.execute({ body: MonitoringExecModel });
```

### `GET /api/monitoring/halt`

Forces an application halt.

```ts
await StdOutControllerApi.halt();
```

### `GET /api/monitoring/clients`

Gets the latest number of websocket clients.

```ts
await StdOutControllerApi.getWsClientCount();
```

