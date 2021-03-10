using System.Windows.Media;

namespace VectronsLibrary.TextBlockLogger
{
    public readonly struct LevelColors
    {
        public readonly Brush? Background;
        public readonly Brush? Foreground;

        public LevelColors(Brush? foreground, Brush? background)
        {
            Foreground = foreground;
            Background = background;
        }
    }
}