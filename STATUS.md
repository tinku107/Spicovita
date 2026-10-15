# SPICO Vita – Project Status

_Last updated: 15 Oct 2026 (IST). Legend: ✅ Completed · 🟡 In progress · ⬜ Pending · ⏸ Deferred_

## 1. Summary
| Area | Status |
|---|---|
| Design docs (requirements, architecture, API standards, test plan, roadmap, reporting) | ✅ Completed (v1, see `docs/`) |
| API solution scaffold (Clean Architecture, .NET 8) | ✅ Completed – builds; 2 unit tests |
| Business modules (features) | ⬜ Pending (only Catalog `Product` entity + list endpoint started) |
| Angular frontend | ⬜ Pending (placeholder only) |
| Dev SQL Server (Docker Developer edition) | ⬜ Pending (Docker not yet installed) |
| CI pipeline | ⬜ Pending |
| Hosting (Azure free tier) | ⬜ Pending – needs Tinku's decision (card required) |

## 2. Platform / cross-cutting (docs/08)
| Requirement | Status |
|---|---|
| Versioned REST API `/api/v1` (Asp.Versioning) | ✅ Wired in scaffold |
| OpenAPI / Swagger contract | ✅ Wired (Swagger UI in Development) |
| ProblemDetails errors | ✅ Wired (`AddProblemDetails`, exception handler) |
| Health checks (`/health`, SQL check) | ✅ Wired |
| Serilog structured logging | ✅ Wired (console) |
| JWT bearer authentication | 🟡 Placeholder config only – no token issuing yet |
| EF Core SQL Server, audit fields (UTC), soft delete filter, rowversion | 🟡 Base entity + DbContext done; no migrations yet |
| Money rounding (AwayFromZero), GST 5% CGST/SGST split | 🟡 Helper + unit tests done |
| Paging convention | 🟡 `PagedResult<T>` used on products list; filtering/sorting pending |
| Idempotency-Key on create/payment, ETag concurrency checks | ⬜ Pending |
| Refresh-token rotation, permission policies per company | ⬜ Pending |
| Rate limiting | ⬜ Pending |
| CQRS (MediatR 12.x behind own interfaces) + FluentValidation | ⬜ Pending |
| Integration tests (Testcontainers SQL Server) | ⬜ Pending (needs Docker) |
| Generated client for Angular / future mobile app | ⬜ Pending |

## 3. Software modules
### 3.1 Catalog — 🟡 In progress
- Products/SKUs (name, pack size, HSN 0713, GST rate, MRP, active) — 🟡 entity + `GET /api/v1/products` done
- Create/update/delete products, price lists, units, categories — ⬜
- Barcode / internal code per SKU — ⬜

### 3.2 Procurement / Vendors — ⬜ Pending
- Vendor master (GSTIN, FSSAI, contacts, terms) — ⬜
- Purchase orders, goods receipt, purchase bills with GST input — ⬜
- Vendor payments & outstanding — ⬜

### 3.3 Inventory / Warehouse — ⬜ Pending
- Multiple sites, bins, batches, expiry — ⬜
- Stock derived from movement ledger only — ⬜
- Transfers, adjustments, stock reports — ⬜

### 3.4 Production / Packing (repacking) — ⬜ Pending
- Repacking batches (bulk kg → packs), outsourced repacker jobs, wastage — ⬜
- Batch / MFG / best-before label data — ⬜

### 3.5 Sales / Channels — ⬜ Pending
- Customers (kirana, B2C, Blinkit/Zepto/Instamart), sales orders — ⬜
- GST tax invoices with sequential numbering per FY — ⬜
- Receipts, returns, credit notes — ⬜

### 3.6 Logistics / Transport — ⬜ Pending (later phase)
- Vehicles, trips, deliveries, transport cost — ⬜

### 3.7 Finance / GST — ⬜ Pending
- Expenses, journal (double entry), cash/bank — ⬜
- GST reports (GSTR-1 / 3B data) — ⬜

### 3.8 Identity / RBAC — ⬜ Pending
- ASP.NET Core Identity, users, roles, permissions, multi-company/GSTIN — ⬜

### 3.9 Audit — 🟡 In progress
- Created/updated timestamps on all entities — 🟡 done in DbContext
- Who-changed-what audit trail table — ⬜

### 3.10 Reporting / Dashboard / P&L — ⬜ Pending
- Owner dashboard; P&L by date range / month / FY — ⬜
- Sales, purchase, stock, expense, GST reports with Excel/PDF export — ⬜
- Later: P&L by product/vendor/warehouse/channel, balance sheet, transport costs, role dashboards — ⬜

## 4. Business launch
| Item | Status | Details |
|---|---|---|
| FSSAI registration | ✅ Done | Basic (Tatkal) 21226194003349, issued 12-10-2026, **Retailer only** |
| FSSAI Marketer / repacker category | ⬜ Pending | Call FoSCoS helpline 1800112100 |
| GST registration | ✅ Completed | Issued 14 Oct 2026; Regular vs Composition to confirm (Regular needed for Blinkit) |
| Virtual office | ✅ Done | 1 year |
| Udyam registration | ✅ Completed | UDYAM-KR-03-0774393, registered 15 Oct 2026 |
| Business current account | ⬜ Pending | ICICI failed earlier; Udyam now live so Axis Liberty Digital (GSTIN+Udyam+video KYC) is the strongest next try |
| Trademark | ⬜ Pending | "SPICO" conflicts with existing class-30 mark; need 2–3 fallback names |
| Barcode (GS1) | ⏸ Deferred | GS1 too costly now; cheaper alternatives under review |
| Trade licence (BBMP) | ⬜ Pending | |
| Suppliers (dal traders) | 🟡 In progress | Emailed, awaiting replies |
| Repackers | 🟡 In progress | Gayathri Associates & Semis Foods emailed, awaiting replies; others need call/WhatsApp |
| Product list | ⬜ Pending | Not finalised by Tinku |
| Pack design | 🟡 In progress | v2 front/back in `common/design` (brand name may change) |
| Quick-commerce listings | ⬜ Pending | After GST, trademark, barcode |

Business checklists and working files: see `common/` (`SPICO_Vita_Tracker.xlsx` is the master checklist).
