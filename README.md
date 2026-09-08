# HomeSentinel engineering showcase

**David Pitcher | Software Development Portfolio**

HomeSentinel is my in-development, local-first Windows security-visibility application. This repository presents its engineering decisions through a case study and a runnable C#/.NET example. The full application, Windows collectors and private evidence store remain private.

## What this sample demonstrates

| Engineering concern | Implementation |
| --- | --- |
| Separate evidence from decisions | Severity and review status have distinct meanings; filtering does not approve findings or change a baseline. |
| Deterministic application logic | Stable severity/category/identity ordering, typed filters and explicit validation. |
| Limited output | Response records contain only selected fields, excluding the synthetic private-evidence field. |
| Testable boundaries | Cancellation, duplicate identities, invalid inputs and no-mutation checks. |

Start with [ReviewProjection.cs](src/ReviewSample/ReviewProjection.cs), then the [30 executable contract tests](tests/ReviewSample.Tests/Program.cs). The [case study](docs/CASE-STUDY.md) explains the decisions and trade-offs.

## Run the synthetic example

With the **.NET 10 SDK** installed:

```bash
dotnet build tests/ReviewSample.Tests/ReviewSample.Tests.csproj -c Release
dotnet run --project tests/ReviewSample.Tests -c Release --no-build
dotnet run --project src/ReviewSample -c Release --no-build -- blockers
```

This console example needs no administrator permissions, WPF, database, network connection or NuGet packages. It creates five fictional findings in memory and prints a JSON projection. **It does not inspect or modify your computer.**

The `blockers` view returns finding IDs `3, 2, 5`: five total findings, three visible findings and three total baseline-refresh blockers. A high-severity finding marked expected is not automatically a blocker, while an unreviewed low-severity finding still needs review. Review state is not proof of safety.

Other views: `all`, `unreviewed`, `reviewed`, `expected`. Unknown options return a nonzero exit. The test executable uses a small dependency-free assertion harness, not xUnit or the source application's test suite.

## How it relates to the full application

The inspected HomeSentinel checkpoint separates Core, Collectors, Store, App, CLI and WPF presentation, with SQLite persistence and owner review. Its documentation records an owner-reported Windows milestone of **805/805 tests passing** and a successful WPF launch. That historical result is separate from the 30 showcase tests; this repository does not rerun or distribute the original suite.

The current product focuses on one authorized Windows endpoint. Scans and bounded observations are not continuous security coverage. This sample demonstrates application logic, not the full WPF interface, endpoint protection or automatic remediation.

## Read more

[Architecture and decisions](docs/CASE-STUDY.md) · [Provenance and adaptation](docs/PROVENANCE.md) · [Validation evidence](docs/VALIDATION.md) · [Validation workflow](https://github.com/davidvpitcher/homesentinel-showcase/actions/workflows/validate.yml)

No open-source licence has been granted. Contact the repository owner about reuse. The [publication and maintenance checklist](docs/PUBLICATION-REVIEW.md) describes the review required before changing visibility or adding source material. Selected technical walkthroughs can be arranged without unrestricted access to the private application.
