# TP8 - Introduction aux microservices avec ASP.NET Core

Solution `MicroservicesApp` réalisée sur la branche indépendante `tp8`.

## Projets

- `ProductsAPI` : CRUD des produits, base `ProductsMicroserviceDB` et publication de `ProductCreated` ;
- `OrdersAPI` : CRUD des commandes, base `OrdersMicroserviceDB` et consommateur RabbitMQ qui synchronise les produits ;
- `ApiGateway` : passerelle Ocelot sur `http://localhost:5162` ;
- `Shared.Models` : contrat de message partagé entre les deux microservices.

## Ports

- ProductsAPI : `http://localhost:5066`
- OrdersAPI : `http://localhost:5065`
- ApiGateway : `http://localhost:5162`
- RabbitMQ AMQP : `localhost:4001`
- RabbitMQ Management : `http://localhost:15672` (`guest` / `guest`)

## Démarrage

1. Installer et démarrer Docker Desktop.
2. Depuis la racine :

```powershell
docker compose up -d
dotnet ef database update --project ProductsAPI --startup-project ProductsAPI
dotnet ef database update --project OrdersAPI --startup-project OrdersAPI
dotnet run --project ProductsAPI --urls http://localhost:5066
dotnet run --project OrdersAPI --urls http://localhost:5065
dotnet run --project ApiGateway --urls http://localhost:5162
```

Swagger est disponible sur `/swagger` pour chaque API. Les routes Ocelot sont accessibles via `/apigateway/Products` et `/apigateway/Orders`.

Après un `POST /apigateway/Products`, `ProductCreated` est publié dans RabbitMQ puis consommé par OrdersAPI, qui enregistre le produit dans sa propre base.
