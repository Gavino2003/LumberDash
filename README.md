# LumberDash

An endless runner built in Unity where you play as Groot — running through a forest, dodging obstacles, collecting coins, and staying ahead of an angry Lumberjack hot on your heels.

---

## Demo

<!-- Para adicionar o vídeo: faz upload no GitHub (arrasta para um Issue/PR) e substitui o link abaixo -->
> 🎥 *Vídeo em breve — arrasta um clip para um Issue do GitHub para obteres o link e cola aqui.*

<!--
[![Watch the demo](https://img.youtube.com/vi/VIDEO_ID/maxresdefault.jpg)](https://www.youtube.com/watch?v=VIDEO_ID)
-->

---

## Gameplay

- **3 lanes** — switch left and right to avoid obstacles
- **Jump** — leap over barriers
- **Roll** — slide under low obstacles
- **Coins** — collect them along the way
- **Speed increases** over time, and so does the difficulty
- **The Lumberjack** chases you — stumble and he closes in fast

### Controls

| Action | Key |
|--------|-----|
| Move Left | ← Arrow |
| Move Right | → Arrow |
| Jump | ↑ Arrow / Space |
| Roll | ↓ Arrow |
| Start | Left Click |

---

## Features

- Procedural obstacle spawning with easy / medium / hard patterns
- Coin arcs, straight lines and zigzag patterns in free lanes
- Cinematic intro camera sequence on game start
- Full audio system — music, footsteps, jump, roll, hits, game over
- Auto-reset after game over with full state cleanup
- Tutorial screen with pulse animation on the start screen
- URP rendering with post-processing

---

## Project Structure

```
Assets/
├── Scripts/          # All game logic
│   ├── PlayerMovement.cs
│   ├── PlayerCollision.cs
│   ├── GameManager.cs
│   ├── AudioManager.cs
│   ├── CameraManager.cs
│   ├── CameraFollow.cs
│   ├── Chaser.cs
│   ├── CoinBehaviour.cs
│   ├── ObstacleSpawner.cs
│   ├── ObstacleMovement.cs
│   ├── InfiniteGround.cs
│   └── SideWalls.cs
├── caracters/        # Groot and Lumberjack models & animations
├── prefabs/          # Player, obstacles, coins, ground
├── Sounds/           # All audio files
├── Scenes/           # Main game scene
└── Settings/         # URP render pipeline config
```

---

## Built With

- **Unity 6** — game engine
- **Universal Render Pipeline (URP)** — rendering
- **Unity Input System** — keyboard input
- **TextMesh Pro** — UI text
- **Simple Low Poly Nature** — environment assets
- **BOXOPHOBIC Skybox Cubemap Extended** — skybox

---

## Getting Started

1. Clone the repository
2. Open the project in **Unity 6** or later
3. Open `Assets/Scenes/SampleScene.unity`
4. Press **Play** and click to start
