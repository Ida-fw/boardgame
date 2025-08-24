

namespace BoardGame_Ass2
{
    abstract class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public abstract PlayerCommand GetPlayerCommand(Board board);
    }

    abstract class HumanPlayer : Player
    {
        public HumanPlayer(string name) : base(name) { }
        public abstract void Help();
    }

    class WildTicTacToeHumanPlayer : HumanPlayer
    {
        public WildTicTacToeHumanPlayer(string name) : base(name) { }

        public override PlayerCommand GetPlayerCommand(Board board)
        {
            while (true)
            {
                Console.Write($"{Name}, enter a move (row,col,value) OR an action (--undo/--redo/--save <path>/--load <path>/--help): ");
                var input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                // if it's action command
                if (input.TrimStart().StartsWith("--"))
                {
                    var parts = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0].ToLowerInvariant();
                    string? arg = parts.Length > 1 ? parts[1].Trim() : null;

                    switch (command)
                    {
                        case "--help":
                            Help(); continue;
                        case "--undo":
                            return new ActionCommand(PlayerAction.Undo, null);
                        case "--redo":
                            return new ActionCommand(PlayerAction.Redo, null);
                        case "--save":
                            return new ActionCommand(PlayerAction.SaveGame, arg);
                        case "--load":
                            return new ActionCommand(PlayerAction.LoadGame, arg);
                        default:
                            Console.WriteLine("Unknown action. Try again.");
                            break;
                    }
                    continue;
                }

                // If it's a move command
                try
                {
                    var move = new WildTicTacToeMove(input);
                    return new MoveCommand(move);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Invalid move: {ex.Message}");
                    continue;
                }
            }
        }

        public override void Help()
        {
            Console.WriteLine("How to play:");
            Console.WriteLine(" - Coordinates are 0..2 for row and column.");
            Console.WriteLine(" - In Wild Tic-Tac-Toe you may place either X or O on your turn.");
            Console.WriteLine(" - The game ends when any 3-in-a-row of X or of O is formed, or the board is full.");
            Console.WriteLine(" - On your turn, enter: row,col,value  e.g. 1,2,X");
            Console.WriteLine(" - Enter --undo to undo the previous steps.");
            Console.WriteLine(" - Enter --redo to redo the undo steps.");
            Console.WriteLine(" - Enter --save to save the game anytime.");
            Console.WriteLine(" - Enter --help to show command help.");
            Console.WriteLine();
        }
    }

    class WildTicTacToeComputerPlayer : Player
    {
        public WildTicTacToeComputerPlayer(string name) : base(name) { }

        public override PlayerCommand GetPlayerCommand(Board board)
        {
            Console.WriteLine($"Computer {Name}'s turn: ");
            return new MoveCommand(board.GetRandomMove());
        }
    }
}
