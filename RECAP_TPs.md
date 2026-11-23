# Récapitulatif des TP .NET (ASP.NET Core) — vérification du 23/11/2026

Pour chaque dossier de `D:\.net\repos` : l'énoncé correspondant, ce que fait le projet, comment on l'a exécuté, et ce qui a été vérifié concrètement (compilation + lancement + appels HTTP réels).

**Environnement utilisé :** .NET SDK 10.0.401, SQL Server LocalDB `(localdb)\MSSQLLocalDB`, Visual Studio 2022 (Package Manager Console) / `dotnet` CLI, `dotnet-ef` 8.0.22.

## Tableau de synthèse

| Dossier | Énoncé | Type | Compile | Fonctionne à l'exécution |
|---|---|---|---|---|
| `employees` | Chapitre 2 — ASP.NET Core MVC | MVC, données en mémoire | ✅ | ✅ |
| `tp2` (branche `TP5`) | TP2 EF Core + TP3 Identity (Chap. 4) + TP4 E‑commerce | MVC + EF Core + Identity | ✅ | ✅ |
| `TP5` (branche `main`) | Chapitre 5 — API Web (cours Employees) | Web API | ✅ | ✅ |
| `TP5` (branche `tp5_atelier_seance2`) | Atelier 5 — Movies API | Web API + DTO + AutoMapper | ✅ | ✅ |
| `TP6_WEBAPI` | TP6 — Gestion des comptes bancaires | Web API + EF Core | ✅ | ✅ |
| `TP7_CLEAN_ARCHITECTURE` (branche `master`) | TP7 — API Clean Architecture | 4 couches | ✅ | ✅ |
| `TP8_MICROSERVICES` | Atelier 8 — Microservices | 3 API + Ocelot + RabbitMQ | ✅ | ⚠️ HTTP/Gateway OK, RabbitMQ bloqué (WSL manquant) |
| `tp9` | Atelier 9 — Sécuriser une API avec JWT | Web API + Identity + JWT | ✅ | ✅ (après `update-database`, voir §8) |
| `tp10` | Atelier 10 — Blazor Partie 1 | API + Blazor WebAssembly | ✅ | ✅ |

`TP7_pdf_pages` ne contient que des images des pages de l'énoncé TP7 (pas de code).

---

## 1. `employees` — Chapitre 2 : ASP.NET Core MVC

**But :** découvrir le modèle MVC avec un exemple « Employés ».

**Contenu :**
- `Models/Employee.cs`, `Models/Repository/IRepository.cs` et `EmployeeRepository.cs` : liste d'employés **en mémoire** (pas de base), avec recherche.
- `EmployeeController` : actions Index, Details, Create, Edit, Delete (+ POST).
- Vues Razor `Views/Employee/*.cshtml`, layout Bootstrap.
- Injection de dépendances du repository dans `Program.cs`.

**Exécution :**
```powershell
cd D:\.net\repos\employees\WebApplication1
dotnet run
```
Puis ouvrir `https://localhost:7207/Employee` (ou F5 dans Visual Studio).

**Vérifié :** `/`, `/Employee`, `/Employee/Details/1` → 200.

---

## 2. `tp2` — TP2 (EF Core), TP3 (Identity) et TP4 (E‑commerce) : projet « DotNetShop »

Un seul projet `WebApplication2` qui a été enrichi au fil des TP. Le travail complet est sur la branche Git **`TP5`** (actuellement active). Les branches `main`, `tp3` et `TP4` pointent toutes sur le commit du TP2 seul.

**TP2 — Gestion des articles (EF Core Code First) :**
- Modèles `Product`, `Category`, contexte `AppDbContext`.
- Repositories `IProductRepository`/`ProductRepository`, `ICategorieRepository`/`CategoryRepository` injectés avec `AddScoped`.
- Chaîne de connexion `ProductDBConnection` → base `MyBaseDB` sur LocalDB.
- `ProductController` / `CategoryController` (CRUD + vues), liste déroulante des catégories (`ViewBag` + `SelectList`).
- Upload d'image via `CreateViewModel` / `EditViewModel` (`IFormFile`) et migration `AddPhotoPathToProducts`.

