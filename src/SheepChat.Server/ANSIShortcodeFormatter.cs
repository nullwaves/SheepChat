using System;
using System.Collections.Generic;
using static SheepChat.Server.ANSI;

namespace SheepChat.Server
{
    public static class ANSIShortcodeFormatter
    {
        public static Dictionary<string, string> Shortcodes;
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
    }
}
