using System.Collections.Generic;
using System.Windows.Controls;

namespace VectronsLibrary.TextBlockLogger
{
    public interface ITextblockProvider
    {
        IEnumerable<TextBlock> Sinks
        {
            get;
        }

        void AddTextBlock(TextBlock textblock);

        void RemoveTextBlock(TextBlock textblock);
    }
}