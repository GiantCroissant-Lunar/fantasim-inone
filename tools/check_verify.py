"""Validate visual verification artifacts from --verify run."""
import json
import os
import sys


def check(verify_dir: str) -> bool:
    ok = True

    # 1. Check screenshot exists and is non-empty
    screenshot = os.path.join(verify_dir, "screenshot.png")
    if not os.path.isfile(screenshot):
        print(f"FAIL: {screenshot} not found")
        ok = False
    elif os.path.getsize(screenshot) < 1000:
        print(f"FAIL: {screenshot} is too small ({os.path.getsize(screenshot)} bytes)")
        ok = False
    else:
        print(f"OK: screenshot.png ({os.path.getsize(screenshot)} bytes)")

    # 2. Check scene_tree.json has expected structure
    scene_tree_path = os.path.join(verify_dir, "scene_tree.json")
    if not os.path.isfile(scene_tree_path):
        print(f"FAIL: {scene_tree_path} not found")
        ok = False
    else:
        with open(scene_tree_path) as f:
            tree = json.load(f)

        # Walk tree looking for expected node types
        found = {"MenuBar": False, "TabContainer": False, "Label": False}

        def walk(node):
            node_type = node.get("type", "")
            for expected in found:
                if node_type == expected:
                    found[expected] = True
            for child in node.get("children", []):
                walk(child)

        walk(tree)

        for node_type, present in found.items():
            if present:
                print(f"OK: scene_tree contains {node_type}")
            else:
                print(f"FAIL: scene_tree missing {node_type}")
                ok = False

    # 3. Check snapshot.json has bundles
    snapshot_path = os.path.join(verify_dir, "snapshot.json")
    if not os.path.isfile(snapshot_path):
        print(f"FAIL: {snapshot_path} not found")
        ok = False
    else:
        with open(snapshot_path) as f:
            snapshot = json.load(f)

        bundles = snapshot.get("Bundles", [])
        bundle_ids = [b["Id"] for b in bundles]
        print(f"OK: snapshot has {len(bundles)} bundles: {bundle_ids}")

        expected_bundles = [
            "com.fantasim.hud_shell",
            "com.fantasim.hud_filemenu",
        ]
        for bid in expected_bundles:
            if bid in bundle_ids:
                print(f"OK: bundle '{bid}' present")
            else:
                print(f"FAIL: bundle '{bid}' missing")
                ok = False

    return ok


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: check_verify.py <verify_dir>")
        sys.exit(1)

    verify_dir = sys.argv[1]
    if not os.path.isdir(verify_dir):
        print(f"FAIL: directory {verify_dir} does not exist")
        sys.exit(1)

    if check(verify_dir):
        print("\nAll checks passed")
        sys.exit(0)
    else:
        print("\nSome checks failed")
        sys.exit(1)
