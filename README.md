hi

# Setup the database
## Run the db container

Run the database container

```shell
docker compose up
```

## Create or run the migrations

### Install EntityFramework

```shell
dotnet tool install --global dotnet-ef
```

Migrations should already be setup inside `.\UmaEventReaderV2.Infrastructure\migrations`. 
If they are, move to the migrations step

### Create migrations
use this to create the migrations

```shell
dotnet ef migrations add InitialCreate --project .\UmaEventReaderV2.Infrastructure.csproj --startup-project .\UmaEventReaderV2.Infrastructure.csproj --output-dir Migrations
 ```

### Apply the migrations

```shell
dotnet ef database update --project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj --startup-project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj
```