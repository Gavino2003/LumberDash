# LumberDash

An endless runner built in Unity where you play as Groot — running through a forest, dodging obstacles, collecting coins, and staying ahead of an angry Lumberjack hot on your heels.

---

## Autor

| Nome | Número |
|------|--------|
| Vasco Filipe Ferraz Gavino | 32746 |

---

## Demo

https://github.com/user-attachments/assets/b15e62ef-0173-4a7d-bd4e-9bd28effa6ab

---

## Versão do Unity

**Unity 6000.3.9f1** (versão pedida no enunciado)

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
- Lumberjack AI that mirrors player movement with a delay
- Distance highscore tracked and saved between sessions
- Shop: spend 150 coins to double distance for the next run
- URP rendering with post-processing

---

## Como Abrir o Projeto

1. Instalar o **Unity Hub** e adicionar a versão **Unity 6000.3.9f1**
2. Clonar o repositório: `git clone https://github.com/Gavino2003/LumberDash.git`
3. No Unity Hub, clicar em **Add** → selecionar a pasta raiz do repositório
4. Abrir o projeto (Unity importa os assets automaticamente)
5. Na janela **Project**, abrir `Assets/Scenes/SampleScene.unity`
6. Clicar em **Play** e clicar com o rato para iniciar o jogo

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
├── Sprites/          # UI sprites and tutorial images
├── Scenes/           # Main game scene
└── Settings/         # URP render pipeline config
```

---

## Assets Multimédia

### Texturas

| Ficheiro | Resolução | Formato | Uso | Justificação |
|----------|-----------|---------|-----|--------------|
| `Groot_CO.png` | 2048×2048 | PNG RGB | Textura de cor do Groot | Resolução alta para manter qualidade no modelo principal do jogador |
| `Groot_1_NR.png` | 2048×2048 | PNG RGB | Normal map do Groot | Necessário para iluminação realista com URP |
| `Groot_ROU.png` | 2048×2048 | PNG RGB | Roughness/Occlusion do Groot | Necessário para o material PBR |
| `TXT_LumberJack.png` | 2048×2048 | PNG RGBA | Textura do Lumberjack | Resolução igual à do jogador para consistência visual |
| `tutorial-removebg-preview.png` | 500×500 | PNG RGBA | Imagem de tutorial (ecrã inicial) | Resolução suficiente para UI; RGBA para fundo transparente |
| `tutorial2-removebg-preview.png` | 500×500 | PNG RGBA | Imagem de tutorial (ecrã inicial) | Idem |
| `cooltext505956721932574.png` | 634×98 | PNG RGBA | Logo do jogo | Formato wide adequado ao texto; RGBA para transparência |

### Sons

| Ficheiro | Formato | Uso | Justificação |
|----------|---------|-----|--------------|
| `hard_battle_1_bpm170.mp3` | MP3 | Música de fundo | MP3 comprimido para reduzir tamanho; música em loop não exige lossless |
| `running.mp3` | MP3 | Footsteps em loop | Loop curto; MP3 adequado para SFX repetitivo |
| `jump.mp3` | MP3 | SFX de salto | Efeito curto; MP3 com qualidade suficiente para one-shot |
| `slide.mp3` | MP3 | SFX de roll | Idem |
| `hit.mp3` | MP3 | SFX de colisão frontal | Idem |
| `lateral_impact.mp3` | MP3 | SFX de colisão lateral | Idem |
| `ground impact.mp3` | MP3 | SFX de aterragem | Idem |
| `coin-collect.mp3` | MP3 | SFX de moeda | Idem |
| `ouch.mp3` | MP3 | SFX de dano | Idem |
| `screaming.mp3` | MP3 | SFX de game over | Idem |
| `suprised.mp3` | MP3 | SFX de surpresa | Idem |
| `mixkit-arrow-whoosh-1491.wav` | WAV | SFX de whoosh | WAV sem compressão para efeito de impacto rápido onde artifacts MP3 seriam audíveis |

---

## Built With

- **Unity 6000.3.9f1** — game engine
- **Universal Render Pipeline (URP)** — rendering
- **Unity Input System** — keyboard input
- **TextMesh Pro** — UI text
- **Simple Low Poly Nature** — environment assets
- **BOXOPHOBIC Skybox Cubemap Extended** — skybox
