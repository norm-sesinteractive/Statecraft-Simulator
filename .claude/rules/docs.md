---
paths:
  - "Docs/**"
  - "CLAUDE.md"
  - "README.md"
---

# Doc rules (full guide: Docs/DOCS_GUIDE.md)

- One owner per fact: link instead of restating. Keep each doc's "Owns" and "Budget" header lines.
- Never edit Docs/generated/ or Docs/decisions/INDEX.md by hand; regenerate with doctool.
- Absolute dates. Status markers: [PROPOSED], [PLANNED], [OPEN], [DEPRECATED].
- Attribute Norm's words. Docs/design/PITCH.md is frozen.
- ASCII punctuation, no em dashes.
- A new doc goes into the CLAUDE.md map in the same edit, with Owns and Budget lines.
- Check with `python Tools/docs/doctool.py lint`.
