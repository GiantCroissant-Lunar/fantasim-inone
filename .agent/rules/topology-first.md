# Topology-First Doctrine

## Rule

The boundary graph is truth. Cells, meshes, and spatial substrates are derived.

### Why

Per RFC-V2-0001: plate topology (plates, boundaries, junctions) is the authoritative truth slice. Any cell-based, Voronoi, or DGGS representation is a derived sampling product and MUST remain recomputable from topology events.

### Guidelines

- **Plates** are identified by `PlateId`, not by cell assignments.
- **Boundaries** separate exactly two plates. They have type (divergent/convergent/transform).
- **Junctions** are where boundaries meet.
- **Events** define truth. The materialized graph is a derived read model, but deterministically replayable.
- **Invariants** are enforced during materialization: no orphan boundaries, no self-referencing boundaries.

### Anti-Patterns

- Treating a cell-to-plate assignment as authoritative truth.
- Creating plate entities that depend on a specific mesh resolution.
- Storing derived spatial data in the event store.
- Bypassing the materializer to directly mutate topology state.
