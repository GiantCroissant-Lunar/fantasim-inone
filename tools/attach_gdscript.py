#!/usr/bin/env python3
"""Post-process a .tscn file to inject a GDScript ext_resource on the root node.

Usage:
    python attach_gdscript.py <tscn_path> <gdscript_res_path>

Example:
    python attach_gdscript.py scenes/toolbar.tscn "res://bundles/com.fantasim.hud_toolbar/scripts/toolbar.gd"
"""

import re
import sys


def attach_gdscript(tscn_path: str, gdscript_res_path: str) -> None:
    with open(tscn_path, "r", encoding="utf-8") as f:
        content = f.read()

    # Increment load_steps in [gd_scene] header
    header_match = re.search(r"\[gd_scene load_steps=(\d+)", content)
    if header_match:
        old_steps = int(header_match.group(1))
        content = content.replace(
            f"load_steps={old_steps}",
            f"load_steps={old_steps + 1}",
            1,
        )
    else:
        # No load_steps yet — add it
        content = content.replace(
            "[gd_scene ",
            "[gd_scene load_steps=2 ",
            1,
        )

    # Find insertion point: after the [gd_scene ...] header line
    header_end = content.index("]") + 1
    # Skip past any trailing newline
    if header_end < len(content) and content[header_end] == "\n":
        header_end += 1

    ext_resource = (
        f'\n[ext_resource type="Script" path="{gdscript_res_path}" id="1_script"]\n'
    )
    content = content[:header_end] + ext_resource + content[header_end:]

    # Attach script to the root [node] block
    # The root node is the first [node ...] without a parent attribute
    root_node_match = re.search(r"(\[node [^\]]*\])\n", content)
    if root_node_match:
        root_line = root_node_match.group(0)
        content = content.replace(
            root_line,
            root_line + 'script = ExtResource("1_script")\n',
            1,
        )

    with open(tscn_path, "w", encoding="utf-8") as f:
        f.write(content)

    print(f"Attached {gdscript_res_path} to {tscn_path}")


if __name__ == "__main__":
    if len(sys.argv) != 3:
        print(f"Usage: {sys.argv[0]} <tscn_path> <gdscript_res_path>", file=sys.stderr)
        sys.exit(1)

    attach_gdscript(sys.argv[1], sys.argv[2])
