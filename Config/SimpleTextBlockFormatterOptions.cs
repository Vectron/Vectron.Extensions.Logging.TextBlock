namespace VectronsLibrary.TextBlockLogger
{
    public class SimpleTextBlockFormatterOptions : TextBlockFormatterOptions
    {
        public bool DisableColors
        {
            get;
            set;
        }

        public bool SingleLine
        {
            get;
            internal set;
        }
    }
}