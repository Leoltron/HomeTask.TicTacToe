namespace HomeTask.TicTacToe.Models.Games;

public class GameRepository : IGameRepository
{
    private readonly Dictionary<Guid, Game> games = new();

    public Game? FindGame(Guid gameId) => games.GetValueOrDefault(gameId);

    public Game? FindGameWithoutOpponent() => games.Values.Where(e => e.State == GameState.WaitingForOpponent).OrderByDescending(e => e.CreatedDate).FirstOrDefault();

    public Game CreateNewGame()
    {
        var newGame = new Game(Guid.NewGuid());
        games.Add(newGame.Id, newGame);
        return newGame;
    }
}