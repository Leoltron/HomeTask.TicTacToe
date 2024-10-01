namespace HomeTask.TicTacToe.Models.Games;

public interface IGameRepository
{
    Game? FindGame(Guid gameId);

    Game CreateNewGame();

    Game? FindGameWithoutOpponent();
}