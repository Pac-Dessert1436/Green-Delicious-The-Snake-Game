# Green Delicious - The Snake Game

![](title_screen.png)

## Description

This mini-game is a VB.NET WinForms project developed over three days. First uploaded to GitHub in March 2024, it has since undergone minor refinements, including the configuration of the `DoubleBuffered` property to improve rendering smoothness.

While the project's basic code skeleton was initially generated with AI assistance, the majority of the work (including feature enrichment and code refactoring) was completed independently. AI only contributed a small portion of the overall project.

Known Limitations:
- Background music and sound effects cannot play simultaneously, due to limitations of WinForms' built-in SoundPlayer class.
- No pause functionality or persistent high score saving is available.

Prerequisite: [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is required to compile and run the project.

## How to Play

1. Clone this repository and navigate to the project directory:
``` bash
git clone https://github.com/Pac-Dessert1436/Green-Delicious-The-Snake-Game
cd Green-Delicious-The-Snake-Game
```
2. Open the project in your preferred IDE, like Visual Studio 2022 or VS Code.
3. Build and run the project: Use the dotnet run command in the terminal, or click the "Run" button in your IDE.
4. Start the game: Press "Enter" to begin with background music, or "Space" to start with sound effects.
5. Gameplay: Control the snake using the arrow keys. Collect red apples to earn points. Occasional golden apples will grant double points.
6. Game Over: The game ends if the snake collides with the wall or bites its own body.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

