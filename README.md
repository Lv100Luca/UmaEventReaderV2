hi

# Setup the database
## Run the db container

Run the database container

```shell
docker compose up
```

## Create or run the migrations
Migrations should already be setup inside `.\UmaEventReaderV2.Infrastructure\migrations`. 
If they are, move to the migrations step

### Create migrations
use this to create the migrations

```powershell
dotnet ef migrations add InitialCreate `
  --project UmaEventReaderV2.Infrastructure `
  --startup-project UmaEventReaderV2.Infrastructure `
  --output-dir Migrations
```