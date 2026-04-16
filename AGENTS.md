# AGENTS

## Repo shape
- Single-solution .NET repo: `RestApiLambda.sln` -> one project at `SimpleApi/SimpleApi.csproj`.
- Lambda entrypoint is `SimpleApi/Functions.cs` (`Functions.Get`), with DI setup in `SimpleApi/Startup.cs`.
- Infra is SAM/CloudFormation in `SimpleApi/serverless.template`.

## Verified commands
- From repo root (`D:\2026\work\lambda\templates\RestApiLambda`): `dotnet build RestApiLambda.sln`.
- From repo root: `dotnet test RestApiLambda.sln` (currently no test project; this is effectively a restore/build check).
- Deploy command is run from `SimpleApi/`: `dotnet lambda deploy-serverless` (requires `Amazon.Lambda.Tools` installed separately).

## AWS/Lambda quirks
- `SimpleApi/serverless.template` is partially managed by `Amazon.Lambda.Annotations`; build-time sync can rewrite generated function resource details.
- Keep Lambda annotations and template runtime aligned (`net10.0` in csproj and `dotnet10` in template).
- `SimpleApi/aws-lambda-tools-defaults.json` has blank `profile` and `region`; deployment fails unless those are supplied via CLI flags or filled in.

## Doc mismatch to avoid
- `SimpleApi/Readme.md` is template boilerplate and references non-existent paths like `SimpleApi/src/SimpleApi` and `SimpleApi/test/SimpleApi.Tests`; use the commands above instead.
