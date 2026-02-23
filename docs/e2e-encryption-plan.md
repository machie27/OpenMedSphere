# E2E Encrypted Quantum-Safe Patient Data Sharing — Full Plan

## Context

Researchers need to share patient data securely on the platform. The sharing must be end-to-end encrypted using quantum-safe cryptography to meet HIPAA 2026 mandatory encryption requirements and GDPR PQC expectations. The server must never see plaintext patient data (zero-knowledge architecture).

**Key decisions made:**
- **Sharing scope**: Cross-study (any researcher to any researcher)
- **Identity**: New `Researcher` domain entity with cryptographic keypairs
- **Key storage**: User-managed (private keys stay client-side only)
- **Frontend**: Replace Blazor WASM with **React + Vite** (TypeScript)
- **Client crypto**: **Rust `pqcrypto` crate compiled to WASM** via `wasm-bindgen` (Signal-style)
- **Server crypto**: .NET 10 native `System.Security.Cryptography.MLKem`/`MLDsa` for any verification
- **Hybrid encryption**: X25519 + ML-KEM-768 (key agreement), AES-256-GCM (symmetric), ECDSA + ML-DSA-65 (signatures), HKDF-SHA256 (key derivation)

---

## Phase 1: Server-Side Domain + Application + Infrastructure + API [COMPLETED]

### Step 1.1: Domain — New Enum

**Created** `src/Core/OpenMedSphere.Domain/Enums/DataShareStatus.cs`

```csharp
public enum DataShareStatus { Pending = 0, Accepted = 1, Revoked = 2, Expired = 3 }
```

### Step 1.2: Domain — New Value Object

**Created** `src/Core/OpenMedSphere.Domain/ValueObjects/PublicKeySet.cs`

Sealed record with `Create()` factory. Keys stored as Base64 strings.

Properties: `MlKemPublicKey`, `MlDsaPublicKey`, `X25519PublicKey`, `EcdsaPublicKey`, `KeyVersion` (monotonically increasing)

### Step 1.3: Domain — New Events

**Created** 3 files in `src/Core/OpenMedSphere.Domain/Events/`:

| File | Record Parameters |
|------|------------------|
| `PatientDataSharedEvent.cs` | `DataShareId`, `SenderResearcherId`, `RecipientResearcherId`, `PatientDataId` |
| `DataShareAccessedEvent.cs` | `DataShareId`, `RecipientResearcherId` |
| `DataShareRevokedEvent.cs` | `DataShareId`, `SenderResearcherId` |

### Step 1.4: Domain — Researcher Entity

**Created** `src/Core/OpenMedSphere.Domain/Entities/Researcher.cs`

Sealed class, `AggregateRoot<Guid>`, private ctors, static `Create()`, `Guid.CreateVersion7()`.

| Property | Type | Notes |
|----------|------|-------|
| `Name` | `string` | required, set |
| `Email` | `string` | required, set |
| `Institution` | `string` | required, set |
| `PublicKeys` | `PublicKeySet` | required, set (owned type) |
| `IsActive` | `bool` | set, default true |
| `CreatedAtUtc` | `DateTime` | init |
| `UpdatedAtUtc` | `DateTime?` | set |

Methods: `Create(...)`, `RotateKeys(newPublicKeys)` (validates version strictly increasing), `UpdateProfile(...)`, `Deactivate()`, `Activate()`

### Step 1.5: Domain — DataShare Entity

**Created** `src/Core/OpenMedSphere.Domain/Entities/DataShare.cs`

Server stores encrypted blobs opaquely.

| Property | Type | Notes |
|----------|------|-------|
| `SenderResearcherId` | `Guid` | required, init |
| `RecipientResearcherId` | `Guid` | required, init |
| `PatientDataId` | `Guid` | required, init |
| `EncryptedPayload` | `string` | required, init (Base64, AES-256-GCM ciphertext) |
| `EncapsulatedKey` | `string` | required, init (Base64, hybrid KEM) |
| `Signature` | `string` | required, init (Base64, hybrid sig) |
| `SenderKeyVersion` | `int` | required, init |
| `RecipientKeyVersion` | `int` | required, init |
| `Status` | `DataShareStatus` | set, default Pending |
| `SharedAtUtc` | `DateTime` | init |
| `AccessedAtUtc` | `DateTime?` | set |
| `ExpiresAtUtc` | `DateTime?` | init |
| `CreatedAtUtc` | `DateTime` | init |
| `UpdatedAtUtc` | `DateTime?` | set |

