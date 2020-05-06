using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace VectronsLibrary.TextBlockLogger.Internal
{
    internal class TextBlockLoggerProcessor : Observable, IDisposable
    {
        private const int maxQueuedMessages = 1024;
        private readonly MenuItem closeMenu = new MenuItem() { Header = "Clear" };
        private readonly BlockingCollection<LogMessageEntry> messageQueue = new BlockingCollection<LogMessageEntry>(maxQueuedMessages);
        private readonly Thread outputThread;
        private readonly TextBlock textBlock;

        public TextBlockLoggerProcessor(TextBlock textBlock, int maxMessages = 100)
        {
            this.textBlock = textBlock ?? throw new ArgumentNullException(nameof(textBlock));
            MaxMessages = maxMessages;
            closeMenu.Click += (o, e) => textBlock.Inlines.Clear();

            if (textBlock.ContextMenu == null)
            {
                textBlock.ContextMenu = new ContextMenu();
            }

            _ = textBlock.ContextMenu.Items.Add(closeMenu);

            // Start TextBlock message queue processor
            outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "TextBlock logger queue processing thread"
            };
            outputThread.Start();
        }

        public int MaxMessages
        {
            get;
            set;
        }

        public void Dispose()
        {
            messageQueue.CompleteAdding();
            textBlock?.ContextMenu?.Items?.Remove(closeMenu);

            try
            {
                outputThread.Join(1500); // with timeout in-case TextBlock is locked by user input
            }
            catch (ThreadStateException) { }
        }

        public virtual void EnqueueMessage(LogMessageEntry message)
        {
            if (!messageQueue.IsAddingCompleted)
            {
                try
                {
                    messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            WriteMessage(message);
        }

        internal virtual void WriteMessage(LogMessageEntry message)
        {
            textBlock?.Dispatcher.Invoke(() =>
            {
                if (message.LevelString != null)
                {
                    var run1 = new Run(message.LevelString)
                    {
                        Foreground = message.LevelForeground
                    };

                    if (textBlock.Inlines.Count > MaxMessages)
                    {
                        _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
                    }

                    textBlock.Inlines.Add(run1);
                }

                var run2 = new Run(message.Message)
                {
                    Foreground = message.LevelForeground
                };

                if (textBlock.Inlines.Count > MaxMessages)
                {
                    _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
                }

                textBlock.Inlines.Add(run2);
            });
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var message in messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    messageQueue.CompleteAdding();
                }
                catch
                {
                }
            }
        }
    }
}