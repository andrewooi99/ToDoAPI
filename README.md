# ToDoAPI

## Local machine setup

### Prerequisites
- [.Net 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Window Docker](https://docs.docker.com/desktop/install/windows-install/) (if wish to run local in docker)
- [MSSQL](https://www.microsoft.com/en-my/sql-server/sql-server-downloads)
- [Postman](https://www.postman.com/downloads/)

### How to run it local dotnet 

- `cd ./ToDoAPI`
  - `dotnet restore`
  - `dotnet build`
  - `dotnet run --project ./ToDoAPI.csproj`
  
- Install MSSQL Server
  - Execute DatabaseScript/ToDoDB_Schema.sql
  
- Postman Collection
  - PostmanScript/ToDoAPI.postman_collection.json [How to import Postman Script](https://apidog.com/blog/how-to-import-export-postman-collection-data/)
  
_Note: to view the API contracts visit http://localhost:5251/swagger
