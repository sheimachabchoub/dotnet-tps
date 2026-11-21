# TP7 - ASP.NET Core Web API avec Clean Architecture

La solution `ProductManagement` est organisée en quatre couches :

- `ProductManagement.Domain` : entité `Product` et règles métier ;
- `ProductManagement.Application` : DTOs, interfaces de repository et service applicatif ;
- `ProductManagement.Infrastructure` : `ApplicationDbContext`, SQL Server, repository et migration EF Core ;
- `ProductManagement.API` : contrôleur REST et configuration Swagger/DI.

## Lancer le projet

Depuis ce dossier :

```powershell
dotnet ef database update --project ProductManagement.Infrastructure --startup-project ProductManagement.API
dotnet run --project ProductManagement.API
```

Swagger est disponible sur `/swagger`.

Endpoints : `GET`, `POST`, `PUT` et `DELETE` sur `/api/Products`.
