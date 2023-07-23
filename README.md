# LondonStockApi 📈🚀

## Setup
This solution requres a database set up with the correct schema. Create a blank SQL Server DB and stick the connection string inside `appsettings.Development.json` where I've left the placeholder. There are EF Core migration files in the `Migrations/` folder [(info on migrations)](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/). Basically, run 
- `dotnet tool install --global dotnet-ef`
- `dotnet ef database update CreateStocksDatabase`
- `dotnet ef database update QueryStoredProcs`

If all has gone well, you should have the database schema set up - the table `dbo.StockTransactions`, the type `dbo.TickerQuery`, and the stored procedures 
- `dbo.GetPriceForTicker`
- `dbo.GetPriceForTickers`
- `dbo.GetPriceForAllTickers`

## Running
It's easiest just to run it from Visual Studio, and interact with it using something like Postman. You can also use Swagger to make requests, which should open in a browser automatically when running the API.

### Sending a transaction to the API
You can make POST requests to the `/api/StockTransaction/` endpoint to register stock transactions with the exchange. To make one of these requests, create a `StockTransactionDTO` object in the request body (you can see the schema for this object in Swagger).

### Getting a stock quote
To get the price of an individual stock, you can make a GET request to the `/api/Quote/:ticker` endpoint - e.g. `/api/quote/NWG`. It will return you the average price of all the transactions created via the POST endpoint.

### Getting multiple stock quotes
Another endpoint handles requests for multiple quotes - `/api/Quote`, with the `tickers` query parameter. You can use it like this: `/api/Quote?tickers=MSFT&tickers=NWG&tickers=AAPL`. This will return you an array of three quotes, for MSFT, NWG and AAPL respectively. If one of the requested quotes isn't in the database, you'll get a 404.

### Getting all stock quotes
Use the endpoint `/api/Quote`.

# Software architecture
This is a basic ASP.NET Core API, using a simple architecture with a presentation layer and a data layer. Requests come in via the presentation layer, and data is sent to and retrieved from the data layer. In a more complex API with more business logic, there would be a domain layer with our business rules sitting between these two layers, but there really wasn't much for it to do here, so I've left it out. The data layer is implemented with Entity Framework Core using a model-first approach, although I had to write custom SQL for the stored procedures. Unit tests are in their own project.

Entities and DTOs are separated, and the controllers use AutoMapper to convert between them. Dependencies are injected using Microsoft's DI library. This lets the presentation layer not depend on anything concrete in the data layer. The `IStockContext` interface is the way that the presentation layer interacts with the data layer. By not relying on concrete dependencies, we can unit-test the controllers.

For an MVP, it would be worth splitting the layers out to separate projects - something like:
- `LondonStockApi.WebAPI.csproj`
- `LondonStockApi.Business.csproj`
- `LondonStockApi.Data.csproj`

# Speed and scalability
The most basic way to calculate the average of all stock transaction prices might be to query the entire StockTransactions table, and average the prices in the API. However, once there are lots of transactions in the database, you'd be sending large amounts of data every time - not worth it. I thought it was better to do the averaging inside the database, since SQL is optimised for these sort of aggregate operations. Something I haven't had time to test is the effect of an index on the `Ticker` column - might make the averaging faster.

For an MVP deployed to the cloud, we might containerise the API, and deploy them in Kubernetes pods, with a load-balancer sitting between the pods and the internet. In this way we can scale up and down our compute resources with ease, using Kubernetes resource monitoring to let us know how much we're using.

That leaves the database. Over time, the frequent average operations might strain the database hardware. Something we could try would be to lazily calculate and cache average prices for each stock - calculating them when someone requests them, then storing the result in case it's requested again. Someone adding a transaction for that stock would require us to invalid the cache entry. However, you can imagine this being worth it at the end of the trading day - when the traders leave the City of London at 4:30, no more transactions are going to be posted - so we might as well calculate the average once and store it, at least until the next morning.

The brute force approach is just to increase the hardware resources of the server - easy to do when using a managed database like SQL.

Other solutions like database replicas will have issues like data consistency, so will be tricky.

# Other things I'd add for an MVP
- A set of end-to-end tests using an in-memory DB - POSTing transactions, then GETting quotes for those stocks and making sure they're correct.
- Some form of authentication on the endpoints
- A Postman collection of API calls