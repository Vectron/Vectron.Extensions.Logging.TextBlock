using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Documents;

namespace VectronsLibrary.TextBlockLogger
{
    internal class TextBlockLoggerProcessor : IDisposable
    {
        private const int maxQueuedMessages = 1024;
        private readonly BlockingCollection<LogMessageEntry> messageQueue = new BlockingCollection<LogMessageEntry>(maxQueuedMessages);
        private readonly Thread outputThread;
        private readonly ITextblockProvider textblockProvider;

        public TextBlockLoggerProcessor(ITextblockProvider textblockProvider)
        {
            // Start TextBlock message queue processor
            outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "TextBlock logger queue processing thread"
            };
            outputThread.Start();
            this.textblockProvider = textblockProvider;
        }

        public int MaxMessages
        {
            get;
            set;
        }

        public void Dispose()
        {
            messageQueue.CompleteAdding();

            try
            {
                outputThread.Join(1500);
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
            try
            {
                WriteMessage(message);
            }
            catch (Exception) { }
        }

        internal virtual void WriteMessage(LogMessageEntry message)
        {
            foreach (var textBlock in textblockProvider.Sinks)
            {
                textBlock?.Dispatcher.Invoke(() =>
                {
                    if (message.LevelString != null)
                    {
                        var run1 = new Run(message.LevelString);

                        if (message.LevelColors.Foreground != null)
                        {
                            run1.Foreground = message.LevelColors.Foreground;
                        }

                        if (message.LevelColors.Background != null)
                        {
                            run1.Background = message.LevelColors.Background;
                        };

                        if (textBlock.Inlines.Count > MaxMessages)
                        {
                            _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
                        }

                        textBlock.Inlines.Add(run1);
                    }

                    var run2 = new Run(message.Message);

                    if (textBlock.Inlines.Count > MaxMessages)
                    {
                        _ = textBlock.Inlines.Remove(textBlock.Inlines.FirstInline);
                    }

                    textBlock.Inlines.Add(run2);
                });
            }
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (LogMessageEntry message in messageQueue.GetConsumingEnumerable())
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
                catch { }
            }
        }
    }
}