# Run the app
This assumes you already setup the db

## Run the Default
The default works for fullscreen play 
```shell
dotnet run --project .\UmaEventReaderV2
```

## Select the Area
you can also manually select the event area, should you be running in a smaller window
```shell
dotnet run --project .\UmaEventReaderV2 --select-area
```

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
dotnet ef migrations add CreateDB --project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj --startup-project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj --output-dir Migrations
 ```

### Apply the migrations

```shell
dotnet ef database update --project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj --startup-project ./UmaEventReaderV2.Infrastructure/UmaEventReaderV2.Infrastructure.csproj
```

### Initialize the database

Switch to the `Infrastructure` project.

```shell
cd UmaEventReaderV2.Infrastructure
```

and run it.

```shell
dotnet run
```

This will read the data from the json file and fill the database
