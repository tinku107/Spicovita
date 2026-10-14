# SPICO Vita MVP Test Plan
Owner: Software Team · QA Tester. Based on docs 01–04. Run locally until hosting is approved.

## Approach
- Unit tests (xUnit) for money, tax, rounding, batch numbers, best-before, stock ledger.
- API integration tests against a throwaway SQL Server database (EF migrations applied fresh).
- Manual mobile checks at 360 px (Chrome device mode plus one real Android phone).
- Each bug is logged with the steps to reproduce it, what was expected, what actually happened and a severity: P1 means money, stock or tax is wrong, P2 means a flow is blocked, P3 is cosmetic.

## P1: Money and tax (must pass before Tinku uses it)
| ID | Case | Expected |
|---|---|---|
| TAX-01 | Sample 1 (doc 04) intra-state | Taxable 5,057.10; CGST 126.43; SGST 126.43; round off +0.04; total 5,310.00 |
| TAX-02 | Sample 2 inter-state | IGST 114.29 (not 114.28); round off +0.01; total 2,400.00 |
| TAX-03 | Customer state 29 vs other | CGST+SGST only vs IGST only, never both |
| TAX-04 | Change SKU GST rate setting | New invoices use new rate; issued invoices unchanged |
| TAX-05 | Round off negative (e.g. subtotal x.60) | Rounds up; x.40 rounds down; x.50 rounds up |
| TAX-06 | PDF totals | PDF figures identical to the screen and the database |
| INV-01 | Invoice numbers | SV/2026-27/0001 sequential, no gaps or duplicates under two quick saves |
| INV-02 | FY rollover (1-Apr-2027) | Restarts at SV/2027-28/0001 |
| INV-03 | Edit issued invoice | Blocked; cancel flag only; cancelled stock returns via ledger |
| MAR-01 | Cost per pack | (bulk ₹/kg x pack kg) + packing cost per pack; margin = taxable − batch cost |

## P1: Stock ledger
| ID | Case | Expected |
|---|---|---|
| STK-01 | Purchase 100 kg | Bulk +100 kg IN movement |
| STK-02 | Repack 50 kg into 40x1 kg + 18x500 g | Bulk −50; SKUs +40/+18; wastage 1 kg recorded |
| STK-03 | Repack output exceeds bulk used | Rejected |
| STK-04 | Sell more than batch on-hand | Rejected; stock never negative |
| STK-05 | No direct stock edit endpoint/UI | Only Adjustment with mandatory reason |
| STK-06 | Dashboard stock = sum of ledger | Matches after mixed purchase/repack/sale/cancel |

## Batches and dates
- BAT-01: Batch number auto-generated, e.g. TD-261013-01; a second batch on the same day gets -02.
- BAT-02: Packed 13-Oct-2026 plus 6 months gives a best-before of 13-Apr-2027.
- BAT-03: Packed 31-Aug-2026 plus 6 months gives 28-Feb-2027 (end of month clamp). Also check 31-Aug-2027, which should give 29-Feb-2028 because 2028 is a leap year.
- BAT-04: A batch within 30 days of its best-before date shows on the dashboard alert.
- BAT-05: All dates are in IST. An entry at 11:55 PM IST stays on the same day.

## Other modules (P2)
- Purchases, expenses and payments, both full and part payment. Receivables and payables must be correct after a part payment, and the cash plus bank balance must match the payments.
- Masters: required fields are enforced, a duplicate SKU is rejected, and the HSN code (0713) is stored.
- Checklist: all 21 items are imported, and estimated cost vs actual cost totals are correct.
- Login: wrong password is rejected, every API returns 401 without a login, and logout works.
- Money is stored with 2 decimal places and quantities with 3. Typing 1.2345 kg is rejected or rounded consistently.

## Spreadsheet import
- IMP-01: Import SPICO_Vita_Tracker.xlsx. Row counts per sheet must match the source.
- IMP-02: Run the import again. No duplicates should be created, or it should be blocked.
- IMP-03: A blank or bad row in the sheet is reported clearly and doesn't stop the other rows from importing.

## Mobile (P2/P3)
- Every screen works at 360 px with no sideways scrolling and tap targets of at least 44 px.
- An invoice can be created in under a minute on a phone.
- The PDF is readable on a phone and prints correctly on A4.
- The app installs as a PWA (home-screen icon).

## Exit criteria for Tinku's first use
All P1 cases pass, there are no open P2 bugs in purchase, repack, sale or invoice, and the import has been checked against the spreadsheet.

