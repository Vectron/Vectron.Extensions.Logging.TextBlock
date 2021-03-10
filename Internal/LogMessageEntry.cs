using System;

namespace VectronsLibrary.TextBlockLogger
{
    internal readonly struct LogMessageEntry
    {
        public readonly LevelColors LevelColors;
        public readonly string LevelString;
        public readonly string Message;

        public LogMessageEntry(string message, string levelString, LevelColors levelColors)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            LevelString = levelString ?? throw new ArgumentNullException(nameof(levelString));
            LevelColors = levelColors;
        }
    }
}