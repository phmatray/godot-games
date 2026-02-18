# dragonfall — Procedural dungeon generation for Godot

A procedural dungeon generation engine for Godot games, implementing BSP (Binary Space Partitioning) tree algorithms to create randomized dungeons with rooms, corridors, and themed environments.

## ✨ Features
- BSP tree dungeon generation algorithm
- Corridor carving between rooms
- Multiple dungeon themes and biomes
- Seeded generation for reproducible dungeons
- Fully unit-tested generation pipeline
- Godot C# integration

## 📦 Installation
```bash
git clone https://github.com/phmatray/dragonfall
cd dragonfall
dotnet test Dragonfall.Tests
# Import into Godot as a C# project
```

## 🚀 Quick Start
```csharp
var generator = new DungeonGenerator(seed: 42);
var dungeon = generator.Generate(width: 80, height: 50);
// Dungeon contains rooms, corridors, and themed areas
foreach (var room in dungeon.Rooms)
    Console.WriteLine($"Room at {room.X},{room.Y}");
```

## 📄 License
MIT — see LICENSE
