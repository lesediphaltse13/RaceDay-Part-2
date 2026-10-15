# GitHub Actions CI Pipeline Guide

## Workflow
The repository's `.github/workflows/part2-ci.yml` defines the **RaceDay Part 2 CI** workflow. It runs on pushes to `main` and pull requests targeting `main`.

## Job stages
1. Check out the repository with `actions/checkout@v4`.
2. Install the .NET 8 SDK using `actions/setup-dotnet@v4`.
3. Restore packages using `dotnet restore RaceDay.sln`.
4. Build the solution in Release mode with `dotnet build RaceDay.sln --no-restore --configuration Release`.
5. Run tests with `dotnet test RaceDay.sln --no-build --configuration Release --logger "console;verbosity=normal"`.

## JWT settings
The workflow supplies a disposable test-only `Jwt__Key`, `Jwt__Issuer` and `Jwt__Audience` as environment variables. These are not production credentials. Never place real secrets in the workflow YAML.

## Interpreting results
| Indicator | Meaning | Next action |
| --- | --- | --- |
| Green check | All required job steps completed | Open the run and verify test output |
| Red X | One or more steps failed | Inspect failing step logs |
| Yellow/in progress | Run not finished | Wait before reporting outcome |
| Skipped | Step/job did not execute | Inspect conditions and triggers |

## Troubleshooting
- Restore failure: inspect NuGet version and SDK compatibility.
- Build failure: inspect compiler errors and changed source files.
- Test failure: inspect the failing test name, status assertion and JWT environment variables.
- Workflow not triggered: confirm the commit is on `main` and the workflow path is correct.

## Evidence checklist
Capture the commit SHA, run URL, workflow status and test summary. Do not describe a newly pushed commit as passing until GitHub Actions reports success.
