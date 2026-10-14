# SPICO Vita

Business software and launch workspace for **SPICO Vita**, a Bangalore pulses brand.

| Folder | Contents |
|---|---|
| `application/api` | .NET 8 Web API – Clean Architecture (Domain, Application, Infrastructure, Api) + unit tests, EF Core / MS SQL Server |
| `application/frontend` | Angular web app (placeholder, pending) |
| `docs/` | Requirements, architecture, data model, hosting, test plan, roadmap, reporting, API standards |
| `common/` | Business checklists & notes: tracker, costing, suppliers, licences, plans, marketing, accounts, pack design |
| `STATUS.md` | **Single status file** – module-wise requirements and Completed / In progress / Pending |

## Run the API
```bash
cd application/api
dotnet build && dotnet test
# SQL Server dev: docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=<pwd> -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
dotnet user-secrets --project src/SpicoVita.Api set "ConnectionStrings:Default" "<conn>"
dotnet run --project src/SpicoVita.Api   # Swagger at /swagger, health at /health
```
Never commit secrets – use user-secrets or environment variables.