**TP3 / Chapitre 4 — ASP.NET Core Identity :**
- `AppDbContext` hérite de `IdentityDbContext`, `AddIdentity<IdentityUser, IdentityRole>()`, migration `MajIdentity`.
- `AccountController` : Register, Login, Logout, AccessDenied (ViewModels `RegisterViewModel`, `LoginViewModel`).
- `AdminController` : création / édition de rôles, ajout/retrait d'utilisateurs dans un rôle, tableau de bord.
- Liens Login/Register/Logout affichés selon l'état de connexion ; `SeedData` initialise les données.

**TP4 — Partie client e‑commerce :**
- Menu latéral des catégories (vue partielle `_CategoriesMenu.cshtml` + `site.js`) et pagination (`ProduitPaginationViewModel`, 4 produits par page).
- Panier (`Models/Help/Item.cs`, `ListeCart.cs`, `PanierController`), Checkout, Confirmation.
- Commandes (`Order`, `OrderItem`, `DeliveryMethod`, `OrderStatus`, `OrderController` : Mes commandes, Gestion), migration `ECommerceFeatures`.

**Exécution :**
```powershell
cd D:\.net\repos\tp2\WebApplication2\WebApplication2
dotnet ef database update
dotnet run
```
(dans Visual Studio : `Add-Migration ...` puis `Update-Database` dans la Package Manager Console, puis F5). URL : `https://localhost:7244`.

**Vérifié :** `/`, `/Product`, `/Category`, `/Account/Login` → 200 (base `MyBaseDB` présente).

---

## 3. `TP5` branche `main` — Chapitre 5 : API Web (cours)

**But :** première API REST avec EF Core.
- Modèles `Employee`, `Department`, `AppDbContext`, `DbInitializer` (données de démonstration).
- Repositories Employee/Department.
- `EmployeesController` : GET all, GET `{id}`, GET `search`, POST, PUT `{id}`, DELETE `{id}` (codes 201/404/400, validation, e‑mail déjà utilisé) ; `DepartmentsController` : GET, GET `{id}`.
- Base `APIEmployeeDB`, Swagger.

**Exécution :**
```powershell
cd D:\.net\repos\TP5
git checkout main
dotnet run --project TP5.Api
```
Puis Swagger : `https://localhost:7129/swagger`.

**Vérifié :** `GET /api/Employees` et `GET /api/Departments` → 200 avec données.

---

## 4. `TP5` branche `tp5_atelier_seance2` — Atelier 5 : Movies API

**But :** API de films avec catégories (genres), DTO, AutoMapper et upload de fichier.
- Modèles `Movie`, `Genre`, `ApplicationDbContext`, migration `InitialMovies`, base `MoviesAtelier5`.
- Services `IGenresService`/`GenresService`, `IMoviesService`/`MoviesService` (filtre par genre, tri par note).
- DTO `GenreDto`, `MovieDto`, `MovieDetailsDto` + `Helpers/MappingProfile` (AutoMapper).
- `MoviesController` : upload du poster (`IFormFile`), contrôle **.png/.jpg** et **1 Mo max**, vérification que le genre existe.

**Exécution :**
```powershell
cd D:\.net\repos\TP5
git checkout tp5_atelier_seance2
dotnet ef database update --project TP5.Atelier5
dotnet run --project TP5.Atelier5
```
Swagger : `https://localhost:7129/swagger`.

**Vérifié :** `GET /api/Genres` et `GET /api/Movies` → 200 avec données.

---

## 5. `TP6_WEBAPI` — TP6 : Gestion des comptes bancaires

**But :** API de gestion de clients, comptes et opérations (retrait, versement, virement).
- Modèles `Client` (1‑N) `Compte` (1‑N) `Operation`, `AppDbContext`, migration `InitialCreate`, base `bd`.
- `ICompteRepository` / `CompteRepository`.
- `CompteController` : CRUD, `search/{clientId}`, `POST operation`, `{num}/nombre-operations`, `{num}/extrait`, `solde-total/{clientId}`, `POST virement`, `total-retraits/{clientId}`.
- Un compte rendu existe déjà : `TP6_WEBAPI/CompteRendu_TP6.md` (+ `.docx`).

