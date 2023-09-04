using System.IO;
using Vectron.Ansi;

namespace Vectron.Extensions.Logging.TextBlock;

/// <summary>
/// Extension methods for <see cref="TextWriter"/>.
/// </summary>
internal static class TextWriterExtension
{
    /// <summary>
    /// Write the ANSI code, then text, then ANSI reset style to the given <paramref name="textWriter"/>.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/>.</param>
    /// <param name="code">The ANSI code to write.</param>
    /// <param name="text">The text to write.</param>
    public static void WriteEscaped(this TextWriter textWriter, string code, string text)
    {
        if (string.IsNullOrEmpty(code))
        {
            textWriter.Write(text);
            return;
        }

        textWriter.Write(code + text + AnsiHelper.ResetColorAndStyleAnsiEscapeCode);
    }
}
