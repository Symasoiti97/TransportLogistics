---
name: prompt-slim
description: Shrink Claude Code prompts (SKILL.md, .claude/rules/*.md, CLAUDE.md) without losing instructions. Use when asked to optimize a skill/rule/CLAUDE.md, reduce tokens, shorten a prompt, or clean up rules, or when such a file has grown. Triggers (RU): «оптимизируй скилл/правило/CLAUDE.md», «меньше токенов», «сократи промпт», «почисти правила».
---

# prompt-slim

Goal: the same instructions in fewer tokens. Every line must change model behavior. Remove everything else.

Write rules and skill bodies in English. Write the report to the user in Russian.

## Principles

- Loading is layered: `description` is always in context, the SKILL.md body loads only when triggered, `references/` load on demand. Keep `description` short and concrete; keep the body to the core procedure.
- The model already knows the basics. Remove "write clean code", "follow best practices", and explanations of common knowledge.
- Duplicates and stale facts are the main source of waste. Check that referenced files, functions, and flags exist.
- Keep structured content (commands, paths, JSON, code) as is. Compress only prose.
- Size targets: CLAUDE.md under ~200 lines; SKILL.md under ~100 lines; move long material to `references/*.md`, one level deep from SKILL.md.
- Scope rules with `paths:` frontmatter so they load only for matching files.

## Procedure

1. **Inventory.** Identify the files. Estimate tokens roughly (bytes / 4; undercounts Cyrillic, so treat Russian files as larger). Report a table: file, lines, approx tokens.
2. **Mark removals**, each with a one-line reason:
   - generic advice and obvious statements;
   - duplicates across files or within a file;
   - stale content: verify references with `grep`/`ls`; if the entity is missing, remove or fix it;
   - examples that repeat a rule without new information;
   - intro paragraphs and "why" sections longer than one line (keep only a non-obvious reason).
3. **Rewrite the rest:**
   - imperative mood: "Use X", not "It is recommended to use X in this project";
   - lists instead of paragraphs; one idea per line;
   - keep specifics: exact commands, paths, names, thresholds.
4. **Structure:** `description` with concrete triggers; body as procedure; long parts in `references/*.md` linked explicitly from SKILL.md.
5. **Check for loss.** For each removed or rewritten item, confirm it does not drop a constraint. If unsure, keep it and flag it, or ask.
6. **Report** (in Russian): before/after token table; removed items with reasons; what moved to references; open questions.

## Do not

- Remove security constraints, bans on dangerous actions, or commit/PR requirements, even if they look obvious.
- Auto-compress code, JSON, paths, or commands.
- Change the meaning of a rule without explicit approval.
- Rewrite files in a shared repo without showing a diff first. Show the diff, then apply after confirmation.
