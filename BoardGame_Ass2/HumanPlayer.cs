namespace BoardGame_Ass2
{
    abstract class HumanPlayer
    {
        public string Name { get; set; }

        public HumanPlayer(string name)
        {
            Name = name;
        }

        public abstract Move DecideMove();
    }

    class WildTicTacToeHumanPlayer : HumanPlayer
    {
        public WildTicTacToeHumanPlayer(string name) : base(name) { }

        public override WildTicTacToeMove DecideMove()
        {
            Console.Write($"{Name}, enter your move as row,col,value (e.g. 1,2,X): ");
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
    }
}
