#!/usr/bin/env python3
"""Build and stage C# bundle runtime files into bundle/bin for pack_bundle.gd."""

from __future__ import annotations

import argparse
import json
import shutil
import subprocess
import sys
from pathlib import Path


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("bundles_root", type=Path, help="Path to project/bundles")
    parser.add_argument("--configuration", default="Debug")
    parser.add_argument("--framework", default="net8.0")
    return parser.parse_args()


def find_csproj(bundle_dir: Path) -> Path | None:
    projects = sorted(bundle_dir.glob("*.csproj"))
    return projects[0] if projects else None


def load_manifest(bundle_dir: Path) -> dict:
    manifest_path = bundle_dir / "manifest.json"
    if not manifest_path.exists():
        return {}
    return json.loads(manifest_path.read_text(encoding="utf-8"))


def build_project(csproj: Path, configuration: str, framework: str) -> None:
    cmd = [
        "dotnet",
        "build",
        str(csproj),
        "-c",
        configuration,
        "-f",
        framework,
    ]
    print(" ".join(cmd))
    subprocess.run(cmd, check=True)


def stage_bundle(bundle_dir: Path, entry_assembly: str, configuration: str, framework: str) -> None:
    source_dir = bundle_dir / "bin" / configuration / framework
    if not source_dir.exists():
        raise FileNotFoundError(f"Build output missing: {source_dir}")

    target_dir = bundle_dir / "bin"
    target_dir.mkdir(parents=True, exist_ok=True)

    for existing in target_dir.iterdir():
        if existing.is_file():
            existing.unlink()

    copied = 0
    for item in sorted(source_dir.iterdir()):
        if not item.is_file():
            continue
        if item.suffix.lower() not in {".dll", ".json", ".pdb"}:
            continue
        shutil.copy2(item, target_dir / item.name)
        copied += 1

    if not (target_dir / entry_assembly).exists():
        raise FileNotFoundError(
            f"entryAssembly '{entry_assembly}' was not staged for bundle '{bundle_dir.name}'"
        )

    print(f"staged {bundle_dir.name}: {copied} file(s) -> {target_dir}")


def main() -> int:
    args = parse_args()
    bundles_root = args.bundles_root
    if not bundles_root.exists():
        print(f"Bundles root not found: {bundles_root}", file=sys.stderr)
        return 1

    processed = 0
    for bundle_dir in sorted(p for p in bundles_root.iterdir() if p.is_dir()):
        manifest = load_manifest(bundle_dir)
        entry_assembly = manifest.get("entryAssembly")
        if not entry_assembly:
            continue

        csproj = find_csproj(bundle_dir)
        if csproj is None:
            print(
                f"Skipping {bundle_dir.name}: manifest has entryAssembly but no .csproj found",
                file=sys.stderr,
            )
            return 1

        build_project(csproj, args.configuration, args.framework)
        stage_bundle(bundle_dir, entry_assembly, args.configuration, args.framework)
        processed += 1

    print(f"processed {processed} C# bundle(s)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
