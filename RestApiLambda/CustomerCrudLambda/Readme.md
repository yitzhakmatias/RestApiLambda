# Customer CRUD Lambda API

This project provides a simple customer CRUD API running in AWS Lambda with an HTTP API Gateway and a DynamoDB table.

## Endpoints

- `GET /customers`
- `GET /customers/{id}`
- `POST /customers`
- `PUT /customers/{id}`
- `DELETE /customers/{id}`

Request body for create/update:

```json
{
  "name": "Ada Lovelace",
  "email": "ada@example.com"
}
```

## Deploy

From this folder:

```bash
dotnet lambda deploy-serverless --resolve-s3 true
```
