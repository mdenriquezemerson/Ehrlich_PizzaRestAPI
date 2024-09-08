# Pizza Place API

## Overview

Pizza Place API is a RESTful service for managing the Pizza Place's database. It provides endpoints for handling orders, pizzas, and other related operations.

## Features

- Manage orders and order details
- Retrieve order amounts and profits
- Manage pizzas and their details
- API versioning
- Swagger/OpenAPI documentation

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- Serilog for logging
- Swagger for API documentation

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server Developer 2022 (It should work with other databases but this is the only one I tested it with)

### Installation

1. Clone the repository:
https://github.com/mdenriquezemerson/Ehrlich_PizzaRestAPI.git

2. Install the required NuGet packages. Change to the right directory:
dotnet restore


3. Update the database connection string. It's on line 28 of PizzaPlaceDbContext.cs (TODO is to move the connection string to the appsettings and encrypt it).

4. Obtain datasets here and import them to SQL server if you want to prepopulate your database.
https://www.kaggle.com/datasets/mysarahmadbhat/pizza-place-sales

5. Run the WEB API and use the swagger doc or other tools like postman.

    