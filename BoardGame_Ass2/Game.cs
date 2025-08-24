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
                var mv = UndoStack.Pop();
                Board.UndoMove(mv);
                RedoStack.Push(mv);
                Board.DisplayBoard();
                return true;
            }
            Console.WriteLine("There is nothing to undo.");
            return false;
        }

        public virtual bool Redo()
        {
            if (RedoStack.Count > 0)
            {
                var mv = RedoStack.Pop();
                Board.ApplyMove(mv);
                UndoStack.Push(mv);
                Board.DisplayBoard();
                return true;
            }
            Console.WriteLine("There is nothing to redo.");
            return false;
        }

        protected void ApplyMove(Move move)
        {
            Board.ApplyMove(move);
            UndoStack.Push(move);
            RedoStack.Clear();
            Board.DisplayBoard();
        }

        protected PlayerAction GetPlayerAction()
        {
            var action = CurrentPlayer.GetAction();
            switch (action)
            {
                case PlayerAction.Undo:
                    Undo(); SwapCurrentPlayer(); break;
                case PlayerAction.Redo:
                    Redo(); break;
                case PlayerAction.Continue:
                    break;
                default: break;
            }
            return action;
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
            CurrentPlayer.Help();

            while (!Board.IsGameOver())
            {
                PlayerCommand command = CurrentPlayer.GetPlayerCommand();
                switch (command)
                {
                    case MoveCommand m:
                        if (!Board.IsMoveValid(m.Move))
                        {
                            Console.WriteLine("That move is not valid. Try again.");
                            continue;
                        }
                        ApplyMove(m.Move);
                        SwapCurrentPlayer();
                        break;
                    case ActionCommand a:
                        switch (a.Action)
                        {
                            case PlayerAction.Undo: if (Undo()) SwapCurrentPlayer(); break;
                            case PlayerAction.Redo: if (Redo()) SwapCurrentPlayer(); break;
                                //case PlayerAction.SaveGame: SaveGame(a.Argument ?? "wild.json"); break;
                                //case PlayerAction.LoadGame: LoadGame(a.Argument ?? "wild.json"); break;
                        }
                        break;
                    default: break;
                }

            }

            Console.WriteLine("=================");
            Console.WriteLine("=== GAME OVER ===");
            Console.WriteLine("=================");
            Board.DisplayBoard();
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