Methods: `Create(...)` (validates sender!=recipient, expiry in future, raises `PatientDataSharedEvent`), `Accept()`, `Revoke()`, `IsExpired()`

### Step 1.6: Application — Repository Abstractions

**Created** `IResearcherRepository.cs` and `IDataShareRepository.cs` in `src/Core/OpenMedSphere.Application/Abstractions/Data/`

### Step 1.7: Application — Validation Constants

**Modified** `ValidationConstants.cs` — added `MaxNameLength`, `MaxEmailLength`, `MaxBase64KeyLength`, `MaxEncryptedPayloadLength`, `MaxEncapsulatedKeyLength`, `MaxSignatureLength`

### Step 1.8: Application — Researcher Commands + Queries

**Created** in `src/Core/OpenMedSphere.Application/Researchers/`:
- `Commands/RegisterResearcher/` — command, handler, validator
- `Commands/UpdateResearcherPublicKeys/` — command, handler, validator
- `Queries/ResearcherResponse.cs` — response DTOs (ResearcherResponse, PublicKeySetResponse, ResearcherSummaryResponse)
- `Queries/GetResearcherById/` — query, handler
- `Queries/GetResearcherPublicKeys/` — query, handler
- `Queries/SearchResearchers/` — query, handler, validator

### Step 1.9: Application — DataShare Commands + Queries

**Created** in `src/Core/OpenMedSphere.Application/DataShares/`:
- `Commands/CreateDataShare/` — command, handler, validator
- `Commands/AcceptDataShare/` — command, handler
- `Commands/RevokeDataShare/` — command, handler
- `Queries/DataShareResponse.cs` — response DTOs (DataShareResponse, DataShareSummaryResponse)
- `Queries/GetDataShareById/` — query, handler (with sender/recipient authorization)
- `Queries/GetIncomingShares/` — query, handler
- `Queries/GetOutgoingShares/` — query, handler

### Step 1.10: Infrastructure — EF Configurations

**Created** `ResearcherConfiguration.cs` (OwnsOne for PublicKeys, unique Email index) and `DataShareConfiguration.cs` (text column for EncryptedPayload, indexes on FK columns)

### Step 1.11: Infrastructure — Repositories

**Created** `ResearcherRepository.cs` (SearchAsync with ILike + EscapeLikePattern) and `DataShareRepository.cs` (ordered by SharedAtUtc descending)

### Step 1.12: Infrastructure — Modified Existing Files

- `ApplicationDbContext.cs` — added `Researchers` and `DataShares` DbSets
- `DependencyInjection.cs` — registered `IResearcherRepository` and `IDataShareRepository`
- `AuditSaveChangesInterceptor.cs` — added `Researcher` and `DataShare` to audited types

### Step 1.13: EF Migration

```bash
dotnet ef migrations add AddResearcherAndDataShare \
  --project src/Infrastructure/OpenMedSphere.Infrastructure \
  --startup-project src/Presentation/OpenMedSphere.API
```

**Status**: Not yet run (requires database connection). Run when deploying.

### Step 1.14: API Endpoints

**Created** `ResearcherEndpoints.cs`:

| Method | Route | Rate Limit |
|--------|-------|-----------|
| POST | `/api/researchers` | write |
| GET | `/api/researchers/{id:guid}` | fixed |
| GET | `/api/researchers/{id:guid}/public-keys` | fixed |
| GET | `/api/researchers/search?query=...` | fixed |
| PUT | `/api/researchers/{id:guid}/public-keys` | write |

**Created** `DataShareEndpoints.cs`:

| Method | Route | Rate Limit |
|--------|-------|-----------|
| POST | `/api/data-shares` | write |
| GET | `/api/data-shares/incoming?researcherId=...` | fixed |
| GET | `/api/data-shares/outgoing?researcherId=...` | fixed |
| GET | `/api/data-shares/{id:guid}?researcherId=...` | fixed |
| PUT | `/api/data-shares/{id:guid}/accept` | write |
| DELETE | `/api/data-shares/{id:guid}` | write |

**Modified** `Program.cs` — mapped both new endpoint groups.

### Step 1.15: Tests

**Domain Tests** (38 new tests):
- `ResearcherTests.cs` — 13 tests
- `DataShareTests.cs` — 15 tests
- `PublicKeySetTests.cs` — 10 tests

