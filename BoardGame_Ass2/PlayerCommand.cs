namespace BoardGame_Ass2
{
    abstract record PlayerCommand;

    record MoveCommand(WildTicTacToeMove Move) : PlayerCommand;

    record ActionCommand(PlayerAction Action, string? Argument) : PlayerCommand;
}