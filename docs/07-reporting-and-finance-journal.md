# Reporting/BI and Finance Journal

## 1. Double-entry journal (Finance, `finance` schema) — agreed with engineer
Every posted document creates a balanced journal in the same transaction (via `InvoiceIssued`, `BillPosted`, `ExpensePosted`, `PaymentRecorded`, `StockAdjusted` events, handled in-process):
- `finance.Account` (chart of accounts, per company; seeded Indian SME COA: Cash, Bank, Debtors, Creditors, Inventory, Sales, COGS, Packing, Freight, Expenses heads, Output CGST/SGST/IGST, Input CGST/SGST/IGST, Capital, Round off).
- `finance.JournalEntry` (Id, CompanyId, Date, FinancialYear, SourceModule, SourceDocType, SourceDocId, Narration, Status) and `finance.JournalLine` (AccountId, Debit, Credit, WarehouseId?, ChannelId?, SkuId?, VendorId?, CustomerId?, CostCentre?).
- DB check: each entry's debits = credits. Posted entries are immutable; corrections are reversing entries. Period lock per month.
- **Dimensions on journal lines** (warehouse, channel, product, vendor, customer) are stored from day one so later P&L by product/vendor/warehouse/channel needs no data migration.
- Perpetual inventory: sale posts Debtors/Sales/Output GST and COGS/Inventory at ledger cost.

## 2. Reporting schema (`reporting`, read-only)
Reports never read module tables directly; they read only the **journal** and the **stock ledger** through views:
- `reporting.vw_TrialBalance`, `vw_ProfitAndLoss` (by date range, month, FY), `vw_BalanceSheet` (Phase 5)
- `vw_SalesRegister`, `vw_PurchaseRegister`, `vw_ExpenseRegister`, `vw_GstSummary` (output vs input by rate/HSN/state; GSTR-ready later)
- `vw_StockOnHand`, `vw_StockValuation`, `vw_BatchExpiry`
- `vw_Receivables`, `vw_Payables` (ageing buckets)
- `vw_DashboardKpi`, `vw_TopSkus`, `vw_TopCustomers`
- Dimension views: `dim_Date` (table with IST dates, FY Apr–Mar), `dim_Sku`, `dim_Customer`, `dim_Vendor`, `dim_Warehouse`, `dim_Channel` — published by each module as read models.

The reporting module gets a read-only DB user; modules don't depend on it.

## 3. Scaling path
1. Phase 1: plain views + good indexes (pilot volume is tiny).
2. When slow: nightly/hourly **summary tables** (`reporting.fact_SalesDaily`, `fact_StockDaily`, `fact_JournalMonthly`) refreshed by a background job; same view names on top so the UI doesn't change.
3. Year 2+: replicate to a separate reporting database or Power BI/Fabric; star schema already defined by the dim/fact names above.

## 4. Pilot reports (Phase 1)
Owner dashboard (sales, purchases, gross margin, cash, stock value, receivables, payables, top SKUs/customers); P&L by date range, month and FY; sales, purchase, stock, expense, GST summaries. Every report exports to Excel (ClosedXML) and PDF (QuestPDF). Reports filter by CompanyId and respect RBAC (`Reporting.<Report>.View`).

## 5. QA reference check
Dashboard figures, P&L and trial balance must reconcile: trial balance totals equal; P&L gross margin = Sales − COGS from journal = sum of invoice-line margins; stock value on dashboard = inventory account balance = `vw_StockValuation`.