**Application Tests** (6 new tests):
- `RegisterResearcherCommandHandlerTests.cs` — 2 tests
- `CreateDataShareCommandHandlerTests.cs` — 4 tests

**Verification**: Build 0 errors, 157 tests all passing.

---

## Phase 2: React Frontend Setup (replaces Blazor WASM) [NOT YET IMPLEMENTED]

### Step 2.1: Create React Project

Create a new `frontend/` directory at repo root with Vite + React + TypeScript:

```bash
npm create vite@latest frontend -- --template react-ts
```

Configure:
- TypeScript strict mode
- ESLint + Prettier
- Path aliases (`@/` for `src/`)
- Tailwind CSS (or chosen styling)
- Vite proxy for API (`/api` -> backend URL)
- PWA via `vite-plugin-pwa` (replaces Blazor PWA support)

### Step 2.2: Core UI Structure

```
frontend/src/
├── api/          # API client (fetch wrapper, types)
├── auth/         # JWT token management
├── components/   # Shared UI components
├── crypto/       # Rust WASM crypto bindings
├── hooks/        # React hooks
├── pages/        # Route pages
├── stores/       # State management (Zustand or React context)
└── types/        # Shared TypeScript types
```

### Step 2.3: API Client

TypeScript API client auto-generated from OpenAPI spec or hand-written, with:
- JWT Bearer token injection
- Error handling for 400/401/404/429
- Types matching API DTOs

### Step 2.4: Aspire Integration

Update `src/Presentation/OpenMedSphere.AppHost/` to serve the React frontend via:
- Aspire `AddNpmApp()` for dev or
- Static file serving for production build

Remove or archive the Blazor `OpenMedSphere.Web` project.

---

## Phase 3: Rust WASM Crypto Module [NOT YET IMPLEMENTED]

### Step 3.1: Rust Crate Setup

Create `frontend/crypto/` as a Rust crate:

```
frontend/crypto/
├── Cargo.toml          # wasm-bindgen, pqcrypto, x25519-dalek, aes-gcm, hkdf, p256
├── src/
│   ├── lib.rs          # wasm_bindgen exports
│   ├── keygen.rs       # Keypair generation (ML-KEM-768, ML-DSA-65, X25519, ECDSA P-256)
│   ├── encrypt.rs      # Hybrid encrypt: X25519+ML-KEM-768 -> HKDF -> AES-256-GCM
│   ├── decrypt.rs      # Hybrid decrypt (inverse)
│   ├── sign.rs         # Hybrid sign: ECDSA + ML-DSA-65 (concatenated)
│   └── verify.rs       # Hybrid verify
└── pkg/                # wasm-pack output (gitignored, built on npm install)
```

Key Rust crates:
- `pqcrypto-kyber` (ML-KEM-768)
- `pqcrypto-dilithium` (ML-DSA-65)
- `x25519-dalek` (X25519)
- `p256` (ECDSA P-256)
- `aes-gcm` (AES-256-GCM)
- `hkdf` + `sha2` (HKDF-SHA256)
- `wasm-bindgen` (JS interop)

### Step 3.2: Build Integration

Add `wasm-pack build` to the frontend's npm scripts:
```json
"prebuild": "cd crypto && wasm-pack build --target web",
"predev": "cd crypto && wasm-pack build --target web --dev"
```

### Step 3.3: TypeScript Crypto Service

Create `frontend/src/crypto/CryptoService.ts`:

```typescript
export interface CryptoService {
  generateKeyPair(): Promise<KeyPair>;
  encrypt(plaintext: Uint8Array, recipientPublicKeys: PublicKeySet): Promise<EncryptedPackage>;
  decrypt(pkg: EncryptedPackage, privateKeys: PrivateKeySet): Promise<Uint8Array>;
  sign(data: Uint8Array, privateKeys: PrivateKeySet): Promise<Uint8Array>;
  verify(data: Uint8Array, signature: Uint8Array, publicKeys: PublicKeySet): Promise<boolean>;
}
```

This service wraps the Rust WASM functions and handles:
- Base64 encoding/decoding for API transport
- Key serialization for IndexedDB storage

### Step 3.4: Key Storage (IndexedDB)

Create `frontend/src/crypto/KeyStorage.ts`:
- Store private keys encrypted with PBKDF2-derived key from user passphrase
- Support versioned key storage (old keys kept for decrypting old shares)
- Use `idb` npm package for IndexedDB access

### Step 3.5: Data Sharing Workflow

