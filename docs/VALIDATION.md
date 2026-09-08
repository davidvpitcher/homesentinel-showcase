# Validation

## Source project evidence

The source README records an owner-reported Windows checkpoint of 805/805 automated tests passing, zero failures/skips and successful WPF launch. That is historical source evidence, not a test result produced by this showcase or a new security audit.

## Showcase checks

The repository provides 30 named executable C# tests for filtering, deterministic sorting, blocker semantics, input validation, cancellation, no mutation and positive export projection. Run the commands in the README; the harness exits nonzero on failure.

The local editing environment did not contain a .NET SDK, and its network access could not retrieve one. Local C# compilation/execution is therefore **not claimed**. The included GitHub Actions workflow installs .NET 10 and runs the Release build, test executable and CLI demonstration. Consult the actual run/PR checks before calling remote validation successful.

No NuGet dependencies are used; `NuGet.Config` clears package sources. The workflow has read-only repository permission, no deployment step, and no production secrets or references to the private application. It validates this small cross-platform console adaptation, not the Windows GUI.
