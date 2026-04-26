# AGENTS

## Verified commands

- Build: `dotnet build RestApiLambda.sln` from `D:\2026\work\lambda\templates\RestApiLambda`
- Test: `dotnet test RestApiLambda.sln` (currently exits 0 but only restore/builds; no test projects exist except `CustomerCrudLambda.Tests`)

## Solution structure

`RestApiLambda.sln` contains two independent Lambda projects plus supporting projects:

| Project | Template | Runtime | Handler |
|---|---|---|---|
| `SimpleApi` | Lambda Annotations | net8.0 / dotnet8 | `SimpleApi::SimpleApi.Functions::Get` |
| `MinimalLambdaApi` | ASP.NET Core Minimal API | net8.0 / dotnet8 | `MinimalLambdaApi` (executable assembly) |
| `CustomerCrudLambda` | ASP.NET Core Minimal API + DynamoDB | net8.0 / dotnet8 | `CustomerCrudLambda` (executable assembly) |
| `CustomerCrudLambda.Tests` | xUnit + Moq + FluentAssertions | net10.0 | — |

**Real entrypoints:**
- `RestApiLambda/SimpleApi/Functions.cs` (`Functions.Get`) + DI wiring in `RestApiLambda/SimpleApi/Startup.cs`
- `RestApiLambda/MinimalLambdaApi/Program.cs` (top-level ASP.NET Core setup + `AddAWSLambdaHosting`)
- `RestApiLambda/CustomerCrudLambda/Program.cs` + `RestApiLambda/CustomerCrudLambda/Features/Customers/CustomerEndpoints.cs`

## Deploy

Run `dotnet lambda deploy-serverless` from each project directory (requires `Amazon.Lambda.Tools`):

```
cd RestApiLambda/SimpleApi && dotnet lambda deploy-serverless
cd RestApiLambda/MinimalLambdaApi && dotnet lambda deploy-serverless
cd RestApiLambda/CustomerCrudLambda && dotnet lambda deploy-serverless
```

`RestApiLambda/MinimalLambdaApi/aws-lambda-tools-defaults.json` has blank `profile` and `region`; set them or pass CLI flags before deploy.

## AWS/Lambda quirks

- `RestApiLambda/SimpleApi/serverless.template` is partially managed by `Amazon.Lambda.Annotations` (v1.9.0.0); builds can rewrite generated function resource details (`SimpleApiFunctionsGetGenerated`).
- Keep runtime pairs aligned when editing projects/templates: `net8.0` <-> `dotnet8`
- `RestApiLambda/CustomerCrudLambda/serverless.template` uses executable assembly handler (`"Handler": "CustomerCrudLambda"`), not `Assembly::Type::Method`.
- `CustomerCrudLambda` requires an existing DynamoDB table (parameter `CustomersTableName`, default `customers`).

## Doc mismatch to avoid

`RestApiLambda/SimpleApi/Readme.md`, `RestApiLambda/MinimalLambdaApi/Readme.md`, and `RestApiLambda/CustomerCrudLambda/Readme.md` are template boilerplate and reference non-existent `src/` and `test/` paths. Trust the solution and project files for real commands/paths.