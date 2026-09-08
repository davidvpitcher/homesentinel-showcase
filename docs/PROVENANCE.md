# Provenance and adaptation

Source basis: the owner's private HomeSentinel repository, inspected read-only at commit `cc6040772248d2f3ae8ee2f955d8f3300abfe76c`.

The README supplied the validated-checkpoint context and product scope. The inspected source tree supplied the component names. `FindingListFilter.cs` and `FindingListItem.cs` supplied review-filter semantics, ordering and the separation between review state and underlying evidence.

Preserved concepts: unreviewed/needs-review/needs-action block baseline refresh; review state and severity have different meanings; incompatible filtering choices are disallowed; stable severity/category/ID ordering does not mutate the input.

New showcase code: typed category/severity/review enums, a compact fixture model, positive output projection, CLI, bounds/cancellation/duplicate-ID checks, and a standalone assertion harness. The source uses richer string-based items and multiple boolean/status options; the sample narrows these to one typed mode and optional status. Unsupported enum values are rejected rather than inheriting the source's treatment of unknown strings. This is deliberately documented, not silently presented as identical behaviour.

Nothing here is a production collector or the product's complete export implementation. No real evidence or local state is copied. The five findings and private-evidence markers were invented specifically for demonstration. The adaptation was prepared with AI assistance and awaits owner review before publication.
