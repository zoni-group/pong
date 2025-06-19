# Zoni Pong

A lightweight Pong clone built with C# WinForms. This simple game demonstrates basic game loop logic, sprite movement, collision detection, and custom rendering in a Windows Forms application.

---

## Features

- **Two-player local multiplayer**: Control the left paddle with `W`/`S` keys and the right paddle with the `Up`/`Down` arrow keys.
- **Ball physics**: Bounces off top/bottom edges and paddles.
- **Scoring system**: Tracks and displays scores for both players.
- **Resizable window**: Automatically recenters paddles and ball on window resize.
- **Optimized rendering**: Uses double-buffering to reduce flicker.

---

## Prerequisites

- Windows OS
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- (Optional) Visual Studio 2022/2023 or Visual Studio Code with C# extensions

---

## Getting Started

1. **Clone the repository** (assuming remote is set up):

   ```bash
   git clone [https://github.com/yourusername/zoni-pong.git](https://github.com/zoni-group/pong.git)
   cd zoni-pong
   ```

2. **Restore and build**:

   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the game**:

   ```bash
   dotnet run --project zgame.csproj
   ```

   Or open the `.sln` in Visual Studio and press **F5**.

---

## Controls

- **Left Paddle**: `W` (up), `S` (down)
- **Right Paddle**: `Up Arrow` (up), `Down Arrow` (down)
- **Resize Window**: Paddles and ball will automatically recenter.

---

## Project Structure

```
├── Program.cs          # Main form and game logic
├── zgame.csproj        # Project file
├── zgame.sln           # Solution file
├── bin/                # Build output (ignored by Git)
├── obj/                # Temporary build files (ignored by Git)
└── .gitignore          # Recommended ignore rules for C# WinForms
```

---

## Customization

- **Ball speed**: Adjust `_ballVelX` and `_ballVelY` constants in `Program.cs`.
- **Paddle speed/size**: Modify `PaddleWidth`, `PaddleHeight`, and movement increment values.
- **Frame rate**: Tweak `_gameTimer.Interval` for faster or slower tick updates.

---

## License

© 2025 ZONI®. All rights reserved.