## Revision 2: long-term architecture (docs 02, 06, 07)

### P1: Finance journal and reports (doc 07 section 5)
| ID | Case | Expected |
|---|---|---|
| FIN-01 | Every purchase, invoice, expense and payment | Posts a balanced journal entry (debits equal credits) |
| FIN-02 | Trial balance for any date range | Debit and credit totals are equal |
| FIN-03 | P&L gross margin | Sales minus COGS from the journal equals the sum of invoice-line margins |
| FIN-04 | Stock value | Dashboard value = inventory account balance = `vw_StockValuation` |
| FIN-05 | Cancel or reverse a document | A reversing entry is posted, the original is untouched, and the totals go back to what they were before |
| FIN-06 | GST summary | Output GST equals the sum of invoice CGST, SGST and IGST, and input GST equals the GST on purchases plus expenses |
| FIN-07 | P&L by month and by financial year | The months add up to the FY figure, and 31-Mar and 1-Apr fall in the right years |
| RPT-01 | Excel and PDF exports | Figures match the screen exactly; totals don't change because of rounding |
| RPT-02 | Dashboard top SKUs and customers | Match a manual SQL total from the source tables |

### Cross-cutting (every module)
- **Company isolation:** data for company A never appears in company B's lists, reports or API calls, including when someone changes an ID by hand.
- **Audit:** every create, update, delete and reversal records who did it and when. Audit rows can't be edited.
- **Soft delete:** deleted records are hidden from lists but kept in the database. A master record that is already used in a posted document can't be hard-deleted.
- **Concurrency:** when two edits are made to the same record, the second one gets a conflict error instead of silently overwriting the first.
- **Module boundaries:** an architecture test (NetArchTest) fails the build if one module references another module's internal code or database schema.
- **Migrations:** every migration applies cleanly to an empty database and to a copy of the previous release.

### Later phases (test when built)
- **Warehouse:** transfers between warehouses and bins balance (out at the source equals in at the destination). Stock is never negative at the bin level. FEFO picking (earliest best-before first) is followed.
- **Procurement:** several vendors can supply one product, and the vendor's price on the purchase order matches the price on the bill.
- **Production/Packing:** each packing unit keeps its own batch numbers with no clashes between sites. Wastage is posted to the journal.
- **Transport:** dispatch follows each status in order. Freight costs are posted to the journal and allocated to deliveries. Stock in transit is counted once, neither lost nor double-counted.
- **Roles and permissions:** each role sees only what its permissions allow, and this holds for both the UI and direct API calls. Every permission change is audited.
- **Scale:** reports for 10 years of data run in under 5 seconds once the summary tables exist.

## Revision 3: API and engineering standards (doc 08)
| ID | Case | Expected |
|---|---|---|
| API-01 | OpenAPI contract test | Every endpoint and DTO matches the published spec; a breaking change in v1 fails CI |
| API-02 | Errors | Every 4xx/5xx is ProblemDetails; validation errors list each field; no stack traces leak |
| API-03 | Paging, filtering and sorting | Same parameters on every list; page size capped; an unknown sort field returns 400 |
| AUTH-01 | JWT | Expired or tampered token returns 401; a refresh token rotates, and reusing an old one is rejected and revokes the token family |
| AUTH-02 | Permissions | Missing permission returns 403 in the API, not only in the UI; another company's ID returns 404 |
| IDEM-01 | Same Idempotency-Key sent twice (invoice, payment) | One record created; second call returns the first response |
| IDEM-02 | Same key with a different body | 409/422, nothing created |
| IDEM-03 | Two parallel calls with the same key | Exactly one invoice number and one journal entry |
| CONC-01 | Stale ETag (If-Match) | 412; no change saved |
| TIME-01 | Stored UTC, shown IST | An invoice at 00:15 IST on 1-Apr falls in the new FY and on the right report day |
| OPS-01 | Health checks | `/health` reports a database outage; it doesn't leak secrets |
| OPS-02 | Rate limiting | Over the limit returns 429 with Retry-After; login is limited more tightly |
| OPS-03 | Logs | Structured, with a correlation ID; no passwords, tokens or full card/bank details |
| SEC-01 | Basic OWASP checks | SQL injection and XSS in text fields, over-posting of CompanyId or IDs, HTTPS only, CORS limited to our apps |

Mobile-readiness: run the API suite with a plain HTTP client, so mobile gets exactly the same rules as Angular.
