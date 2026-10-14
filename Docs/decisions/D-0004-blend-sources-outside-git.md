# D-0004: Exports in git, Blender sources outside it, no Git LFS

**Status:** Accepted (Norm, 2026-10-14) | **Date:** 2026-10-14 | **Area:** Process

## Context
Norm will make Blender models and icons. Neither earlier project used Git LFS; GitHub's free LFS quota is small.

## Decision
`.blend` files live in `ArtSource/` (git-ignored except its README) or anywhere outside the repo. Exported FBX
and PNG files go under `Assets/Art/` and are committed normally. No LFS.

## Because
Norm's choice among three options (LFS for everything, plain git for everything). Keeps the repo small and avoids
LFS quotas. Cost: ArtSource is not backed up by git, so Norm backs it up separately (ArtSource/README.md).
