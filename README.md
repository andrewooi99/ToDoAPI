# ToDo List API Application

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


# ToDo List REST API

The REST API to the ToDo List is described below.

# Authentication And Login

## Admin Registration

### Request 

`POST /api/Authentication/RegisterAdmin`

    curl --location 'http://localhost:5251/api/Authentication/RegisterAdmin' \
		 --header 'Content-Type: application/json' \
		 --data-raw '{
		 	 "username": "ToDoAdmin",
		 	 "password": "Password@123",
		 	 "email": "todo_admin@gmail.com"
		 }'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:27:27 GMT
	Transfer-Encoding: chunked
	 
	{"status":"Success","message":"Registration Successful!"}
	
## User Registration

### Request 

`POST /api/Authentication/Register`

    curl --location 'http://localhost:5251/api/Authentication/Register' \
		 --header 'Content-Type: application/json' \
		 --data-raw '{
		 	 "username": "ToDoUser",
		 	 "password": "Password@123",
		 	 "email": "todo_user@gmail.com"
		 }'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:27:27 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Registration Successful!"}
	
## Login

### Request 

`POST /api/Authentication/Login`

	curl --location 'http://localhost:5251/api/Authentication/Login' \
		 --header 'Content-Type: application/json' \
		 --data-raw '{
			 "Username": "ToDoAdmin",
			 "password": "Password@123"
		 }'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:29:51 GMT	
	Transfer-Encoding: chunked
	{"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVG9Eb0FkbWluIiwianRpIjoiNjFiMTc1YTEtMWY1ZS00Y2UxLWI3MmUtMWE5NTJiNjNlNmViIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjpbIkFkbWluIiwiVXNlciJdLCJleHAiOjE3MDEwMTYxOTEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NDIwMCJ9.IEcscWdkUUFzacUGs4amBrlpjOFIcqS9asXh-2Ra1Rc","expiration":"2023-11-26T16:29:51Z"}
	
# ToDo API CRUD

## Get ToDo List With Tags

### Request 

`GET /api/ToDo/GetToDoItemWithTags`

	curl --location 'http://localhost:5251/api/ToDo/GetToDoItemWithTags?Status=1&DueDateFrom=2023-11-10&DueDateTo=2023-11-25&Sorting=DueDate%7CDESC&Sorting=Status%7CASC' \
	--header 'Authorization: Bearer token'
	
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:33:38 GMT
	Transfer-Encoding: chunked
	
	[{
		"id": 1,
		"name": "ToDo 1",
		"description": "Test",
		"dueDate": "2023-11-25T00:00:00",
		"status": 1,
		"priority": 2,
		"sharedBy": [
			"User20231119021620"
		],
		"toDoItemTagList": [
			{
				"id": 1,
				"toDoItemId": 1,
				"tagKey": "ToDo 1 Tag 1",
				"tagValue": "ToDo 1 Value 1",
				"createdAt": "2023-11-17T17:48:54.42",
				"createdBy": "ToDoAdmin",
				"updatedAt": "2023-11-18T09:03:30.31",
				"updatedBy": "ToDoAdmin"
			},
			{
				"id": 2,
				"toDoItemId": 1,
				"tagKey": "ToDo 1 Tag 2",
				"tagValue": "ToDo 1 Value 2",
				"createdAt": "2023-11-17T17:48:54.42",
				"createdBy": "ToDoAdmin",
				"updatedAt": "2023-11-18T09:03:30.31",
				"updatedBy": "ToDoAdmin"
			}
		],
		"createdAt": "2023-11-17T17:48:54.42",
		"createdBy": "ToDoAdmin",
		"updatedAt": "2023-11-18T09:03:30.31",
		"updatedBy": "ToDoAdmin"
	}]
	
## Get ToDo With Tags By Id

### Request 

`GET /api/ToDo/GetToDoItemWithTagsById`

	curl --location 'http://localhost:5251/api/ToDo/GetToDoItemWithTagsById?id=1' \
	--header 'Authorization: Bearer token'
	
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 06:02:53 GMT
	Transfer-Encoding: chunked
	
	{
		"id": 1,
		"name": "ToDo 1",
		"description": "Test Update By User",
		"dueDate": "2023-11-25T00:00:00",
		"status": 1,
		"priority": 2,
		"sharedBy": [
			"User20231119021620"
		],		
		"toDoItemTagList": [
			{
				"id": 1,
				"toDoItemId": 1,
				"tagKey": "ToDo Tag Update By User",
				"tagValue": "ToDo Tag Update Value",
				"createdAt": "2023-11-17T17:48:54.42",
				"createdBy": "ToDoAdmin",
				"updatedAt": "2023-11-19T13:36:59.757",
				"updatedBy": "User20231119021620"
			},
			{
				"id": 2,
				"toDoItemId": 1,
				"tagKey": "ToDo Tag Update",
				"tagValue": "ToDo Tag Update Value",
				"createdAt": "2023-11-17T17:48:54.42",
				"createdBy": "ToDoAdmin",
				"updatedAt": "2023-11-19T13:36:59.757",
				"updatedBy": "User20231119021620"
			}
		],
		"createdAt": "2023-11-17T17:48:54.42",
		"createdBy": "ToDoAdmin",
		"updatedAt": "2023-11-19T13:36:59.757",
		"updatedBy": "User20231119021620"
	}
	
