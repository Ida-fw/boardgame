
namespace BoardGame_Ass2
{
    abstract class Board
    {
        public abstract void DisplayBoard();
        public abstract bool IsMoveValid(Move move);
        public abstract void ApplyMove(Move move);
        public abstract void UndoMove(Move move);
        public abstract Move GetRandomMove();
        public abstract bool IsGameOver();
    }

    class WildTicTacToeBoard : Board
    {
        private readonly WildTicTacToeValue?[,] boardGrid = new WildTicTacToeValue?[3, 3];
        private static readonly Random rand = new();

        public override void DisplayBoard()
        {
            Console.WriteLine("-------------");
            for (int r = 0; r < 3; r++)
            {
                Console.Write("|");
                for (int c = 0; c < 3; c++)
                {
                    var cell = boardGrid[r, c];
                    string v = cell.HasValue ? (cell.Value == WildTicTacToeValue.X ? " X " : " O ") : "   ";
                    Console.Write(v);
                    if (c < 2) { Console.Write('|'); } else { Console.WriteLine('|'); }

                }
                Console.WriteLine("-------------");
            }
        }

        public override bool IsMoveValid(Move move)
        {
            if (move is not WildTicTacToeMove wm) return false;
            if (!InBoardBoundary(wm.Row, wm.Column)) return false;
            return !boardGrid[wm.Row, wm.Column].HasValue;
        }

        public override void ApplyMove(Move move)
        {
            if (move is not WildTicTacToeMove wm) throw new ArgumentException("Expected WildTicTacToeMove.", nameof(move));
            boardGrid[wm.Row, wm.Column] = IsMoveValid(move) ? wm.Value : throw new InvalidOperationException("Move not valid");
        }

        public override void UndoMove(Move move)
        {
            if (move is not WildTicTacToeMove wm) throw new ArgumentException("Expected WildTicTacToeMove.", nameof(move));
            if (!InBoardBoundary(wm.Row, wm.Column)) throw new ArgumentOutOfRangeException(nameof(move), "Undo is out of the board boundary.");
            if (boardGrid[wm.Row, wm.Column].HasValue)
                boardGrid[wm.Row, wm.Column] = null;
            else
                throw new InvalidOperationException("Invalid undo, no value in this cell.");
        }

        public override Move GetRandomMove()
        {
            var vacany = new List<(int r, int c)>();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (!boardGrid[r, c].HasValue) vacany.Add((r, c));

            if (vacany.Count == 0)
                throw new InvalidOperationException("No valid moves available.");

            var (row, col) = vacany[rand.Next(vacany.Count)];
            var val = rand.Next(2) == 0 ? WildTicTacToeValue.X : WildTicTacToeValue.O;
            return new WildTicTacToeMove(row, col, val);
        }

        public override bool IsGameOver()
        {
            return HasThreeInRow(WildTicTacToeValue.X) || HasThreeInRow(WildTicTacToeValue.O) || IsFull();
        }

        private bool IsFull()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (!boardGrid[r, c].HasValue) return false;
            return true;
        }

        private bool HasThreeInRow(WildTicTacToeValue value)
        {
            // check all rows
            for (int r = 0; r < 3; r++)
                if (boardGrid[r, 0] == value && boardGrid[r, 1] == value && boardGrid[r, 2] == value) return true;

            // check all columns
            for (int c = 0; c < 3; c++)
                if (boardGrid[0, c] == value && boardGrid[1, c] == value && boardGrid[2, c] == value) return true;

            // check diagonals
            if (boardGrid[0, 0] == value && boardGrid[1, 1] == value && boardGrid[2, 2] == value) return true;
            if (boardGrid[0, 2] == value && boardGrid[1, 1] == value && boardGrid[2, 0] == value) return true;

            return false;
        }

        private static bool InBoardBoundary(int r, int c) => r >= 0 && r < 3 && c >= 0 && c < 3;
    }




}