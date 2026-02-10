# Build and Verify Gates

## Rule

Agents MUST run `task build` and `task test` before committing. Code that doesn't build or has failing tests MUST NOT be committed.

### Gates

| Gate | Command | What Must Pass |
|------|---------|----------------|
| Build | `task build` | Zero errors, zero warnings (warnings-as-errors) |
| Test | `task test` | All xunit tests green |
| Verify | `task verify` | Godot headless loads, BootstrapShim prints init, clean exit |
| Format | `task format:check` | No formatting violations |
| Full | `task gate` | All of the above |

### Guidelines

- Run `task build` after every file change.
- Run `task test` before every commit.
- Run `task verify` when changing Godot host files or bootstrap.
- The `task gate` command runs everything — use before pushing.
- If a test fails, fix it before committing. Do not skip tests.
