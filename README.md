# KqiShop

## Annotation

A project with two microservices for products and orders and with an API gateway for interacting with them via swagger.

## How run

### 1. Standard way

Create databases:

`psql -U postgres -c "CREATE DATABASE product_service;"`

`psql -U postgres -d product_service -f ./product_service.sql`

`psql -U postgres -c "CREATE DATABASE order_service;"`

`psql -U postgres -d order_service -f ./order_service.sql`

In WebApi/appsettings.json files (ProductService and OrderService) establish connection to DBs "ConnectionStrings:DefaultConnection"

Run three projects (ProductService, OrderService, ApiGateway) with http profiles in Visual Studio

http://localhost:5292/swagger

### 2. Run using docker compose

`docker-compose up`

http://localhost:5000/swagger

### 3. Run using docker compose with loading images from docker hub

`docker-compose -f ./docker-compose-with-images.yml up`

http://localhost:5000/swagger
