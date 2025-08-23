using BoardGame_Ass2;

class Program
{
    static void Main(string[] args)
    {
        //Player p1 = new HumanPlayer { Name = "Alice" };
        //Player p2 = new ComputerPlayer { Name = "AI" };
        //Game game = new NumericalTicTacToe(p1, p2);
        //game.Start();
        Console.WriteLine("Choose play mode: 0: Human Vs Human, 1: Human Vs Computer");
        string? userInput = Console.ReadLine();

        if (int.TryParse(userInput, out int choice) && Enum.IsDefined(typeof(PlayMode), choice))
        {
            PlayMode mode = (PlayMode)choice;

            switch (mode)
            {
                case PlayMode.HumanVsHuman:
                    Console.WriteLine("Starting Human vs Human mode...");
                    HumanPlayer p1 = new WildTicTacToeHumanPlayer("Alice");
                    HumanPlayer p2 = new WildTicTacToeHumanPlayer("Ida");
                    Console.WriteLine(p1.Name);
                    Console.WriteLine(p1.DecideMove());
                    Console.WriteLine(p2.Name);
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