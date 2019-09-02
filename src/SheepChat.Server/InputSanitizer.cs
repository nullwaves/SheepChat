using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SheepChat.Server
{
    public class InputSanitizer
    {
        public const string NewLine = "\r";

        public delegate void InputReceievedEventHandler(object sender, ConnectionArgs connectionArgs, string input);

        public event InputReceievedEventHandler InputReceived;

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

                string[] commands = input.Split(NewLine, StringSplitOptions.RemoveEmptyEntries);
                string currLine = string.Empty;

                for(int i = 0; i < commands.Length; i++)
                {
                    currLine = commands[i];
                    if(i < commands.Length-1 || (isFinished && i == commands.Length-1))
                    {
                        sender.LastRawInput = currLine;
                        InputReceived?.Invoke(this, new ConnectionArgs(sender), input);
                    }
                }

                sender.Buffer.Clear();
                if (!isFinished) sender.Buffer.Append(currLine);

            }
        }

        private void SetLastTerminator(IConnection sender, string input)
        {
            if (input.Contains("\r") && input.Contains("\n")) sender.LastInputTerminator = "\r\n";
            else if (input.Contains("\r")) sender.LastInputTerminator = "\r";
            else if (input.Contains("\n")) sender.LastInputTerminator = "\n";
        }

        private static string StripExcessTerminator(IConnection sender, string input)
        {
            input = (sender.LastInputTerminator == "\r" && input.StartsWith("\n")) ? input.Replace("\n", string.Empty) : input;
            input = (sender.LastInputTerminator == "\n" && input.StartsWith("\r")) ? input.Replace("\r", string.Empty) : input;
            return input;
        }
    }
}
