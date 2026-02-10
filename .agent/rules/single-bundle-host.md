# Single BundleHost

## Rule

There is ONE BundleHost implementation at `project/plugins/Fantasim.App.Bundles.Core/BundleHost.cs`. Agents MUST NOT create a second BundleHost, split the lifecycle across multiple classes, or add alternative bundle loading mechanisms.

### Why

fantasim-app-godot accumulated 3 competing BundleHost implementations because each agent added its own. This caused broken load/unload lifecycle, leaked services, and config passing failures. One implementation, tested, is the only acceptable state.

### Guidelines

- All bundle lifecycle (Load, Unload, Reload) goes through `BundleHost`.
- Service modules are invoked **inside** LoadAsync/UnloadAsync, not as a separate manual step.
- The `IGodotBundleVfs` abstraction keeps BundleHost testable without Godot.
- Only `GodotBundleVfs` (in the host project) depends on Godot — everything else is pure .NET.

### Anti-Patterns

- Creating a "SimpleBundleLoader" or "BundleManager" alongside BundleHost.
- Moving service module invocation out of the load/unload lifecycle.
- Adding Godot dependencies to the Contracts or Core projects.
