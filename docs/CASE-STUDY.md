# Case study: review context without implicit approval

## Product and architecture

HomeSentinel is a local-first Windows security-visibility application. The inspected source separates Core models/rules, Collectors, SQLite-backed Store, App services, a command-line interface and WPF presentation. The current project notes describe six state-baseline lanes: network, access, scheduled tasks, Windows services, startup registry and firewall.

The product deliberately separates observed state, review decisions and the trust attached to a baseline. Scans are observations, not proof of uninterrupted coverage or safety. The source README also records read-only history queries and preservation of current finding identities. These statements describe the inspected checkpoint, not promised future capabilities.

## Selected implementation idea

The source finding filter accepts a list and a review-filter selection. It prevents incompatible review filters, determines baseline-refresh blocking from review state, and orders output by severity, category and finding ID. This is a useful small unit of application logic: it is meaningful to users, does not need Windows to run, and can be tested separately from storage or presentation.

The showcase isolates those semantics in a pure `ReviewProjection` with immutable records and a small CLI. A single input batch represents one synthetic view; the example does not resolve historical finding identities, read a real database or recalculate security findings.

## Decisions demonstrated

**Review state is distinct from severity.** Unreviewed, needs-review and needs-action findings are blockers. Expected and false-positive findings are not. That is a workflow rule, not a statement that an expected finding is objectively safe.

**Filtering is not mutation.** Producing a reviewed-only or blockers-only view does not change a finding, its review state or a baseline. Totals are labelled separately: total findings and total blockers describe the input; visible findings describes the chosen view.

**Ordering is reproducible.** Severity is descending, followed by case-insensitive category label and finding ID. Tests also reverse the input order and check that the result stays the same.

**Private evidence is not an export field.** Synthetic source objects contain a private-evidence marker. Export DTOs have only the deliberately selected fields. Tests verify that neither the marker nor the private field name appears in serialized output. This is a fixed schema, not a general-purpose sanitizer for arbitrary real evidence.

**Invalid input is explicit.** The sample uses typed enums, unique positive IDs, a bounded input size and cancellation. Unsupported values are rejected rather than turned into an apparently complete result. This strict typed boundary is a showcase adaptation, not a claim that the original string-based filter had identical validation.

## What has been intentionally omitted

No Windows collectors, registry/service operations, SQLite schema, private database, owner notes, AI-provider configuration or baseline-mutation implementation is copied. There is no WPF screen in this runnable sample and no attempt to reproduce the full product through synthetic screenshots.

The test executable uses simple assertions so a reviewer needs only the .NET SDK. This reduces setup friction; it is not a recommendation to replace a larger project's normal test framework. The original 805-test milestone and this sample's tests are distinct evidence.
