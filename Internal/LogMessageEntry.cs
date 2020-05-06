using System.Windows.Media;

namespace VectronsLibrary.TextBlockLogger.Internal
{
    public struct LogMessageEntry
    {
        public Brush LevelBackground;
        public Brush LevelForeground;
        public string LevelString;
        public string Message;
        public Brush MessageColor;
    }
}