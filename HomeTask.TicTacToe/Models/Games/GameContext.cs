namespace HomeTask.TicTacToe.Models.Games;

public class GameContext
{
    public required Game Game { get; init; }
    public bool IsPlayer1 { get; init; }
    
    public int PlayerNumber => IsPlayer1 ? 1 : 2;
    public GameRole? PlayerRole => IsPlayer1 ? Game.Player1Selection() : Game.Player2Selection();
}