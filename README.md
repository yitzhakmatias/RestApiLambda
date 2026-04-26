# RestApiLambda

AWS Lambda ASP.NET projects — all targeting `net8.0 / dotnet8`.

## Projects

| Project | Style | Handler | Notes |
|---|---|---|---|
| `SimpleApi` | Lambda Annotations (attribute-based) | `SimpleApi::SimpleApi.Functions::Get` | DI wired in `Startup.cs` |
| `MinimalLambdaApi` | ASP.NET Core Minimal API | `MinimalLambdaApi` (executable assembly) | `AddAWSLambdaHosting` in `Program.cs` |
| `CustomerCrudLambda` | Minimal API + DynamoDB | `CustomerCrudLambda` (executable assembly) | Requires existing DynamoDB table |
| `CustomerCrudLambda.Tests` | xUnit + Moq + FluentAssertions | — | Targets `net10.0`; library projects target `net8.0` |

## Build & Test

```powershell
# Build
dotnet build RestApiLambda/RestApiLambda.sln

# Test (currently only restores/builds — no live tests run due to net8.0/net10.0 target mismatch)
dotnet test RestApiLambda/RestApiLambda.sln
```

## Deploy

Requires `Amazon.Lambda.Tools` (included in AWS .NET SDK).

```powershell
# SimpleApi
cd RestApiLambda/SimpleApi && dotnet lambda deploy-serverless

# MinimalLambdaApi — set profile/region in aws-lambda-tools-defaults.json first
cd RestApiLambda/MinimalLambdaApi && dotnet lambda deploy-serverless

# CustomerCrudLambda — requires existing DynamoDB table (default: "customers")
cd RestApiLambda/CustomerCrudLambda && dotnet lambda deploy-serverless
```

## Entry Points

- `RestApiLambda/SimpleApi/Functions.cs` — `Functions.Get()` method attributed with `[LambdaFunction]`
- `RestApiLambda/MinimalLambdaApi/Program.cs` — top-level Minimal API setup
- `RestApiLambda/CustomerCrudLambda/Program.cs` + `Features/Customers/CustomerEndpoints.cs`

## AWS / Lambda Quirks

- `SimpleApi/serverless.template` is partially rewritten by `Amazon.Lambda.Annotations` (v1.9.0.0) on build — the generated handler name includes `_Generated` suffix (e.g., `SimpleApiFunctionsGetGenerated`).
- Runtime pair must stay aligned: `net8.0` <-> `dotnet8`.
- `CustomerCrudLambda` uses executable-assembly handler style (`"Handler": "CustomerCrudLambda"`), not the `Assembly::Type::Method` format.
- `MinimalLambdaApi/aws-lambda-tools-defaults.json` has blank `profile` and `region` — set them before deploying.

## DO NOT TRUST

The per-project `Readme.md` files under `SimpleApi/`, `MinimalLambdaApi/`, and `CustomerCrudLambda/` are template boilerplate that references non-existent `src/` and `test/` paths. Use solution/project files and `aws-lambda-tools-defaults.json` for real commands and configuration.