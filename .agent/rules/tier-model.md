# Tier Model (T1–T4)

## Rule

The codebase follows a strict tier model. Contracts MUST NOT reference implementations.

### Tiers

| Tier | Directory | Description | Dependencies |
|------|-----------|-------------|--------------|
| T1 | `project/contracts/` | Interfaces, records, value types | None (pure .NET) |
| T2 | `project/schemas/` | Wire formats, serialization | T1 only |
| T3 | `project/plugins/` | Implementations | T1, T2 |
| T4 | `project/hosts/` | Composition roots, Godot entry | T1, T2, T3 |

### Guidelines

- **T1 contracts** define the "what" — interfaces, events, entities. Zero implementation logic.
- **T3 plugins** implement T1 interfaces. They never reference other T3 projects directly.
- **T4 hosts** compose everything. This is where DI/service wiring happens.
- Bundles (`project/bundles/`) are independently deployable T3 units that reference T1 contracts.

### Validation

A contract project referencing a plugin project is a build error (by design — the csproj references enforce this).
