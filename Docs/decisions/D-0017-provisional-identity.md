# D-0017: Provisional identity, Android settings and orientation

**Status:** Proposed | **Date:** 2026-10-14 | **Area:** Platform

## Context
Mobile first (D-0002) needs player settings from the start. InsignificAnts uses company "SES Interactive", bundle
id `com.SESInteractive.InsignificAnts`, IL2CPP and minimum API 26.

## Decision
`ProjectSetup` sets: company "SES Interactive", product "Statecraft Simulator", bundle id
`com.SESInteractive.StatecraftSimulator` (Android and iOS), IL2CPP, ARM64 only, minimum API 26, landscape only
(either way up). Keystores stay outside the repo.

## Because
Mirrors Norm's working Android setup. ARM64-only is what Google Play requires and keeps builds lean. Landscape
suits a world map. Bundle ids are permanent once published, so Norm confirms before the first store build (Q-012).
