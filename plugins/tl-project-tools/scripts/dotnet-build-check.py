#!/usr/bin/env python3
"""PostToolUse check: build the project that owns an edited C# file and report compile errors.

Finds the nearest .csproj above the edited file (stops at src/), runs an incremental
`dotnet build --no-restore` on it and shows only errors. Exit 2 feeds the errors back to Claude.
If the project needs a restore (new package), the build fails with a restore error: run
`dotnet restore` yourself and retry.
"""

import json
import os
import subprocess
import sys

TIMEOUT_SECONDS = 180
MAX_ERROR_LINES = 15


def owning_project(path: str, project_dir: str):
    src_root = os.path.join(project_dir, "src")
    current = os.path.dirname(os.path.abspath(path))
    while current.startswith(src_root):
        for name in os.listdir(current) if os.path.isdir(current) else []:
            if name.endswith(".csproj"):
                return os.path.join(current, name)
        parent = os.path.dirname(current)
        if parent == current:
            break
        current = parent
    return None


def main() -> int:
    try:
        payload = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    path = (payload.get("tool_input", {}) or {}).get("file_path", "") or ""
    if not path.endswith((".cs", ".csproj")):
        return 0

    project_dir = os.environ.get("CLAUDE_PROJECT_DIR") or os.getcwd()
    if not os.path.abspath(path).startswith(os.path.join(project_dir, "src") + os.sep):
        return 0

    project = owning_project(path, project_dir)
    if project is None:
        return 0

    try:
        result = subprocess.run(
            ["dotnet", "build", project, "--no-restore", "-nologo", "-v", "q", "-clp:ErrorsOnly"],
            capture_output=True, text=True, timeout=TIMEOUT_SECONDS,
        )
    except subprocess.TimeoutExpired:
        print(f"dotnet build of {os.path.basename(project)} timed out after {TIMEOUT_SECONDS}s "
              "(not checked).", file=sys.stderr)
        return 0

    if result.returncode == 0:
        return 0

    lines = [ln for ln in (result.stdout + result.stderr).splitlines() if " error " in ln or "error:" in ln]
    lines = list(dict.fromkeys(lines))[:MAX_ERROR_LINES]
    print(f"Build failed for {os.path.basename(project)} after editing {path}:", file=sys.stderr)
    for ln in lines or ["(no error lines captured; run dotnet build for the full output)"]:
        print(f"  {ln}", file=sys.stderr)
    return 2


if __name__ == "__main__":
    sys.exit(main())
