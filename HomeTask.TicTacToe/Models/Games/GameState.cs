namespace HomeTask.TicTacToe.Models.Games;


public enum GameState
{
    WaitingForOpponent,
    SelectingRole,
    
    XTurn,
    OTurn,
    
    XWin,
    OWin,
    Draw
}