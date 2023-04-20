using System;
using System.IO;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// <see cref="TextWriter"/> extension methods.
/// </summary>
internal static class TextWriterExtensions
{
    /// <summary>
    /// Write an ansi colored string.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
    /// <param name="message">The message to write.</param>
    /// <param name="background">The background color.</param>
    /// <param name="foreground">The foreground color.</param>
    public static void WriteColoredMessage(this TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor? foreground)
    {
        // Order: background color, foreground color, Message, reset foreground color, reset background color
        if (background.HasValue)
        {
            textWriter.Write(AnsiParser.GetBackgroundColorEscapeCode(background.Value));
        }

        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser.GetForegroundColorEscapeCode(foreground.Value));
        }

        textWriter.Write(message);
        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser.DefaultForegroundColor); // reset to default foreground color
        }

        if (background.HasValue)
        {
            textWriter.Write(AnsiParser.DefaultBackgroundColor); // reset to the background color
        }
    }
}