**Exécution :**
```powershell
cd D:\.net\repos\TP6_WEBAPI
dotnet ef database update
dotnet run
```
Swagger : `http://localhost:5096/swagger`.

**Vérifié :** `GET /api/Compte` et `GET /api/Compte/1` → 200 avec client et opérations.

---

## 6. `TP7_CLEAN_ARCHITECTURE` — TP7 : API Clean Architecture

**But :** gestion de produits organisée en 4 couches.
- `ProductManagement.Domain` : entité `Product` (règles : prix positif, stock non négatif).
- `ProductManagement.Application` : DTO (`ProductDto`, `CreateProductDto`, `UpdateProductDto`), interfaces `IProductRepository`, `IProductService`, implémentation `ProductService`.
- `ProductManagement.Infrastructure` : `ApplicationDbContext` (EF Core SQL Server, base `ProductManagementDB`), `ProductRepository`, `DependencyInjection.cs`.
- `ProductManagement.API` : `ProductsController` (GET, GET `{id}`, POST, PUT, DELETE) + Swagger.

> ⚠️ **Attention :** ce dossier est actuellement sur la branche Git `tp8` (qui contient une copie du TP8 microservices). Le code Clean Architecture est sur la branche **`master`**. Ouvrir `ProductManagement.slnx` sur la branche `tp8` ne marche pas (projets absents). Il faut d'abord :
> ```powershell
> cd D:\.net\repos\TP7_CLEAN_ARCHITECTURE
> git checkout master
> ```

**Exécution (après `git checkout master`) :**
```powershell
dotnet ef database update --project ProductManagement.Infrastructure --startup-project ProductManagement.API
dotnet run --project ProductManagement.API
```
Swagger : `http://localhost:5219/swagger`.

**Vérifié (sur la branche `master`) :** `GET /api/Products` → 200 (Laptop, Smartphone…).

---

## 7. `TP8_MICROSERVICES` — Atelier 8 : Introduction aux microservices

**But :** une solution `MicroservicesApp` avec 3 API + une librairie partagée.
- `ProductsAPI` (port 5066) : CRUD produits, base `ProductsMicroserviceDB`, publie un message `ProductCreated` via **MassTransit/RabbitMQ**.
- `OrdersAPI` (port 5065) : CRUD commandes, base `OrdersMicroserviceDB`, `ProductCreatedConsumer` qui recopie les produits reçus dans sa propre base.
- `ApiGateway` (port 5162) : **Ocelot**, routes `/apigateway/Products` et `/apigateway/Orders` (`ocelot.json`).
- `Shared.Models` : contrat de message `Messages/ProductCreated`.
- `docker-compose.yml` : RabbitMQ (AMQP `localhost:4001`, interface `http://localhost:15672`, guest/guest).

**Exécution :**
```powershell
cd D:\.net\repos\TP8_MICROSERVICES
docker compose up -d
```
```powershell
dotnet run --project ProductsAPI --urls http://localhost:5066
```
```powershell
dotnet run --project OrdersAPI --urls http://localhost:5065
```
```powershell
dotnet run --project ApiGateway --urls http://localhost:5162
```
(ou « Plusieurs projets de démarrage » dans Visual Studio).

**Vérifié :**
- `GET` direct sur ProductsAPI et OrdersAPI → 200.
- `GET /apigateway/Products` et `/apigateway/Orders` via Ocelot → 200 (la gateway route bien).
- ⚠️ `POST /apigateway/Products` reste bloqué : **RabbitMQ ne tourne pas**, donc la publication du message échoue (`Connection failed, host 127.0.0.1:4001`). Docker Desktop est installé mais ne démarre pas car **WSL n'est pas installé** sur ce PC. À faire une fois (PowerShell en administrateur, puis redémarrer le PC) :
```powershell
wsl --install
```
Ensuite ouvrir Docker Desktop, puis `docker compose up -d` dans `TP8_MICROSERVICES` avant de lancer les 3 projets.

