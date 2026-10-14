# Hosting Path (approximate list prices, ~₹84/USD; verify before sign-up)

| Stage | When | Setup | Approx. monthly |
|---|---|---|---|
| 0. Local | Now | Developer machine / shared computer, SQL Server Express or Docker | ₹0 |
| 1. Free pilot | Pilot, 1 user | Azure App Service F1 (free, sleeps, 60 CPU min/day) + Static Web Apps Free + Azure SQL Database free offer (100k vCore-sec + 32 GB/month, auto-pause) | **₹0** (card needed for sign-up) |
| 2. Small business | 2 to 10 staff, daily use | App Service B1 (~$13) + Azure SQL serverless/General Purpose small or Basic/S0 (~$5 to $15) + Blob storage for PDFs/photos (~$1) + domain | **~₹1,600 to ₹2,500** |
| 3. Growing ops | Multiple warehouses, transport module, 10 to 50 users | App Service P0v3 (~$60) or 2 x B2, Azure SQL S1/S2 or GP 2 vCore serverless (~$30 to $150), Application Insights (~$5 to $20), backups with longer retention | **~₹8,000 to ₹20,000** |
| 4. Production scale | Multi-company, heavy reporting | App Service P1v3 + staging slot, Azure SQL GP 2 to 4 vCore with geo backup, Service Bus Basic (~$1 to $10) if modules split, Front Door/CDN optional | **~₹30,000 to ₹70,000** |

Alternative at stages 1 to 2: a Linux VPS (~₹400 to ₹700) running Docker with SQL Server Express (10 GB cap); cheapest always-on but we own patching and backups.

The code is the same at every stage (container-ready, config via environment variables), so moving up is a hosting change, not a rewrite.

**Decision for Tinku:** create an Azure account (card required, ₹0 on free tier) when the pilot is ready to go online.
