namespace HomeTask.TicTacToe.Models.Games;

public interface IGameListManager
{
    GameContext? FindCurrentGame();

    GameContext FindOrCreateActiveGame();
}