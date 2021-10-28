using System.Windows.Media;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// A class to store the forground and background color of text.
/// </summary>
public readonly struct LevelColors
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LevelColors"/> struct.
    /// </summary>
    /// <param name="foreground">The <see cref="Brush"/> to use to draw the foreground text.</param>
    /// <param name="background">The <see cref="Brush"/> to use to draw the background.</param>
    public LevelColors(Brush? foreground, Brush? background)
    {
        Foreground = foreground;
        Background = background;
    }

    /// <summary>
    /// Gets the <see cref="Brush"/> to use to draw the background text.
    /// </summary>
    public Brush? Background
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="Brush"/> to use to draw the foreground text.
    /// </summary>
    public Brush? Foreground
    {
        get;
    }
}