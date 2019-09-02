namespace SheepChat.Server.ANSI
{
    public class ANSI
    {
        /// <summary>
        /// Basic ANSI Escape code. Starts every sequence.
        /// </summary>
        public const string Escape = "\x1B";

        /// <summary>
        /// Save the current cursor position.
        /// </summary>
        public const string SaveCursorPosition = Escape + "[s";

        /// <summary>
        /// Load saved cursor position.
        /// </summary>
        public const string LoadCursorPosition = Escape + "[u";
        
        /// <summary>
        /// Clear screen and return cursor to home position.
        /// </summary>
        public const string ClearScreenAndHomeCursor = Escape + "[2J";
        
        /// <summary>
        /// Clear to end of line.
        /// </summary>
        public const string ClearToEOL = Escape + "K";

        /// <summary>
        /// Foreground specific ANSI Text color code. See also <seealso cref="Color(FGColorBit)"/>.
        /// </summary>
        public enum FGColorBit
        {
            /// <summary>
            /// ANSI Black Text
            /// </summary>
            Black = 30,
            /// <summary>
            /// ANSI Red Text
            /// </summary>
            Red = 31,
            /// <summary>
            /// ANSI Green Text
            /// </summary>
            Green = 32,
            /// <summary>
            /// ANSI Yellow Text
            /// </summary>
            Yellow = 33,
            /// <summary>
            /// ANSI Blue Text
            /// </summary>
            Blue = 34,
            /// <summary>
            /// ANSI Magenta Text
            /// </summary>
            Magenta = 35,
            /// <summary>
            /// ANSI Cyan Text
            /// </summary>
            Cyan = 36,
            /// <summary>
            /// ANSI White Text
            /// </summary>
            White = 37
        }

        /// <summary>
        /// Background specific ANSI Text color code. See also <seealso cref="Color(BGColorBit)"/>.
        /// </summary>
        public enum BGColorBit
        {
            /// <summary>
            /// ANSI Black Background
            /// </summary>
            Black = 40,
            /// <summary>
            /// ANSI Red Background
            /// </summary>
            Red = 41,
            /// <summary>
            /// ANSI Green Background
            /// </summary>
            Green = 42,
            /// <summary>
            /// ANSI Yellow Background
            /// </summary>
            Yellow = 43,
            /// <summary>
            /// ANSI Blue Background
            /// </summary>
            Blue = 44,
            /// <summary>
            /// ANSI Magenta Background
            /// </summary>
            Magenta = 45,
            /// <summary>
            /// ANSI Cyan Background
            /// </summary>
            Cyan = 46,
            /// <summary>
            /// ANSI White Background
            /// </summary>
            White = 47
        }

        /// <summary>
        /// "Fancy" attribute specific ANSI text formatting code. See also <seealso cref="RichText(AttributeBit)"/>
        /// </summary>
        public enum AttributeBit
        {
            /// <summary>
            /// ANSI Default Text Attribute
            /// </summary>
            Normal = 0,
            /// <summary>
            /// ANSI Bold Text Attribute
            /// </summary>
            Bold = 1,
            /// <summary>
            /// ANSI Underlined Text Attribute
            /// </summary>
            Underline = 4
        }

        /// <summary>
        /// ANSI sequence to set text to a color.
        /// </summary>
        /// <param name="bit">Color to set text to.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string Color(FGColorBit bit) => ColorText((int)bit);

        /// <summary>
        /// ANSI sequence to set background to a color.
        /// </summary>
        /// <param name="bit">Color to set background to.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string Color(BGColorBit bit) => ColorText((int)bit);

        /// <summary>
        /// ANSI sequence to set non-color attribute.
        /// </summary>
        /// <param name="bit">Text attribute to apply.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string RichText(AttributeBit bit) => ColorText((int)bit);

        /// <summary>
        /// ANSI sequence to set all text attributes in one sequence.
        /// </summary>
        /// <param name="bit">Non-color text attribute</param>
        /// <param name="fbit">Text color</param>
        /// <param name="bbit">Background color</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string RichText(AttributeBit bit, FGColorBit fbit, BGColorBit bbit) => Escape + string.Format("[{0};{1};{2}", bit, fbit, bbit);

        /// <summary>
        /// Directional bit in ANSI cursor-moving sequences.
        /// </summary>
        public enum MoveDirection
        {
            Up = 'A',
            Down = 'B',
            Right = 'C',
            Left = 'D'
        }

        /// <summary>
        /// ANSI sequence to move the cursor.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="numRowsOrCols">Number of rows or columns to move respectively.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string MoveCursor(MoveDirection direction, int numRowsOrCols) => Escape + string.Format("[{0}{1}", numRowsOrCols, direction);

        /// <summary>
        /// ANSI sequence to move the cursor to a specific location.
        /// </summary>
        /// <param name="x">Target row position.</param>
        /// <param name="y">target column position.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        public static string MoveCursorTo(int x, int y) => Escape + string.Format("[{0};{1}H", x, y);

        /// <summary>
        /// Internal shorthand command for single text attribute sequences.
        /// </summary>
        /// <param name="n">Text attribute bit.</param>
        /// <returns>Formatted ANSI sequence as a string.</returns>
        private static string ColorText(int n)
        {
            return Escape + string.Format("[{0}m", n);
        }
    }
}