## Create ToDo

Allow ToDo Sharing by adding username to SharedBy parameters

### Request 

`POST /api/ToDo/CreateToDoItem`

	curl --location 'http://localhost:5251/api/ToDo/CreateToDoItem' \
	--header 'Content-Type: application/json' \
	--header 'Authorization: Bearer token' \
	--data '{
		"Name": "ToDo 2",
		"Description": "ToDo 2 Description",
		"DueDate": "2023-11-20",
		"Status": 2,
		"Priority": 1,
		"ToDoItemTagList": [
			{
				"TagKey": "ToDo 2 Tag 1",
				"TagValue": "ToDo 2 Value 1"
			},        
			{
				"TagKey": "ToDo 2 Tag 2",
				"TagValue": "ToDo 2 Value 2"
			}
		],
		"SharedBy": [ "User1", "User2" ]
	}'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:39:32 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Create ToDo Successful"}
	
## Update ToDo

Only owner of ToDo allowed to update SharedBy

### Request 

`PATCH /api/ToDo/UpdateToDoItem`

	curl --location --request PATCH 'http://localhost:5251/api/ToDo/UpdateToDoItem' \
	--header 'Content-Type: application/json' \
	--header 'Authorization: Bearer token' \
	--data '{
		"Id": 1,
		"Name": "ToDo 1",
		"Description": "Test",
		"Status": 1,
		"Priority": 2, 
		"SharedBy": [ "User1", "User2" ]
	}'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:41:31 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Update ToDo Successful"}
	
## Delete ToDo (Authorization By Role - Only Admin Allowed)

Delete ToDo will delete all the ToDo Tags associated

### Request 

`DELETE /api/ToDo/DeleteToDoItem`

	curl --location --request DELETE 'http://localhost:5251/api/ToDo/DeleteToDoItem?id=68' \
	--header 'Authorization: Bearer token'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:44:59 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Delete ToDo Successful"}
	
## Create ToDo Tag

### Request 

`POST /api/ToDo/CreateToDoItemTag`

	curl --location 'http://localhost:5251/api/ToDo/CreateToDoItemTag' \
	--header 'Content-Type: application/json' \
	--header 'Authorization: Bearer token' \
	--data '{
		"ToDoId": 1,
		"TagKey": "ToDo 1 Extra 3",
		"TagValue": "Tag Value Extra"
	}'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:47:59 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Create ToDo Tag Successful"}
	
## Get ToDo Tags By Id

### Request 

`GET /api/ToDo/GetToDoItemTagsById`

	curl --location 'http://localhost:5251/api/ToDo/GetToDoItemTagsById?id=1' \
	--header 'Authorization: Bearer token'
	
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 06:04:43 GMT
	Transfer-Encoding: chunked
	
	{
		"id": 1,
		"toDoItemId": 1,
		"tagKey": "ToDo Tag Update By User",
		"tagValue": "ToDo Tag Update Value",
		"createdAt": "2023-11-17T17:48:56.253",
		"createdBy": "ToDoAdmin",
		"updatedAt": "2023-11-19T13:37:24.513",
		"updatedBy": "User20231119021620"
	}

## Update ToDo Tag

### Request 

`PATCH /api/ToDo/UpdateToDoItemTag`

	curl --location --request PATCH 'http://localhost:5251/api/ToDo/UpdateToDoItemTag' \
	--header 'Content-Type: application/json' \
	--header 'Authorization: Bearer token' \
	--data '{
		"Id": 2,
		"TagKey": "ToDo Tag Update",
		"TagValue": "ToDo Tag Update Value"
	}'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:49:59 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Update ToDo Tag Successful"}
	
## Delete ToDo Tag (Authorization By Role - Only Admin Allowed)

### Request 

`DELETE /api/ToDo/DeleteToDoItemTag`

	curl --location --request DELETE 'http://localhost:5251/api/ToDo/DeleteToDoItemTag?id=55' \
	--header 'Authorization: Bearer token'
		 
### Response

	HTTP/1.1 200 OK
	Content-Type: application/json; charset=utf-8
	Date: Sun, 19 Nov 2023 04:51:58 GMT
	Transfer-Encoding: chunked
	
	{"status":"Success","message":"Delete ToDo Tag Successful"}