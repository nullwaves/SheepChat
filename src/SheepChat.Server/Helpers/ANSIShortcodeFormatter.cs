using System;
using System.Collections.Generic;
using static SheepChat.Server.ANSI;

namespace SheepChat.Server
{
    /// <summary>
    /// Data formatter that parses and replaces a custom format shortcode for special ANSI escape sequences like fancy text or cursor moving.
    /// </summary>
    public static class ANSIShortcodeFormatter
    {
        /// <summary>
        /// Full dictionary list of shortcodes and their associated ANSI escape sequence.
        /// </summary>
        public static Dictionary<string, string> Shortcodes;

        /// <summary>
        /// Static default constructor for the formatter. Loads all acceptable shortcodes
        /// </summary>
        static ANSIShortcodeFormatter()
        {
            Shortcodes = new Dictionary<string, string>();

            var fg = (FGColorBit[])Enum.GetValues(typeof(FGColorBit));
            foreach (var f in fg)
            {
                Shortcodes.Add(
                    Enum.GetName(f.GetType(), f).ToLower(),
                    Color(f)
                    );
            }

            var bg = (BGColorBit[])Enum.GetValues(typeof(BGColorBit));
            foreach (var b in bg)
            {
                Shortcodes.Add(
                    "b" + Enum.GetName(b.GetType(), b).ToLower(),
                    Color(b)
                    );
            }

            var at = (AttributeBit[])Enum.GetValues(typeof(AttributeBit));
            foreach (var a in at)
            {
                Shortcodes.Add(
                    Enum.GetName(a.GetType(), a).ToLower(),
                    RichText(a)
                    );
            }

            Shortcodes.Add("cls", ClearScreenAndHomeCursor);
            Shortcodes.Add("cln", ClearToEOL);
            Shortcodes.Add("svcur", SaveCursorPosition);
            Shortcodes.Add("ldcur", LoadCursorPosition);

            foreach (var d in (MoveDirection[])Enum.GetValues(typeof(MoveDirection)))
            {
                Shortcodes.Add(
                    Enum.GetName(d.GetType(), d).ToLower(),
                    MoveCursor(d, 1)
                    );
            }
        }

        /// <summary>
        /// Format a string that might contain shortcodes, replacing them with their associated escape sequence.
        /// </summary>
        /// <param name="data">String to parse</param>
        /// <returns>Formatted string with ANSI escape sequences</returns>
        public static string Format(string data)
        {
            foreach (KeyValuePair<string, string> kvp in Shortcodes)
            {
                data = data.Replace("<#" + kvp.Key + ">", kvp.Value);
            }
            return data;
        }
    }
}