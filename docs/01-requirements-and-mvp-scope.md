# SPICO Vita Business Software: Requirements and MVP Scope
Owner: Tinku (solo). Architect: Software Team · Solution Architect. Status: Pilot scope APPROVED. Rev 2: long-term (15-year) architecture, see 02 and 06.

## Goal
One mobile-first web app Tinku can use daily within 2 to 3 weeks to record purchases, repacking, sales and expenses, raise GST invoices, and see cash, stock and margin at a glance. Replaces SPICO_Vita_Tracker.xlsx.

## Users
Pilot: one Owner user, but the full multi-user, role/permission, multi-company and multi-warehouse model exists from day one (see 02).

## MVP (build now, target 2 to 3 weeks)
1. **Login**: one admin account (ASP.NET Core Identity, cookie or JWT).
2. **Masters**: Suppliers (from tracker "Suppliers" sheet), Customers with channel (Kirana, Line supplier, Quick-commerce, Retail), Products (dal type) and SKUs (500 g, 1 kg; HSN, GST 5%, MRP, reorder level).
3. **Purchases**: bulk loose dal by kg from a supplier, rate, GST, payment status. Creates a stock IN movement of bulk kg.
4. **Repacking batches**: batch no. (auto, e.g. TD-261013-01), product, bulk kg used, packs produced per SKU, packing cost, packed date, best-before (default packed date + configurable months). Bulk OUT, packed SKU IN. Wastage = bulk used minus packed kg, recorded.
5. **Sales and GST invoice**: customer, lines of SKU + batch, qty, rate. Tax 5%, split CGST 2.5% + SGST 2.5% inside Karnataka, IGST 5% outside. Sequential invoice no. per financial year (SV/2026-27/0001). Printable/shareable PDF (A4 and mobile view). Stock OUT per batch.
6. **Expenses**: date, head (transport, packing, rent, marketing...), amount, GST, payment mode.
7. **Payments**: mark a purchase or invoice paid or part paid (cash, UPI, bank). Drives cash position and receivables/payables.
8. **Dashboard**: cash and bank balance, stock by SKU with value and reorder flag, batches near best-before (30 days), this month's sales, gross margin per SKU, receivables and payables.
9. **Launch checklist**: the 21 tracker items, status, cost estimate vs actual, due date.
10. **One-time Excel import** of suppliers, SKUs, checklist and budget, plus any filled transactions.

## Business rules
- Stock is never edited by hand. It is the sum of a stock movement ledger (Purchase IN, Repack OUT/IN, Sale OUT, Adjustment with mandatory reason). On-hand per SKU per batch is derived.
- Cost per pack = (bulk cost per kg x kg per pack) + packing cost per pack, from the batch. Margin = sale taxable value minus batch cost.
- Money stored as decimal(18,2); quantities as decimal(18,3) kg or integer packs.
- GST on packaged, labelled pulses taken as 5% (HSN 0713). Rate kept on the SKU so it can change without code. To be confirmed with the Accounts agent / CA.
- Invoices cannot be edited after issue; cancel and reissue (credit note out of scope for MVP, cancel flag only).
- All dates IST.

## Later (not in MVP)
GSTR-1/3B export, e-invoicing/e-way bill, credit notes, quick-commerce payout reconciliation, barcode labels, multiple users and roles, supplier quote comparison, WhatsApp invoice sending, offline mode, a native app.

## Non-functional
- Mobile first: works on a 360 px phone screen, big tap targets; installable as a PWA (home-screen icon).
- Nightly database backup.
- HTTPS only. Cheap or free hosting (see 03-hosting-options.md).
