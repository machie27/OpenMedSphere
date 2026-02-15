# E2E Encryption — Architecture Decisions

This document captures the key architectural decisions made for the E2E encrypted quantum-safe patient data sharing feature.

---

## 1. Sharing Scope

**Decision**: Cross-study sharing (any researcher to any researcher)

**Alternatives considered**:
- Intra-study only (researchers within the same study)
- Group-based sharing (share to a research group)

**Rationale**: Cross-study sharing is the most flexible model and aligns with real-world research collaboration patterns where researchers at different institutions need to share data across study boundaries. Group-based sharing can be layered on top later.

---

## 2. Researcher Identity

**Decision**: New `Researcher` domain entity with cryptographic keypairs stored server-side (public keys only)

**Alternatives considered**:
- Extend existing user/JWT claims with crypto keys
- External identity provider (e.g., ORCID) with key binding

**Rationale**: A dedicated `Researcher` entity keeps the domain clean and allows the crypto identity to evolve independently of authentication. The entity stores only public keys — private keys never touch the server.

---

## 3. Private Key Storage

**Decision**: User-managed, client-side only (IndexedDB, passphrase-protected via PBKDF2)

**Alternatives considered**:
- Server-side key escrow (encrypted with master key)
- Hardware security module (HSM) integration
- Browser Web Crypto API key storage

**Rationale**: Zero-knowledge architecture is a core requirement. The server must never have access to private keys. IndexedDB with passphrase-based encryption gives users control while keeping keys persistent across sessions. Versioned key storage supports key rotation (old keys kept for decrypting old shares).

---

## 4. Frontend Technology

**Decision**: Replace Blazor WASM with React + Vite (TypeScript)

**Alternatives considered**:
- Keep Blazor WASM
- Angular, Vue, Svelte

**Rationale**: React + TypeScript provides the best ecosystem for integrating Rust WASM modules (via wasm-bindgen). The Blazor WASM runtime adds significant overhead and has limited interop with native WASM crypto. Vite provides fast HMR and simple wasm-pack integration. The existing Blazor frontend has minimal implementation, making migration low-cost.

---

## 5. Client-Side Cryptography

**Decision**: Rust `pqcrypto` crate compiled to WASM via `wasm-bindgen`

**Alternatives considered**:
- `@noble/post-quantum` (pure JavaScript)
- `liboqs` compiled to WASM
- Web Crypto API (no PQC support yet)

**Rationale**: Rust WASM provides near-native performance for cryptographic operations. The `pqcrypto` crate is a well-maintained Rust binding to the reference PQC implementations. `wasm-bindgen` provides ergonomic JavaScript interop. This is the Signal-style approach — crypto in a compiled language, UI in JavaScript.

---

## 6. Hybrid Encryption Scheme

**Decision**: Dual classical + post-quantum algorithms at every layer

| Layer | Classical | Post-Quantum |
|-------|-----------|-------------|
| Key Agreement | X25519 | ML-KEM-768 |
| Symmetric Encryption | AES-256-GCM | — |
| Digital Signature | ECDSA P-256 | ML-DSA-65 |
| Key Derivation | HKDF-SHA256 | — |

**Alternatives considered**:
- PQC-only (drop classical algorithms)
- ML-KEM-1024 / ML-DSA-87 (higher security levels)
- ChaCha20-Poly1305 instead of AES-256-GCM

**Rationale**: Hybrid encryption provides defense-in-depth — if either the classical or post-quantum algorithm is broken, the other still protects data. ML-KEM-768 and ML-DSA-65 (NIST security level 3) provide adequate security for medical data without the performance overhead of level 5. AES-256-GCM is chosen for broad hardware acceleration support.

---

## 7. Server-Side Architecture

**Decision**: Zero-knowledge — server stores encrypted blobs opaquely

**Key properties**:
- Server never sees plaintext patient data
- Server stores: `EncryptedPayload` (AES-256-GCM ciphertext), `EncapsulatedKey` (hybrid KEM), `Signature` (hybrid sig)
- Server can verify metadata (who shared with whom, timestamps, status) but cannot read content
- All encryption/decryption happens client-side

**Alternatives considered**:
- Server-side encryption (easier but weaker trust model)
- Proxy re-encryption (server transforms ciphertext without decrypting)

**Rationale**: Zero-knowledge architecture provides the strongest privacy guarantees, which is essential for HIPAA compliance and patient trust. The server acts as an encrypted mailbox.

---

## 8. Server-Side Crypto (Optional Verification)

**Decision**: .NET 10 native `System.Security.Cryptography.MLKem`/`MLDsa` for optional server-side verification

**Rationale**: .NET 10 ships with native ML-KEM and ML-DSA support. While the server doesn't decrypt data, it may need to verify signatures on public key updates or audit operations. Using the native .NET crypto avoids additional dependencies.

---

## 9. Data Share Lifecycle

**Decision**: Simple state machine: Pending -> Accepted/Revoked/Expired

```
Create() -> [Pending]
  |-> Accept() -> [Accepted]
  |-> Revoke() -> [Revoked]
  |-> (time passes) -> [Expired]

Accepted -> Revoke() -> [Revoked]
```

**Key rules**:
- Only the recipient can accept
- Only the sender can revoke
- Cannot accept if expired or already revoked
- Cannot revoke if already revoked
- Expiry is checked at accept time (not a background job)

**Alternatives considered**:
- More granular states (e.g., Downloaded, Viewed)
- Two-phase accept (request + confirm)

**Rationale**: Simple lifecycle keeps the domain model clean. Additional states can be added later if needed. Domain events capture all transitions for audit purposes.

---

## 10. Key Rotation

**Decision**: Monotonically increasing `KeyVersion` on `PublicKeySet`, old shares retain their key versions

**Key properties**:
- `RotateKeys()` enforces `newVersion > currentVersion`
- Each `DataShare` records `SenderKeyVersion` and `RecipientKeyVersion` at creation time
- Client must keep old private keys to decrypt shares encrypted with older key versions
- No automatic re-encryption of existing shares on key rotation

**Alternatives considered**:
- Automatic re-encryption on rotation (complex, server would need to decrypt)
- Fixed key (no rotation support)

**Rationale**: Key rotation is essential for forward secrecy. Storing the key version per share allows the client to select the correct decryption key. Not re-encrypting existing shares maintains the zero-knowledge property.

---

## 11. EF Core Mapping for PublicKeySet

**Decision**: `OwnsOne` with named columns (`PublicKeys_MlKem`, `PublicKeys_MlDsa`, etc.)

**Alternatives considered**:
- JSON column (single `jsonb` column)
- Separate `PublicKeys` table

**Rationale**: `OwnsOne` maps the value object to flat columns in the `Researchers` table, keeping queries simple and avoiding JSON parsing overhead. The key data is queried frequently (on every share creation) so flat columns with proper types are optimal.

---

## 12. EncryptedPayload Storage

**Decision**: PostgreSQL `text` column (TOAST handles large values automatically)

**Alternatives considered**:
- `bytea` column (binary)
- External blob storage (S3/Azure Blob)
- Separate `EncryptedPayloads` table

**Rationale**: Base64-encoded ciphertext in a `text` column is simple and works well with JSON API transport. PostgreSQL TOAST automatically compresses and stores large values out-of-line. The 50MB validation limit keeps payloads reasonable. External blob storage can be added later if needed for very large datasets.
