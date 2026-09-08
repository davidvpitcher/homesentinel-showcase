# Publication and maintenance checklist

Merging a pull request does not change repository visibility or grant reuse rights. The owner reviews the exact material and controls publication separately.

Before initial publication or adding material from the private application:

- Check that descriptions distinguish the full Windows application from this console adaptation and its synthetic fixtures.
- Review tracked files and reachable history, plus pull-request discussions, workflow logs and artifacts. Public visibility exposes more than the current file tree.
- Exclude local evidence, database files, machine identifiers, private configuration, unpublished implementation plans and source-project history.
- Keep the original 805-test milestone separate from the showcase's 30 tests. Verify the checks for the exact proposed revision and the post-merge run on `main`.
- Review any future screenshots or recordings for private evidence before adding them. Do not present synthetic console output as the actual WPF interface.
- Make a separate licensing decision. No open-source licence is added by this checklist or the preparation changes.

Only `homesentinel-showcase` is a candidate for public visibility. The full HomeSentinel repository remains private and must not be changed as part of publication.

This checklist is a review aid, not an assertion of exhaustive historical secret scanning or a security certification. A repository visibility change and permission to reuse its source are separate decisions.
