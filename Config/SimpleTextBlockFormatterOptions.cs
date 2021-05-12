namespace VectronsLibrary.TextBlockLogger
{
    /// <summary>
    /// Options for the built-in default textblock formatter.
    /// </summary>
    public class SimpleTextBlockFormatterOptions : TextBlockFormatterOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether no colors should be used.
        /// </summary>
        public bool DisableColors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether messages should be printed on a single line.
        /// </summary>
        public bool SingleLine
        {
            get;
            internal set;
        }
    }
}