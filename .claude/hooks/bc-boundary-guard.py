#!/usr/bin/env python3
"""PreToolUse guard: block edits that add a reference from one bounded context to another.

Rule: .claude/rules/architecture.md — BCs (Tariff, Identity, Locations) must not reference each other.
Reads the hook JSON from stdin. Exit 2 blocks the tool call and shows the message to Claude.
"""

import json
import re
import sys

# Area is detected by path prefix (relative to the project root).
AREAS = {
    "tariff": "src/TransportLogistics/Tariff/",
    "identity": "src/Socials/Identity/",
    "locations": "src/Locations/",
}

# Code: substrings that point into another BC (namespaces or paths).
FORBIDDEN_CODE = {
    "tariff": ["TL.Socials", "Socials/Identity", "Socials\\Identity", "TL.Locations", "Locations/Locations", "Locations\\Locations"],
    "identity": ["TL.TransportLogistics", "TransportLogistics/Tariff", "TransportLogistics\\Tariff", "TL.Locations", "Locations/Locations", "Locations\\Locations"],
    "locations": ["TL.TransportLogistics", "TL.Socials", "TransportLogistics/Tariff", "TransportLogistics\\Tariff", "Socials/Identity", "Socials\\Identity"],
}

# Project references: folder names that belong to another BC.
FORBIDDEN_FOLDERS = {
    "tariff": {"Socials", "Locations"},
    "identity": {"TransportLogistics", "Locations"},
    "locations": {"TransportLogistics", "Socials"},
}

PROJECT_REF = re.compile(r'ProjectReference\s+Include="([^"]+)"')


def area_of(path: str):
    normalized = path.replace("\\", "/")
    for name, prefix in AREAS.items():
        if prefix in normalized or normalized.startswith(prefix.rstrip("/")):
            return name
    return None


def new_text(tool_input: dict) -> str:
    """Text the tool is about to write, for Write, Edit and MultiEdit."""
    parts = []
    if "content" in tool_input:
        parts.append(tool_input["content"] or "")
    if "new_string" in tool_input:
        parts.append(tool_input["new_string"] or "")
    for edit in tool_input.get("edits", []) or []:
        parts.append(edit.get("new_string", "") or "")
    return "\n".join(parts)


def violations(path: str, text: str, area: str):
    found = []
    if path.endswith(".csproj"):
        for ref in PROJECT_REF.findall(text):
            segments = set(re.split(r"[\\/]", ref))
            for folder in FORBIDDEN_FOLDERS[area] & segments:
                found.append(f"ProjectReference '{ref}' points into another BC folder '{folder}'")
    elif path.endswith(".cs"):
        for token in FORBIDDEN_CODE[area]:
            if token in text:
                found.append(f"'{token}' refers to another bounded context")
    return found


def main() -> int:
    try:
        payload = json.load(sys.stdin)
    except json.JSONDecodeError:
        return 0

    tool_input = payload.get("tool_input", {}) or {}
    path = tool_input.get("file_path", "") or ""
    area = area_of(path)
    if area is None:
        return 0

    problems = violations(path, new_text(tool_input), area)
    if not problems:
        return 0

    print(f"BC boundary violation in {path} ({area} context):", file=sys.stderr)
    for p in problems:
        print(f"  - {p}", file=sys.stderr)
    print("BCs must not reference each other (.claude/rules/architecture.md). "
          "Go through SharedKernel or an Application-layer interface of the same BC.", file=sys.stderr)
    return 2


if __name__ == "__main__":
    sys.exit(main())
