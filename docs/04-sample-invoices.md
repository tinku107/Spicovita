# Sample Invoices (QA reference)

## Rounding rules (normative)
1. Line taxable = Qty x Rate, rounded to 2 dp.
2. Tax is computed on the invoice's total taxable value per rate: CGST = SGST = round(Taxable x 2.5%, 2); IGST = round(Taxable x 5%, 2).
3. Rounding is **half away from zero** (`MidpointRounding.AwayFromZero`), NOT .NET's default banker's rounding.
4. Grand total rounded to the nearest rupee; difference shown as a "Round off" line.

## Sample 1: intra-state (Karnataka kirana), CGST + SGST
Invoice SV/2026-27/0001, customer state 29.
| SKU | Qty | Rate | Taxable |
|---|---|---|---|
| Toor dal 1 kg | 20 | 142.86 | 2,857.20 |
| Toor dal 500 g | 30 | 73.33 | 2,199.90 |
| **Taxable total** | | | **5,057.10** |
| CGST 2.5% (126.4275) | | | 126.43 |
| SGST 2.5% | | | 126.43 |
| Subtotal | | | 5,309.96 |
| Round off | | | +0.04 |
| **Grand total** | | | **5,310.00** |

## Sample 2: inter-state (e.g. Tamil Nadu, state 33), IGST
Invoice SV/2026-27/0002.
| SKU | Qty | Rate | Taxable |
|---|---|---|---|
| Moong dal 1 kg | 15 | 152.38 | 2,285.70 |
| IGST 5% (114.285) | | | **114.29** (banker's rounding would wrongly give 114.28) |
| Subtotal | | | 2,399.99 |
| Round off | | | +0.01 |
| **Grand total** | | | **2,400.00** |

## Best-before check
Packed 13-Oct-2026, shelf life 6 months: best-before 13-Apr-2027. Packed 31-Aug-2026 + 6 months gives 28-Feb-2027 (end-of-month clamp).
