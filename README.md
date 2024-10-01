# Tic Tac Toe

## How to launch:

From folder HomeTask.TicTacToe run `dotnet run`. You need to have [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed. Both players need to be on different browsers since game uses cookies for state persistence.

## How it works:

### Project 
This is a ASP.NET Core project. Initial page is served via MVC model with Bootstrap CSS framework and mostly vanilla JavaScript. For game itself SignalR is used, which includes built into ASP.NET Core implementation on backend and JS library on frontend.

### Structure
Game class handles the state of a single game. Same class handles changes made to the game (assigment of roles and making moves). GameRepository handles the game storage and while current implementation is in-memory, it can be rewritten to any database implementation for persistent storage. GameListManager handles game lookup and creation. 

### Flow
When new player joins, server tries to find a game waiting for an opponent. The latest one is picked, if several are avaiilable. If there is no such game, a new one is created with state "WaitingForOpponent" and its ID is saved into "GameId" cookie along with "PlayerNumber" 1. When next player joins and finds this game as an awaiting game, they will join it in a similar way but with cookie "PlayerNumber" = 2. After loading the page, players connect to the game server via a SignalR managed WebSocket connection. When both players are present, game shifts into state "PickingRole". Both players can then pick "X" or "O". First player can pick a role even before second player joins. When one of the players pick a role, a second player's role is automatically picked and game starts. During gameplay, game alternates between XTurn and OTurn states, and as players make their moves, they send messages to the WS server, and it handles them, chancghing game state and informing players of new board and game state.

### Game ending
Game ending condition is checked after every move. When any of the columns, rows or diagonals are filled with the same figure (X or O), game ends with their win. Otherwize, if no further moves can be made, game ends in a draw.

