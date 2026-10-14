# Statecraft Simulator (working title)

A mobile-first strategy simulation. You are a hidden society steering nations through their media,
schools, courts and politics; you never give an order, and the nations act on their own. Simple controls,
complex outcomes: shape the river and watch where it floods.

- Engine: Unity 6000.3.13f1 (URP), mobile first.
- All code is written through Claude Code; the documentation system keeps every session oriented.
- Start reading at [CLAUDE.md](CLAUDE.md), then [Docs/STATUS.md](Docs/STATUS.md).

Quick checks (from this folder):

```
dotnet test Tools/Headless/Statecraft.Headless.slnx --nologo -v q
dotnet run --project Tools/Headless/Runner -- run --script stoke-then-channel --years 80 --every 10
```

Open `Assets/Scenes/Main.unity` in the editor and press Play to see the developer HUD.
