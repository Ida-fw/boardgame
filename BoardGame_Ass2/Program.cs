using BoardGame_Ass2;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Choose play mode: 0: Human Vs Human, 1: Human Vs Computer");
        string? userInput = Console.ReadLine();

        if (int.TryParse(userInput, out int choice) && Enum.IsDefined(typeof(PlayMode), choice))
        {
            PlayMode mode = (PlayMode)choice;

            switch (mode)
            {
                case PlayMode.HumanVsHuman:
                    Console.WriteLine("Starting Human vs Human mode...");
                    Player p1 = new WildTicTacToeHumanPlayer("Player1");
                    Player p2 = new WildTicTacToeHumanPlayer("Player2");
                    WildTicTacToeBoard board = new();
                    WildTicTacToeGame game = new(board, p1, p2, p1);
                    game.Start();
                    break;
                case PlayMode.HumanVsComputer:
                    Console.WriteLine("Starting Human vs Computer mode...");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid choice!");
        }
    }
}