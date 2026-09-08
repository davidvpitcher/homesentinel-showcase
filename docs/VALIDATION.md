# Validation

## Source project evidence

The source README records an owner-reported Windows checkpoint of 805/805 automated tests passing, zero failures/skips and successful WPF launch. That is historical source evidence, not a test result produced by this showcase or a new security audit.

## Verified showcase result

[GitHub Actions run 34176181569](https://github.com/davidvpitcher/homesentinel-showcase/actions/runs/34176181569) completed successfully for pull request #1 at head commit `b3966deb3889e8f4a7a74494cfec4ae7658c6493`. The run tested GitHub's proposed merge with the unchanged initial main branch.

Runner: Ubuntu 24.04; .NET SDK 10.0.400.

- Release build: passed; 0 compiler warnings, 0 errors.
- Executable contract tests: **30 passed, 0 failed**.
- CLI `blockers` demonstration: passed; total findings 5, visible findings 3, total baseline blockers 3, ordered finding IDs 3, 2, 5.
- The output contains only the selected fields, not synthetic private evidence.

The logs were inspected, not just the workflow status. GitHub emitted a checkout-action Node runtime deprecation warning; this did not fail the build or tests. No runtime downgrade or insecure compatibility flag was enabled.

## Reproduction and boundaries

Run the commands in the README. The test executable covers filtering, deterministic sorting, blocker semantics, input validation, cancellation, no mutation and the output-field projection. It exits nonzero on failure.

The local editing environment did not contain a .NET SDK and could not download one, so no local C# compilation or execution is claimed. The successful result above is from GitHub Actions.

No NuGet dependencies are used; `NuGet.Config` clears package sources. The workflow has read-only repository permission, no deployment step, and no production secrets or access to the private application. It validates this small cross-platform console adaptation, not the Windows GUI. Later revisions should be assessed using their own PR checks.
