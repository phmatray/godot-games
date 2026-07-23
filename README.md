![godot-games banner](.github/banner.png)

# Godot Games

<!-- portfolio-badges:start -->
<!-- Identity -->
[![phmatray - godot-games](https://img.shields.io/static/v1?label=phmatray&message=godot-games&color=blue&logo=github)](https://github.com/phmatray/godot-games)
![Top language](https://img.shields.io/github/languages/top/phmatray/godot-games)
[![Stars](https://img.shields.io/github/stars/phmatray/godot-games?style=social)](https://github.com/phmatray/godot-games/stargazers)
[![Forks](https://img.shields.io/github/forks/phmatray/godot-games?style=social)](https://github.com/phmatray/godot-games/network/members)
[![License](https://img.shields.io/github/license/phmatray/godot-games)](https://github.com/phmatray/godot-games/blob/HEAD/LICENSE)

<!-- Activity -->
[![Issues](https://img.shields.io/github/issues/phmatray/godot-games)](https://github.com/phmatray/godot-games/issues)
[![Pull requests](https://img.shields.io/github/issues-pr/phmatray/godot-games)](https://github.com/phmatray/godot-games/pulls)
[![Last commit](https://img.shields.io/github/last-commit/phmatray/godot-games)](https://github.com/phmatray/godot-games/commits)
<!-- portfolio-badges:end -->

<!-- portfolio-toc:start -->

## Table of Contents

- [Projects](#projects)
- [Features](#features)
- [Getting Started](#getting-started)
- [History](#history)
- [License](#license)

<!-- portfolio-toc:end -->



> A collection of **Godot 4 + .NET** games and engines — consolidated in one place
> (full git history preserved).

## Projects

| Path | What it is | From |
|---|---|---|
| [`lenia/`](lenia) | **Lenia** — a beautiful continuous cellular-automaton system in Godot 4 | `phmatray/lenia-godot` ★ |
| [`raga/`](raga) | A **gacha-style game** — Godot client + gRPC + server-authoritative .NET backend | `phmatray/Raga` |
| [`dragonfall/`](dragonfall) | A **procedural dungeon generator** — BSP partitioning, corridors, themed rooms | `phmatray/dragonfall` |
| [`platformer/`](platformer) | A **2D platformer** built with Godot 4.2 (Brackeys tutorial) | `phmatray/Platformer2D_Brackeys` |

## Features

- **Games and reusable engines** — Lenia and the dungeon generator are engines; Raga and the platformer are games
- **Godot 4 + C#/.NET** throughout
- **One home** — shared issues, discussions and history

## Getting Started

Open a project folder in **[Godot 4](https://godotengine.org/)** with .NET support, then build & run:

```bash
git clone https://github.com/phmatray/godot-games.git
cd godot-games/lenia   # or raga / dragonfall / platformer
```

## History

Each folder was merged with **full git history preserved** (`git subtree`). The
original repositories are archived and redirect here.

<!-- portfolio-techstack:start -->

## Tech Stack

- **.NET 10 · .NET 8**
- GdUnit4
- GdUnit4.Api
- ObservableCollections.R3
- R3
- Google.Protobuf
- Grpc.Net.Client
- Grpc.Tools
- Bogus

<!-- portfolio-techstack:end -->

<!-- portfolio-roadmap:start -->

## Roadmap

Planned work and known limitations are tracked in the [open issues](https://github.com/phmatray/godot-games/issues). Contributions toward them are welcome.

<!-- portfolio-roadmap:end -->

## License

MIT — see [`LICENSE`](LICENSE).
