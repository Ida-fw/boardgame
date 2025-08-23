namespace BoardGame_Ass2
{
    abstract class HumanPlayer
    {
        public string Name { get; set; }

        public HumanPlayer(string name)
        {
            Name = name;
        }

        public abstract string DecideMove();
    }

    class WildTicTacToeHumanPlayer : HumanPlayer
    {
        public WildTicTacToeHumanPlayer(string name) : base(name) { }

        public override string DecideMove()
        {
            Console.Write($"{Name}, enter your move (row, column and mark X/O): ");
            string? input = Console.ReadLine();

            // Validate: basic check for empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid move! Please try again.");
                return DecideMove(); // retry recursively
            }

            return input.Trim();
        }
    }
}
