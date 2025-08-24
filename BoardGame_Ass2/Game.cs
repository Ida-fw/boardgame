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

        protected void SwapCurrentPlayer() =>
            CurrentPlayer = ReferenceEquals(CurrentPlayer, Player1) ? Player2 : Player1;
    }


    class WildTicTacToeGame : Game
    {
        public WildTicTacToeGame(Board board, Player player1, Player player2, Player currentPlayer)
            : base(board, player1, player2, currentPlayer) { }


        public override void Start()
        {
            Console.WriteLine("========================");
            Console.WriteLine("=== Wild Tic-Tac-Toe ===");
            Console.WriteLine("========================");

            while (!Board.IsGameOver())
            {
                PlayerCommand command = CurrentPlayer.GetPlayerCommand(Board);
                switch (command)
                {
                    case MoveCommand m:
                        if (!Board.IsMoveValid(m.Move))
                        {
                            Console.WriteLine("That move is not valid. Try again.");
                            continue;
                        }
                        Board.ApplyMove(m.Move);
                        UndoStack.Push(m.Move);
                        RedoStack.Clear();
                        Board.DisplayBoard();
                        SwapCurrentPlayer();
                        break;
                    case ActionCommand a:
                        switch (a.Action)
                        {
                            case PlayerAction.Undo: if (Undo()) SwapCurrentPlayer(); break;
                            case PlayerAction.Redo: if (Redo()) SwapCurrentPlayer(); break;
                            case PlayerAction.SaveGame: SaveGame(a.Argument ?? "wild-tic-tac-toe.json"); return;
                            case PlayerAction.LoadGame:
                                if (UndoStack.Count > 0 || RedoStack.Count > 0) Console.WriteLine("Unable to load game in the middle of the current game.");
                                else LoadGame(a.Argument ?? "wild-tic-tac-toe.json");
                                break;
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
                    Player1Name = Player1.Name,
                    Player2Name = Player2.Name,
                    CurrentPlayerName = CurrentPlayer.Name,
                };

                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                var fullPath = Path.GetFullPath(filePath);
                File.WriteAllText(fullPath, json);
                Console.WriteLine($"Saved game to: {fullPath}");
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
                    Console.WriteLine("File not found.");
                    return;
                }

                var json = File.ReadAllText(filePath);
                var state = JsonSerializer.Deserialize<SaveState>(json) ?? throw new InvalidOperationException("Corrupt save file.");

                // Set the current player
                CurrentPlayer = state.CurrentPlayerName == Player1.Name ? Player1 : Player2;

                // Replay
                foreach (var dto in state.Moves)
                {
                    var move = new WildTicTacToeMove(dto.Row, dto.Column, dto.Value);
                    if (!Board.IsMoveValid(move))
                        throw new InvalidOperationException($"Saved move not valid on step ({dto.Row},{dto.Column},{dto.Value}).");
                    Board.ApplyMove(move);
                }

                Console.WriteLine($"Loaded game from: {filePath}");
                Board.DisplayBoard();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load: {ex.Message}");
            }
        }


        // DTOs for file saving
        private sealed class SaveState
        {
            public List<MoveDto> Moves { get; set; } = new();
            public string? Player1Name { get; set; }
            public string? Player2Name { get; set; }
            public string? CurrentPlayerName { get; set; }
        }

        private sealed class MoveDto
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public WildTicTacToeValue Value { get; set; }
        }
    }
}
