namespace SheepChat.Server.Helpers
{
    /// <summary>
    /// RFC 854 and 857 Implementation for negotiating Interpret as Command over telnet.
    /// </summary>
    public static class Negotiation
    {
        /// <summary>
        /// RFC 857 Telnet Echo Option
        /// </summary>
        public const byte ECHO = 1;

        /// <summary>
        /// End of subnegotiation parameters.
        /// </summary>
        public const byte SE = 240;

        /// <summary>
        /// No operation.
        /// </summary>
        public const byte NOP = 241;

        /// <summary>
        /// The data stream portion of a Synch. This should always be accompanied by a TCP Urgent notification.
        /// </summary>
        public const byte DataMark = 242;

        /// <summary>
        /// NVT character BRK.
        /// </summary>
        public const byte BRK = 243;

        /// <summary>
        /// Interrupt Process.
        /// </summary>
        public const byte IP = 244;

        /// <summary>
        /// Abort Output.
        /// </summary>
        public const byte AO = 245;

        /// <summary>
        /// Are You There.
        /// </summary>
        public const byte AYT = 246;

        /// <summary>
        /// Erase Character.
        /// </summary>
        public const byte EC = 247;

        /// <summary>
        /// Erase Line.
        /// </summary>
        public const byte EL = 248;

        /// <summary>
        /// Go Ahead.
        /// </summary>
        public const byte GA = 249;

        /// <summary>
        /// Indicates that what follows is subnegotiation of the indicated option.
        /// </summary>
        public const byte SB = 250;

        /// <summary>
        /// Indicates the desire to begin performing, or confirmation that you are now performing, the indicated option.
        /// </summary>
        public const byte WILL = 251;

        /// <summary>
        /// Indicates the refusal to perform, or continue performing, the indicated option.
        /// </summary>
        public const byte WONT = 252;

        /// <summary>
        /// Indicates the request that the other party perform, or confirmation that you are expecting the other party to perform, the indicated option.
        /// </summary>
        public const byte DO = 253;

        /// <summary>
        /// Indicates the demand that the other party stop performing, or confirmation that you are no longer expecting the other party to perform, the indicated option.
        /// </summary>
        public const byte DONT = 254;

        /// <summary>
        /// Interpret As Command
        /// </summary>
        public const byte IAC = 255;

        /// <summary>
        /// Concatenate IAC WILL command.
        /// </summary>
        /// <param name="command">Probably <see cref="ECHO"/>.</param>
        /// <returns>Full IAC WILL byte sequence.</returns>
        public static byte[] Will(byte command)
        {
            return new byte[]
            {
                IAC,
                WILL,
                command
            };
        }

        /// <summary>
        /// Concatenate IAC WONT command.
        /// </summary>
        /// <param name="command">Probably <see cref="ECHO"/>.</param>
        /// <returns>Full IAC WONT byte sequence.</returns>
        public static byte[] Wont(byte command)
        {
            return new byte[]
            {
                IAC,
                WONT,
                command
            };
        }

        /// <summary>
        /// Concatenate IAC DO command.
        /// </summary>
        /// <param name="command">Probably <see cref="ECHO"/>.</param>
        /// <returns>Full IAC DO byte sequence.</returns>
        public static byte[] Do(byte command)
        {
            return new byte[]
            {
                IAC,
                DO,
                command
            };
        }

        /// <summary>
        /// Concatenate IAC DONT command.
        /// </summary>
        /// <param name="command">Probably <see cref="ECHO"/>.</param>
        /// <returns>Full IAC DONT byte sequence.</returns>
        public static byte[] Dont(byte command)
        {
            return new byte[]
            {
                IAC,
                DONT,
                command
            };
        }
    }
}