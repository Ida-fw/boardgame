namespace BoardGame_Ass2
{
    abstract class Move { }

    class WildTicTacToeMove : Move
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public WildTicTacToeValue Value { get; set; }

        public WildTicTacToeMove(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be empty.");
            }

            // Split by comma
            string[] parts = input.Split(',');

            if (parts.Length != 3)
            {
                throw new ArgumentException("Input must be in format: row,column,value (e.g. 1,2,X).");
            }

            if (!int.TryParse(parts[0].Trim(), out int row)) // parse row
            {
                throw new ArgumentException("Row must be an integer.");
            }

            if (!int.TryParse(parts[1].Trim(), out int column)) // parse column
            {
                throw new ArgumentException("Column must be an integer.");
            }

            if (!Enum.TryParse(parts[2].Trim(), true, out WildTicTacToeValue val)) // parse value
            {
                throw new ArgumentException("Value must be 'X' or 'O'.");
            }

            Row = row;
            Column = column;
            Value = val;
        }

        public override string ToString()
        {
            return $"Move: ({Row},{Column}) with '{Value}'";
        }
    }

}