---

## 8. `tp9` — Atelier 9 : Sécuriser une API avec JWT

**But :** authentification par jeton JWT avec Identity.
- `ApplicationUser` (Identity), `ApplicationDbContext`, migration `initialcreate`, base `TestApiJWT`.
- `Helpers/JWT.cs` (Key, Issuer, Audience, durée) lu depuis `appsettings.json`.
- `IAuthService` / `AuthService` : `RegisterAsync` (crée l'utilisateur + rôle `User`), `GetTokenAsync` (génère le JWT avec les rôles en claims), `AddRoleAsync`.
- `AuthController` : `POST api/Auth/register`, `POST api/Auth/token`, `POST api/Auth/addrole`.
- `WeatherForecastController` protégé par `[Authorize(Roles = "admin")]`.
- Swagger configuré avec le bouton **Authorize** (Bearer).

**Exécution :**
```powershell
cd D:\.net\repos\tp9\SecureAPI_JWT
dotnet ef database update
dotnet run
```
Swagger : `https://localhost:7005/swagger`. Scénario : `register` → `token` → copier le jeton dans **Authorize** (`Bearer <token>`) → appeler `WeatherForecast`.

**Vérifié :**
- Problème trouvé : la base `TestApiJWT` **n'existait pas** (la migration n'avait jamais été appliquée), donc `register` renvoyait une erreur 500. J'ai lancé `dotnet ef database update` (équivalent de `update-database` de l'énoncé) : la base est maintenant créée.
- Ensuite : `register` → 200 avec un token ; `WeatherForecast` sans token → **401** ; avec un token de rôle `User` → **403** (normal, il faut le rôle `admin`). L'utilisateur de test a été supprimé après le test.
- Pour tester le rôle admin (dernière étape de l'énoncé) : créer le rôle `admin` dans la table `AspNetRoles`, appeler `POST api/Auth/addrole` avec `{ "userId": "...", "role": "admin" }`, puis redemander un token.

---

## 9. `tp10` — Atelier 10 : Blazor (Partie 1)

**But :** client Blazor WebAssembly qui consomme une API.
- `EmployeeManagement.Models` : `Employee`, `Department`, `Gender` (bibliothèque partagée).
- `EmployeeManagement.Api` : `EmployeesController`, `DepartmentsController` (GET, GET `{id}`), repositories, EF Core **SQLite** (`EmployeeDB.db`), CORS autorisé pour le client Blazor.
- `EmployeeManagement.Web` (Blazor WebAssembly) : services `EmployeeService` / `DepartmentService` (HttpClient vers `https://localhost:7089/`), composants avec code‑behind : `EmployeeList` (`/EmployeeList`), `EmployeeDetails` (`/EmployeeDetails/{Id}`), `EditEmployee` (`/editemployee/{id}`, le bouton Update est volontairement désactivé — prévu pour la partie 2).

**Exécution (2 terminaux, l'API d'abord) :**
```powershell
cd D:\.net\repos\tp10
dotnet run --project EmployeeManagement.Api --launch-profile https
```
```powershell
cd D:\.net\repos\tp10
dotnet run --project EmployeeManagement.Web --launch-profile https
```
Puis ouvrir `https://localhost:7297/EmployeeList`.

**Vérifié :** `GET /api/employees` et `/api/departments` → 200 avec données ; le client Blazor est servi (`/` et `blazor.webassembly.js` → 200).

---

## Remarques générales

- Les 9 projets/solutions **compilent sans erreur** (seulement des avertissements, ex. nullabilité).
- Tous les TP ont leur base de données sur LocalDB (sauf TP10 en SQLite) ; elles existent déjà sauf celle du TP9 qui vient d'être créée.
- `TP5` contient plusieurs dossiers `.build-check*` : ce sont des sorties de compilation de vérification, ils peuvent être supprimés sans risque.
- `tp9` et `tp10` ne sont pas encore sous Git (contrairement aux autres dossiers).
