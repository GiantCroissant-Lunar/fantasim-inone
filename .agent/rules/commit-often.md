# Commit Often

## Rule

Agents MUST commit early and often throughout a development session.

### Guidelines

- **Commit after each logical unit of work** — do not batch unrelated changes into a single commit.
- **Commit before switching context** — if you are about to start a different task, commit the current work first.
- **Commit working states** — even if the feature is incomplete, commit when the code builds and tests pass.
- **Small commits are better than large ones** — a series of small, focused commits is easier to review, revert, and understand.
- **Use conventional commit messages** — follow the format: `type(scope): description`
  - `feat:` new feature
  - `fix:` bug fix
  - `docs:` documentation only
  - `refactor:` code change that neither fixes a bug nor adds a feature
  - `test:` adding or updating tests
  - `chore:` maintenance, tooling, configuration

### Anti-Patterns

- Accumulating dozens of changed files before committing.
- Committing only at the end of a session.
- Mixing unrelated changes in one commit.