`frontend/src/services/DataSharingService.ts`:
1. Fetch recipient public keys from `GET /api/researchers/{id}/public-keys`
2. Fetch patient data from `GET /api/patient-data/{id}`
3. Serialize to JSON, encrypt via `CryptoService.encrypt()`
4. Sign the ciphertext via `CryptoService.sign()`
5. POST to `POST /api/data-shares`
6. Receiving: fetch encrypted package, decrypt, verify signature

---

## Phase 4: Documentation Updates [PARTIALLY COMPLETED]

### Step 4.1: Update CLAUDE.md [COMPLETED]

Updated:
- Architecture section — added Researcher/DataShare entities, E2E encryption overview
- Domain Layer Structure — added new entities, events, value objects, enums
- Current Implementation Status — added E2E Encrypted Data Sharing section, updated test counts
- API Security — added zero-knowledge architecture note
- Planned section — updated with React + Rust WASM frontend plans

### Step 4.2: Update MEMORY.md [COMPLETED]

Added notes about:
- Researcher and DataShare entity patterns
- Hybrid PQC encryption decisions
- Zero-knowledge architecture
- Frontend plans (React + Rust WASM)

---

## Implementation Order

1. **Phase 1** — Server-side foundation (~40 files) **[DONE]**
2. **Phase 4.1** — CLAUDE.md updates **[DONE]**
3. **Phase 4.2** — MEMORY.md updates **[DONE]**
4. **Phase 2** — React frontend setup (separate PR)
5. **Phase 3** — Rust WASM crypto (separate PR, depends on Phase 2)

---

## Verification Checklist

### Phase 1 Verification [PASSED]
- [x] **Build**: `dotnet build OpenMedSphere.slnx` — 0 errors
- [x] **Tests**: `dotnet test OpenMedSphere.slnx` — 157 tests all passing (133 domain + 24 application)
- [ ] **EF Migration**: `dotnet ef migrations add AddResearcherAndDataShare` — run when DB available
- [ ] **API smoke test** (after running with Aspire):
  - Get dev token: `POST /api/auth/dev-token`
  - Register researcher: `POST /api/researchers` with public keys
  - Search researchers: `GET /api/researchers/search?query=...`
  - Create data share: `POST /api/data-shares` with encrypted payload
  - List incoming shares: `GET /api/data-shares/incoming?researcherId=...`
  - Accept share: `PUT /api/data-shares/{id}/accept`
  - Revoke share: `DELETE /api/data-shares/{id}`

### Phase 2 Verification (future)
- [ ] `npm run dev` starts Vite dev server
- [ ] Vite proxy routes `/api` to backend
- [ ] Aspire `AddNpmApp()` integrates frontend
- [ ] Blazor Web project archived/removed

### Phase 3 Verification (future)
- [ ] `wasm-pack build` succeeds
- [ ] `CryptoService.generateKeyPair()` returns valid keys
- [ ] Round-trip: encrypt -> decrypt recovers plaintext
- [ ] Round-trip: sign -> verify returns true
- [ ] IndexedDB stores/retrieves passphrase-protected keys
- [ ] Full sharing flow: encrypt on sender -> POST -> GET -> decrypt on recipient

---

## Files Summary

### Phase 1 (completed): ~42 files
- **Domain**: 7 new (1 enum, 1 value object, 3 events, 2 entities)
- **Application**: ~25 new (2 repo interfaces, 3 commands w/ handlers+validators, 3 commands w/ handlers, 6 queries w/ handlers, 1 query validator, 2 response DTO files)
- **Infrastructure**: 4 new + 3 modified (2 EF configs, 2 repositories, modified DbContext/DI/Audit)
- **API**: 2 new + 1 modified (2 endpoint files, modified Program.cs)
- **Tests**: 5 new (3 domain test files, 2 application test files)
- **Docs**: 2 modified (CLAUDE.md, MEMORY.md)

### Phase 2 (planned): ~15-20 files
- Vite config, TypeScript config, ESLint/Prettier configs
- API client with types
- Auth module (JWT token management)
- Core page components (shell, routing)
- Aspire AppHost update

### Phase 3 (planned): ~10-15 files
- Rust crate (Cargo.toml, lib.rs, keygen.rs, encrypt.rs, decrypt.rs, sign.rs, verify.rs)
- TypeScript wrappers (CryptoService.ts, KeyStorage.ts, DataSharingService.ts)
- Type definitions for crypto interfaces
