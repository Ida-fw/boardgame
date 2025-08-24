using System.Text.Json;

namespace BoardGame_Ass2
{
    abstract class Game
    {
        protected readonly Board Board;
        protected readonly Player Player1;
        protected readonly Player Player2;
        protected Player CurrentPlayer;
        protected Stack<Move> UndoStack { get; } = new();
        protected Stack<Move> RedoStack { get; } = new();

        public abstract void Start();
        public abstract void SaveGame(string filePath);
        public abstract void LoadGame(string filePath);
        public abstract void Help();

        protected Game(Board board, Player player1, Player player2, Player currentPlayer)
        {
            Board = board;
            Player1 = player1;
            Player2 = player2;
            CurrentPlayer = currentPlayer;
        }

        public virtual bool Undo()
        {
            if (UndoStack.Count > 0)
            {
                var m = UndoStack.Pop();
                Board.UndoMove(m);
                RedoStack.Push(m);
                SwapCurrentPlayer();
                return true;
            }
            return false;
        }

        public virtual bool Redo()
        {
            if (RedoStack.Count > 0)
            {
                var m = RedoStack.Pop();
                Board.ApplyMove(m);
                UndoStack.Push(m);
                SwapCurrentPlayer();
                return true;
            }
            return false;
        }

        protected void ApplyMove(Move move)
        {
            Board.ApplyMove(move);
            UndoStack.Push(move);
            RedoStack.Clear();
            Console.WriteLine(CurrentPlayer.GetAction());
            SwapCurrentPlayer();
        }

        protected void SwapCurrentPlayer() =>
            CurrentPlayer = ReferenceEquals(CurrentPlayer, Player1) ? Player2 : Player1;
    }


    class WildTicTacToeGame : Game
    {
        public WildTicTacToeGame(Board board, Player player1, Player player2, Player currentPlayer)
            : base(board, player1, player2, currentPlayer) { }


        public override void Start()
        {
            Console.WriteLine("=== Wild Tic-Tac-Toe ===");
            Help();

            while (!Board.IsGameOver())
            {
                Board.DisplayBoard();
                Move move = CurrentPlayer.DecideMove();
                if (!Board.IsMoveValid(move))
                {
                    Console.WriteLine("That move is not valid. Try again.");
                    continue;
                }
                ApplyMove(move);
            }

            Console.WriteLine();
            Console.WriteLine("=== GAME OVER ===");
            Board.DisplayBoard();
        }

        public override void Help()
        {
            Console.WriteLine("How to play:");
            Console.WriteLine(" - Coordinates are 0..2 for row and column.");
            Console.WriteLine(" - On your turn, enter: row,col,value  e.g. 1,2,X");
            Console.WriteLine(" - In Wild Tic-Tac-Toe you may place either X or O on your turn.");
            Console.WriteLine(" - The game ends when any 3-in-a-row of X or of O is formed, or the board is full.");
            Console.WriteLine(" - Enter --undo to undo the previous steps.");
            Console.WriteLine(" - Enter --redo to redo the undo steps.");
            Console.WriteLine(" - Enter --save to save the game anytime.");
        }

        public override void SaveGame(string filePath)
        {
            try
            {
                var history = UndoStack.Reverse()
                                       .OfType<WildTicTacToeMove>()
                                       .Select(m => new MoveDto { Row = m.Row, Column = m.Column, Value = m.Value })
                                       .ToList();

                var state = new SaveState
                {
                    Moves = history,
                    // Convention: Player1 always starts when (re)playing the saved moves.
                    Player1Name = Player1.Name,
                    Player2Name = Player2.Name,
                };

                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Saved game to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save: {ex.Message}");
            }
        }

        public override void LoadGame(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Save file not found.");
                    return;
                }

                var json = File.ReadAllText(filePath);
                var state = JsonSerializer.Deserialize<SaveState>(json) ?? throw new InvalidOperationException("Corrupt save file.");

                // Return board to initial state by undoing all known moves (if any)
                while (Undo()) { }
                RedoStack.Clear();

                // Convention: Player1 starts when we replay
                CurrentPlayer = Player1;

                // Replay in order
                foreach (var dto in state.Moves)
                {
                    var move = new WildTicTacToeMove(dto.Row, dto.Column, dto.Value);
                    if (!Board.IsMoveValid(move))
                        throw new InvalidOperationException($"Saved move not valid on step ({dto.Row},{dto.Column},{dto.Value}).");
                    ApplyMove(move);
                }

                Console.WriteLine($"Loaded game from: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load: {ex.Message}");
            }
        }

        // ---- helpers ----

        // DTOs for robust, non-polymorphic save files
        private sealed class SaveState
        {
            public List<MoveDto> Moves { get; set; } = new();
            public string? Player1Name { get; set; }
            public string? Player2Name { get; set; }
            public string? Mode { get; set; }
        }

        private sealed class MoveDto
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public WildTicTacToeValue Value { get; set; }
        }
    }
}
