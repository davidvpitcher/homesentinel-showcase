# HomeSentinel engineering showcase

Technical portfolio showcasing HomeSentinel's C#/.NET architecture, testing, and local-first Windows application design.

**David Pitcher | Private review draft**

HomeSentinel is my in-development Windows security-visibility application. This repository presents a source-grounded case study and a small, runnable C# adaptation. It does **not** contain the full product, collectors, private evidence store or unreleased implementation plans.

## Try the synthetic example

With the .NET 10 SDK installed:

```bash
dotnet build tests/ReviewSample.Tests/ReviewSample.Tests.csproj -c Release
dotnet run --project tests/ReviewSample.Tests -c Release --no-build
dotnet run --project src/ReviewSample -c Release --no-build -- blockers
```

The example runs without administrator permissions, WPF, a database, network access or NuGet packages. It creates five fictional findings in memory and returns a deterministic, privacy-limited JSON projection. It does not inspect your computer.

The `blockers` view should contain finding IDs `3, 2, 5`, with five total findings, three visible findings and three total baseline-refresh blockers. An expected high-severity finding does not become a blocker solely because of its severity. Conversely, an unreviewed low-severity finding still needs review.

Other views: `all`, `unreviewed`, `reviewed`, `expected`. Unknown options return a nonzero exit.

## What this demonstrates

| Area | Evidence in this repository |
| --- | --- |
| Separation of responsibilities | Typed inputs, pure application projection and a minimal CLI. |
| Explicit review semantics | Review states and baseline-refresh blockers remain separate from severity. |
| Deterministic results | Stable severity/category/identity ordering without changing source rows. |
| Privacy by construction | Output DTOs exclude private evidence rather than trying to redact serialized objects afterward. |
| Testability | Dependency-free executable contract tests, boundary validation and cancellation. |
| Honest scope | Synthetic output is not presented as a real scan or proof of safety. |

## Read the case study

[Architecture and decisions](docs/CASE-STUDY.md) · [Provenance and differences](docs/PROVENANCE.md) · [Validation](docs/VALIDATION.md) · [Publication review](docs/PUBLICATION-REVIEW.md)

Start with `src/ReviewSample/ReviewProjection.cs`, then `tests/ReviewSample.Tests/Program.cs`. The test executable is a small assertion harness, not xUnit and not the source product's test suite.

## Original project context

The inspected source checkpoint documents a WPF GUI, CLI, Core, Collectors, Store and App layers, with SQLite persistence and owner review. Its README records an owner-reported Windows checkpoint of **805/805 tests passing** and a successful WPF launch. This repository did not rerun that original suite.

HomeSentinel focuses on one authorized Windows endpoint. Scans and bounded Activity Watch observations are not continuous security coverage. This showcase does not claim enterprise endpoint protection, automatic remediation, multi-endpoint collection or a new product release.

No real application screenshot is included yet. A future owner-approved demonstration can be added without publishing the complete source.

## Review status

The source repository remains private and unchanged. This adaptation also remains private pending owner approval. No reusable-source licence or public release has been approved. Technical walkthroughs can be arranged with selected material rather than unrestricted source access.
