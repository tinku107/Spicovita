# Architecture and Data Model (rev 2: 15-year modular monolith)

## 1. Style
**Modular monolith**: one deployable ASP.NET Core (.NET 8 LTS, upgrade per LTS) host, one Angular app, **one SQL Server database with one schema per module**. Modules can later be split into services because:
- each module owns its schema and its own EF Core DbContext + migrations history table (`<schema>.__EFMigrationsHistory`);
- **no cross-schema foreign keys or joins**; other modules' data is referenced by Id only and read via that module's public contract (`I<Module>Api`) or a read-model copy;
- modules talk in-process through **contracts** (sync queries) and **integration events** (async), published via an **outbox table** per module (so moving to a broker such as RabbitMQ/Azure Service Bus later is a config change);
- architecture tests (NetArchTest) fail the build if a module references another module's internals.

### Solution layout
```
<repo root>/
  application/
    api/        .NET solution: src/Host/SpicoVita.Api, src/BuildingBlocks/*, src/Modules/<Module>/*, tests (unit, integration, architecture)
    frontend/   Angular app, one lazy-loaded feature area per module
  docs/         architecture, requirements, test plan
  tests/        QA test plans, API/E2E suites
```

## 2. Modules and schemas
| Module | Schema | Owns | Pilot? |
|---|---|---|---|
| Identity/RBAC | `iam` | Company, User, Role, Permission, UserCompanyRole, UserWarehouseScope | Yes (1 admin, but full model) |
| Catalog | `catalog` | Product, Sku, PackSize, Uom, HsnCode, TaxRate (effective-dated), PriceList | Yes |
| Procurement/Vendors | `procurement` | Vendor, VendorProduct (many vendors per product, quoted rate), PurchaseOrder, GoodsReceipt, PurchaseBill | Yes (bill + receipt only) |
| Inventory/Warehouse | `inventory` | Warehouse, Location/Bin, Batch, **StockLedger**, StockTransfer, StockAdjustment, StockCount | Yes (1 warehouse, default bin) |
| Production/Packing | `production` | PackingUnit (own or outsourced), PackingOrder, PackingBatchOutput, PackagingMaterial, Wastage | Yes (repack batches) |
| Sales/Channels | `sales` | Customer, Channel, SalesOrder, SalesInvoice, InvoiceLine, Return | Yes (invoice direct) |
| Logistics/Transport | `logistics` | Vehicle, Driver, Transporter, Route, Trip, Dispatch, DeliveryProof, FreightCharge | No (Phase 3) |
| Finance/GST | `finance` | Chart of accounts, double-entry JournalEntry/Line (see 07), Payment, Expense, GstReturnPeriod, NumberSeries | Yes (payments, expenses, numbering) |
| Audit | `audit` | AuditLog (who, when, entity, before/after JSON), LoginLog | Yes |
| Reporting/BI | `reporting` | Read-only views + later summary tables over journal and stock ledger (see 07) | Yes |
| Launch/Checklist | `ops` | ChecklistItem | Yes (small, can retire later) |

## 3. Cross-cutting rules (every table, day one)
- `Id` = `uniqueidentifier` (sequential GUID, generated in app) so records can move between databases/services later; human numbers (InvoiceNo, BatchNo) are separate unique columns.
- `CompanyId` on every business table (multi-company/multi-GSTIN); a global EF query filter enforces tenant scope. Company carries GSTIN, state code, FSSAI, legal name.
- Audit columns: `CreatedAt, CreatedBy, ModifiedAt, ModifiedBy` (UTC `datetime2`, shown in IST) + `RowVersion` (optimistic concurrency).
- Soft delete: `IsDeleted, DeletedAt, DeletedBy` with a global query filter. **Posted documents (invoices, bills, ledger rows) are never deleted or edited; they are cancelled/reversed** by new entries.
- Audit log written by an EF SaveChanges interceptor (before/after JSON) to `audit.AuditLog`.
- Money `decimal(18,2)`, quantities `decimal(18,3)`, rounding AwayFromZero.
- Tax rates are effective-dated rows, never hard-coded.
- Number series per company + document type + financial year + (optional) warehouse, allocated in a serialisable transaction.

## 4. Inventory core (the heart; get it right once)
- **StockLedger** (append-only): `Id, CompanyId, WarehouseId, LocationId, SkuId or ProductId (bulk), BatchId, Uom, QtyIn, QtyOut, UnitCost, MovementType (Receipt, PackIssue, PackOutput, Sale, TransferOut, TransferIn, Return, Adjustment, Wastage), SourceModule, SourceDocType, SourceDocId, PostedAt, Reason`.
- On-hand = SUM(QtyIn − QtyOut) by warehouse, location, item, batch. Maintained as a `StockBalance` projection table updated in the same transaction (for speed), always rebuildable from the ledger; nightly check compares the two.
- No negative stock per location+batch (checked under lock).
- Costing: weighted average per item per warehouse for Phase 1, batch cost kept for margin; FIFO possible later from the same ledger.
- Pilot runs with one Warehouse ("Main") and one default Location; no code changes needed to add more.

## 5. Identity/RBAC
ASP.NET Core Identity + JWT for the Angular app. Permission-based (`Sales.Invoice.Create`), roles are bundles of permissions, assigned per company and optionally scoped to warehouses. Pilot seeds Owner role with all permissions. External SSO (Entra ID/Google) can be added later.

## 6. Integration events (examples)
`GoodsReceived` (Procurement → Inventory), `PackingCompleted` (Production → Inventory), `InvoiceIssued` (Sales → Inventory, Finance), `DispatchCreated`/`Delivered` (Logistics → Sales). Phase 1 handles these in-process via outbox + background dispatcher.

## 7. Engineer confirmation
Confirmed: one database, schema per module, DbContext per module, contracts + in-process events, CompanyId/audit/soft-delete on every table, WarehouseId + LocationId on every stock movement. Rebase the spreadsheet import onto this (import becomes a Host-level tool that calls each module's contract).
