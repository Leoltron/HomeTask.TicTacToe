namespace HomeTask.TicTacToe.Models.Games;

public class Game(Guid id)
{
    public Guid Id { get; } = id;
    public DateTime CreatedDate { get; } = DateTime.UtcNow;
    public GameState State { get; set; } = GameState.WaitingForOpponent;
    public BoardCellState[][] Board { get; } =
    {
        [BoardCellState.Empty, BoardCellState.Empty, BoardCellState.Empty], [BoardCellState.Empty, BoardCellState.Empty, BoardCellState.Empty], [BoardCellState.Empty, BoardCellState.Empty, BoardCellState.Empty]
    };
    public PlayerRoleSelection PlayerRoleSelection { get; set; } = PlayerRoleSelection.NotSelected;

    public GameRole? Player1Selection()
    {
        switch (PlayerRoleSelection)
        {
            case PlayerRoleSelection.NotSelected:
                return null;
            case PlayerRoleSelection.Player1IsX:
                return GameRole.X;
            case PlayerRoleSelection.Player1IsO:
                return GameRole.O;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public GameRole? Player2Selection() => Player1Selection()?.Opposite();

    public bool OnSecondPlayerConnected()
    {
        if (State != GameState.WaitingForOpponent)
        {
            return false;
        }
        lock (this)
        {
            if (State != GameState.WaitingForOpponent)
            {
                return false;
            }
            if (PlayerRoleSelection == PlayerRoleSelection.NotSelected)
            {
                State = GameState.SelectingRole;
            }
            else
            {
                State = GameState.XTurn;
            }
            return true;
        }
    }

    public bool TrySetRole(bool isPlayer1, GameRole role)
    {
        return TrySetPlayer1Role(isPlayer1 ? role : role.Opposite());
    }
    public bool TrySetPlayer1Role(GameRole role)
    {
        if (!CanSelectRole)
        {
            return false;
        }
        lock (this)
        {
            if (!CanSelectRole)
            {
                return false;
            }

            PlayerRoleSelection = role == GameRole.X ? PlayerRoleSelection.Player1IsX : PlayerRoleSelection.Player1IsO;
            if (State == GameState.SelectingRole)
            {
                State = GameState.XTurn;
            }
            return true;
        }
    }

    public bool TryMakeAMove(GameRole role, int x, int y)
    {
        if (x < 0 || x >= 3 || y < 0 || y >= 3)
        {
            return false;
        }

        if (role == GameRole.X && State != GameState.XTurn || role == GameRole.O && State != GameState.OTurn || Board[y][x] != BoardCellState.Empty)
        {
            return false;
        }

        lock (this)
        {
            if (role == GameRole.X && State != GameState.XTurn || role == GameRole.O && State != GameState.OTurn || Board[y][x] != BoardCellState.Empty)
            {
                return false;
            }

            Board[y][x] = role.ToBoardCellState();

            var winner = CheckWinner();
            if (winner != null)
            {
                State = winner == GameRole.X ? GameState.XWin : GameState.OWin;
            }
            else if (IsDraw())
            {
                State = GameState.Draw;
            }
            else
            {
                State = State == GameState.XTurn ? GameState.OTurn : GameState.XTurn;
            }
            return true;
        }
    }

    public bool IsDraw()
    {
        return CheckWinner() == null && !CanMakeAMove();
    }

    public bool CanMakeAMove()
    {
        for (var x = 0; x < 3; x++)
        {
            for (var y = 0; y < 3; y++)
            {
                if (Board[y][x] == BoardCellState.Empty)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public GameRole? CheckWinner()
    {
        for (var y = 0; y < 3; y++)
            if (Board[y][0] == Board[y][1] && Board[y][1] == Board[y][2] && Board[y][0] != BoardCellState.Empty)
                return Board[y][0] == BoardCellState.X ? GameRole.X : GameRole.O;

        for (var x = 0; x < 3; x++)
            if (Board[0][x] == Board[1][x] && Board[1][x] == Board[2][x] && Board[0][x] != BoardCellState.Empty)
                return Board[0][x] == BoardCellState.X ? GameRole.X : GameRole.O;

        if (Board[0][0] == Board[1][1] && Board[1][1] == Board[2][2] && Board[0][0] != BoardCellState.Empty)
            return Board[0][0] == BoardCellState.X ? GameRole.X : GameRole.O;

        if (Board[0][2] == Board[1][1] && Board[1][1] == Board[2][0] && Board[0][2] != BoardCellState.Empty)
            return Board[0][2] == BoardCellState.X ? GameRole.X : GameRole.O;

        return null;
    }

    private bool CanSelectRole => State is GameState.WaitingForOpponent or GameState.SelectingRole && PlayerRoleSelection == PlayerRoleSelection.NotSelected;
}