# API and Engineering Standards (enterprise grade; API is the product)

## API contract
- REST, URL-versioned `/api/v1/...` (Asp.Versioning). Breaking changes create v2; v1 kept for at least 12 months for old mobile builds.
- OpenAPI/Swagger is the published contract; generated TypeScript client for Angular (and later mobile) from it. Contract diff checked in CI.
- Resource naming: plural kebab-case (`/repack-batches`), JSON camelCase, dates ISO 8601 UTC (`Z`), money as decimal number + `currency: "INR"`.
- DTOs per use case (`CreateInvoiceRequest`, `InvoiceDto`); never expose entities.
- Errors: RFC 7807 ProblemDetails with `type`, `title`, `status`, `traceId`, and `errors` (field → messages) for validation (400), 401/403, 404, 409 conflict (rowversion/idempotency), 422 business rule (e.g. insufficient stock), 429.
- Lists: `?page=1&pageSize=25` (max 100), `?sort=-invoiceDate,invoiceNo`, `?filter[status]=Issued&filter[date][gte]=2026-10-01`, `?q=` search. Response envelope `{ items, page, pageSize, totalCount }`. Cursor paging allowed for ledger/sync endpoints.
- Concurrency: `ETag` = rowversion; updates require `If-Match`, else 428; mismatch 409/412.
- Idempotency: `Idempotency-Key` header required on POST create and all payment endpoints; stored in `iam.IdempotencyRecord` (key, user, request hash, response, 24 h+ retention). Same key + same body returns the stored response; different body returns 409.
- State changes as commands: `POST /invoices/{id}/cancel`, `/purchase-bills/{id}/post`.

## Security
- ASP.NET Core Identity; JWT access token (15 min) + rotating refresh token (stored hashed, 30 days, revoked on reuse), per device. Lockout, password policy, 2FA ready.
- Claims: userId, companyId (active company; switch via endpoint), permissions. Policy-based `[HasPermission("Sales.Invoice.Create")]`. Tenant filter on every query.
- HTTPS/HSTS, CORS allow-list, rate limiting (per user + per IP; stricter on /auth), secrets via environment/Key Vault, never in git.

## Code structure (per module)
- Domain (entities, value objects, domain events, no EF), Application (CQRS commands/queries, FluentValidation validators, pipeline behaviours: validation, logging, transaction, idempotency, authorisation), Infrastructure (EF Core DbContext per schema, repositories, outbox), Api (minimal API endpoints or controllers mapping DTOs).
- **Mediator: MediatR 12.x (last MIT-licensed) behind our own `ICommandBus`/`IQueryBus` interfaces** — agreed with engineer, so the library can be swapped (or upgraded with the free community licence, a Tinku decision) without touching modules.
- Reads use dedicated read models/views (Dapper or EF no-tracking); reports only via `reporting` schema (doc 07).

## Operations
- Serilog structured JSON logs with correlation/trace id, enriched with user and company; no PII or tokens logged.
- OpenTelemetry-ready; health checks `/health/live`, `/health/ready` (DB, outbox lag).
- All timestamps stored UTC `datetime2`; business dates (invoice date) as `date` in IST; UI shows IST.
- Audit trail and soft delete as doc 02.

## Testing and CI
- Unit tests (domain, GST, rounding), architecture tests (NetArchTest + schema-ownership test), integration tests with **Testcontainers SQL Server** (fresh DB per run), API contract tests.
- SQL Server for dev/test runs in Docker (`mcr.microsoft.com/mssql/server`, Developer edition, free); Testcontainers uses the same. Native SQL Server for Linux is not supported on Debian 13 and LocalDB is Windows-only, so Docker is the only path on our build computer. Pure unit tests must never need a database.
- CI (GitHub Actions or Azure DevOps, free tiers): restore, build, format check, tests with coverage, OpenAPI diff, Angular lint/test/build, container image build. No deploy step until Tinku approves hosting.
