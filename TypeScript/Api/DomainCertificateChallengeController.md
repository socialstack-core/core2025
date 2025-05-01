### Using this API

```typescript
import DomainCertificateChallengeControllerApi, { DomainCertificateChallengeController } from 'Api/DomainCertificateChallengeController';
```

# DomainCertificateChallengeController API

This controller provides API methods for the `DomainCertificateChallengeController` entity.

Base URL: `/api/.well-known/acme-challenge`

---

### `GET /api/.well-known/acme-challenge/{token}`

Handles all token requests.

```ts
await DomainCertificateChallengeControllerApi.catchAll();
```

