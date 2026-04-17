# AGENTS

## Repo shape
- Solution: `RestApiLambda/RestApiLambda.sln` with two independent Lambda projects:
  - `RestApiLambda/SimpleApi/SimpleApi.csproj` (Lambda Annotations function)
  - `RestApiLambda/MinimalLambdaApi/MinimalLambdaApi.csproj` (ASP.NET Core Web API in Lambda)
- Real entrypoints:
  - `RestApiLambda/SimpleApi/Functions.cs` (`Functions.Get`) and DI wiring in `RestApiLambda/SimpleApi/Startup.cs`
  - `RestApiLambda/MinimalLambdaApi/Program.cs` (top-level ASP.NET Core setup + `AddAWSLambdaHosting`)

## Verified commands
- From `D:\2026\work\lambda\templates\RestApiLambda`:
  - `dotnet build RestApiLambda.sln`
  - `dotnet test RestApiLambda.sln` (currently exits 0 but only restore/builds; there are no test projects)
- Deploy from each project directory (requires `Amazon.Lambda.Tools`):
  - `dotnet lambda deploy-serverless` in `RestApiLambda/SimpleApi`
  - `dotnet lambda deploy-serverless` in `RestApiLambda/MinimalLambdaApi`

## AWS/Lambda quirks
- `RestApiLambda/SimpleApi/serverless.template` is partially managed by `Amazon.Lambda.Annotations`; builds can rewrite generated function resource details (`SimpleApiFunctionsGetGenerated`).
- Keep runtime pairs aligned when editing projects/templates:
  - `SimpleApi`: `net8.0` <-> `dotnet8`
  - `MinimalLambdaApi`: `net10.0` <-> `dotnet10`
- `RestApiLambda/MinimalLambdaApi/aws-lambda-tools-defaults.json` has blank `profile` and `region`; set them or pass CLI flags before deploy.
- `RestApiLambda/MinimalLambdaApi/serverless.template` uses executable assembly handler (`"Handler": "MinimalLambdaApi"`), not `Assembly::Type::Method`.

## Doc mismatch to avoid
- `RestApiLambda/SimpleApi/Readme.md` and `RestApiLambda/MinimalLambdaApi/Readme.md` are template boilerplate and reference non-existent `src/` and `test/` paths.
- Trust the solution and project files for real commands/paths.
