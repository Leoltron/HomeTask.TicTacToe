using HomeTask.TicTacToe.Models.Games;
namespace HomeTask.TicTacToe.Models;

public static class Extensions
{
    public static GameRole Opposite(this GameRole role) => role switch
    {
        GameRole.X => GameRole.O,
        GameRole.O => GameRole.X,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static BoardCellState ToBoardCellState(this GameRole role) => role switch
    {
        GameRole.X => BoardCellState.X,
        GameRole.O => BoardCellState.O,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static bool IsActive(this GameState state) => state switch
    {
        GameState.WaitingForOpponent or GameState.SelectingRole or GameState.XTurn or GameState.OTurn => true,
        GameState.XWin or GameState.OWin or GameState.Draw => false,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
    };
}