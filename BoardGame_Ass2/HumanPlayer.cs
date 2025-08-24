namespace BoardGame_Ass2
{
    abstract class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public abstract Move DecideMove();
        public abstract PlayerAction GetAction();
    }

    class WildTicTacToeHumanPlayer : Player
    {
        public WildTicTacToeHumanPlayer(string name) : base(name) { }

        public override WildTicTacToeMove DecideMove()
        {
            Console.Write($"{Name}'s turn: ");
            string? input = Console.ReadLine();

            try
            {
                return new WildTicTacToeMove(input ?? "");
            }
            catch (ArgumentException error)
            {
                Console.WriteLine($"Invalid input: {error.Message}");
                return DecideMove();
            }
        }

        public override PlayerAction GetAction()
        {
            Console.Write($"{Name}, any action before move on to next player? Enter an action (--undo, --redo, --save <path>, --load <path>, --help) or just enter to continue: ");
            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return PlayerAction.Continue;
                var command = input.Trim().ToLowerInvariant();
                switch (command)
                {
                    case "--undo":
                        return PlayerAction.Undo;
                    case "--redo":
                        return PlayerAction.Redo;
                    case "--help":
                        return PlayerAction.ShowHelp;
                    case "--save":
                        return PlayerAction.SaveGame;
                    case "--load":
                        return PlayerAction.LoadGame;
                    default:
                        Console.WriteLine("Unknown action. Try again.");
                        break;
                }
            }
        }
    }
}
