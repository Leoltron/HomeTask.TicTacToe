namespace HomeTask.TicTacToe.Models.Games;

public class GameListManager(IGameRepository gameRepository, IHttpContextAccessor httpContextAccessor, ILogger<GameListManager> logger) : IGameListManager
{
    public GameContext? FindCurrentGame()
    {
        var gameIdCookie = HttpContext.Request.Cookies[Constants.Cookies.GameId];
        var playerNumberCookie = HttpContext.Request.Cookies[Constants.Cookies.PlayerNumber];
        if (!Guid.TryParse(gameIdCookie, out var gameId) || int.TryParse(playerNumberCookie, out var playerNumber) && playerNumber != 1 && playerNumber != 2)
        {
            return null;
        }

        var game = gameRepository.FindGame(gameId);
        if (game == null || !game.State.IsActive())
        {
            return null;
        }

        return new GameContext
        {
            Game = game,
            IsPlayer1 = playerNumber == 1,
        };
    }

    private GameContext? FindAndEnterAGameWithoutOpponent()
    {
        const int maxTries = 10;
        Game? game = null;
        for (var tries = 0; tries < maxTries; tries++)
        {
            game = gameRepository.FindGameWithoutOpponent();
            if (game == null)
            {
                break;
            }

            if (game.OnSecondPlayerConnected())
            {
                break;
            }
        }
        if (game == null)
        {
            return null;
        }

        logger.LogInformation("Second player joined game {}", game.Id);
        HttpContext.Response.Cookies.Append(Constants.Cookies.GameId, game.Id.ToString());
        HttpContext.Response.Cookies.Append(Constants.Cookies.PlayerNumber, "2");

        return new GameContext
        {
            Game = game,
            IsPlayer1 = false
        };
    }

    public GameContext FindOrCreateActiveGame() => FindCurrentGame() ?? FindAndEnterAGameWithoutOpponent() ?? CreateNewGame();

    private GameContext CreateNewGame()
    {

        var newGame = gameRepository.CreateNewGame();
        logger.LogInformation("Created new game {}", newGame.Id);
        HttpContext.Response.Cookies.Append(Constants.Cookies.GameId, newGame.Id.ToString());
        HttpContext.Response.Cookies.Append(Constants.Cookies.PlayerNumber, "1");

        return new GameContext
        {
            Game = newGame,
            IsPlayer1 = true
        };
    }

    private HttpContext HttpContext => httpContextAccessor.HttpContext!;

}