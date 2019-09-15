using SheepChat.Server.Interfaces;
using System;
using System.Text;

namespace SheepChat.Server
{
    /// <summary>
    /// Santizer service for cleaning input received from connections.
    /// </summary>
    public class InputSanitizer
    {
        /// <summary>
        /// Single character NewLine reference.
        /// </summary>
        public const string NewLine = "\r";

        /// <summary>
        /// Event handler delegate for sending connection and input information to the event handlers
        /// </summary>
        /// <param name="sender">Sender that triggered this event</param>
        /// <param name="connectionArgs">Connection that triggered this event</param>
        /// <param name="input">Input from the connection</param>
        public delegate void InputReceievedEventHandler(object sender, ConnectionArgs connectionArgs, string input);

        /// <summary>
        /// Event handler for data received from a connection
        /// </summary>
        public event InputReceievedEventHandler InputReceived;

        /// <summary>
        /// Process incoming data from a connection.
        /// </summary>
        /// <param name="sender">Connection sending the data</param>
        /// <param name="data">Data sent by the connection</param>
        public void OnDataReceived(IConnection sender, byte[] data)
        {
            string input = Encoding.ASCII.GetString(data);
            sender.Buffer.Append(input);
            input = sender.Buffer.ToString();
            input = StripExcessTerminator(sender, input);
            if (input.Length == 0) return;
            SetLastTerminator(sender, input);
            input = input.Replace("\n", NewLine);
            input = input.Replace("\r\r", NewLine);
            if(input.Contains(NewLine))
            {
                if(input == NewLine)
                {
                    InputReceived?.Invoke(this, new ConnectionArgs(sender), input);
                }

                bool isFinished = input.Contains(NewLine);

                string[] commands = input.Split(NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string currLine = string.Empty;

                for(int i = 0; i < commands.Length; i++)
                {
                    currLine = commands[i];
                    if(i < commands.Length-1 || (isFinished && i == commands.Length-1))
                    {
                        sender.LastRawInput = currLine;
                        InputReceived?.Invoke(this, new ConnectionArgs(sender), currLine.Trim());
                    }
                }

                sender.Buffer.Clear();
                if (!isFinished) sender.Buffer.Append(currLine);
            }
        }

        /// <summary>
        /// Keep track of the terminator last used by the connection when it sends data
        /// </summary>
        /// <param name="sender">Connection sending data</param>
        /// <param name="input">String input sent by the connection</param>
        private void SetLastTerminator(IConnection sender, string input)
        {
            if (input.Contains("\r") && input.Contains("\n")) sender.LastInputTerminator = "\r\n";
            else if (input.Contains("\r")) sender.LastInputTerminator = "\r";
            else if (input.Contains("\n")) sender.LastInputTerminator = "\n";
        }

        /// <summary>
        /// Handle excess terminators sent by systems like Windows which uses \r\n as opposed to a more concise \r or \n
        /// </summary>
        /// <param name="sender">Connection sending input</param>
        /// <param name="input">Input sent by the connection</param>
        /// <returns>String input stripped of any excess line terminators</returns>
        private static string StripExcessTerminator(IConnection sender, string input)
        {
            input = (sender.LastInputTerminator == "\r" && input.StartsWith("\n")) ? input.Replace("\n", string.Empty) : input;
            input = (sender.LastInputTerminator == "\n" && input.StartsWith("\r")) ? input.Replace("\r", string.Empty) : input;
            return input;
        }
    }
}
