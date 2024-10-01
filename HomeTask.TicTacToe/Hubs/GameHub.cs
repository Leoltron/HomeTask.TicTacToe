using System.Collections.Concurrent;
using HomeTask.TicTacToe.Models;
using HomeTask.TicTacToe.Models.Games;
using Microsoft.AspNetCore.SignalR;
namespace HomeTask.TicTacToe.Hubs;

public class GameHub(IGameListManager gameListManager, ILogger<GameHub> logger) : Hub
{
    private readonly ConcurrentDictionary<string, GameContext> connectionGameContext = new();

    public override async Task OnConnectedAsync()
    {
        var gameContext = GetGameContextOrAbort();
        if (gameContext == null)
        {
            return;
        }

        var gameIdString = gameContext.Game.Id.ToString();
        logger.LogInformation("Player {} (connection {}) connected to Game {}", gameContext.PlayerNumber, Context.ConnectionId, gameIdString);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameIdString);
        connectionGameContext.TryAdd(Context.ConnectionId, gameContext);
        await Clients.Group(gameContext.Game.Id.ToString()).SendAsync("PlayerConnected", gameContext.PlayerNumber);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Player (connection {}) disconnected", Context.ConnectionId);
        if (connectionGameContext.TryRemove(Context.ConnectionId, out var gameContext))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameContext.Game.Id.ToString());
            logger.LogInformation("Player {} (connection {}) removed from game {}", gameContext.PlayerNumber, Context.ConnectionId, gameContext.Game.Id.ToString());
            await Clients.Group(gameContext.Game.Id.ToString()).SendAsync("PlayerDisconnected", gameContext.PlayerNumber);
        }
    }

    public async Task SelectRole(string roleString)
    {
        var gameContext = GetGameContextOrAbort();
        if (gameContext == null)
        {
            return;
        }

        if (!Enum.TryParse<GameRole>(roleString, out var gameRole))
        {
            return;
        }

        if (gameContext.Game.TrySetRole(gameContext.IsPlayer1, gameRole))
        {
            await Clients.Group(gameContext.Game.Id.ToString()).SendAsync("Player1RoleSelected", gameRole.ToString());
            await OnGameStateUpdatedAsync(gameContext.Game.Id, gameContext.Game.State);
        }
    }

    public async Task MakeMove(int x, int y)
    {
        var gameContext = GetGameContextOrAbort();
        if (gameContext == null)
        {
            return;
        }

        if (x < 0 || x >= 3 || y < 0 || y >= 3)
        {
            return;
        }

        var role = gameContext.PlayerRole;
        if (role == null)
        {
            return;
        }

        var game = gameContext.Game;
        if (game.TryMakeAMove(role.Value, x, y))
        {
            await Clients.Group(game.Id.ToString()).SendAsync("PlayerMoved", role, x, y);
            await OnGameStateUpdatedAsync(game.Id, game.State);
        }        
    }

    private async Task OnGameStateUpdatedAsync(Guid gameId, GameState gameState)
    {
        await Clients.Group(gameId.ToString()).SendAsync("GameStateUpdated", gameState.ToString());
    }

    private GameContext? GetGameContextOrAbort()
    {
        var gameContext = gameListManager.FindCurrentGame();
        if (gameContext == null)
        {
            logger.LogError("Game not found");
            Context.Abort();
            return null;
        }
        return gameContext;
    }


    public async Task SendMessage(string user, string message)
    {
        var a = Context.GetHttpContext()!.Request.Cookies.TryGetValue("test", out var value);
        await Clients.All.SendAsync("ReceiveMessage", user + (a && !string.IsNullOrEmpty(value) ? " " + value : ""), message);
    }
}