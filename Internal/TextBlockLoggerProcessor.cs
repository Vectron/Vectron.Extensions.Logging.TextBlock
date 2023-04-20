using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace VectronsLibrary.TextBlockLogger.Internal;

/// <summary>
/// Message processor that writes the messages to the actual <see cref="System.Windows.Controls.TextBlock"/>.
/// </summary>
internal sealed class TextBlockLoggerProcessor : IDisposable
{
    private readonly Queue<LogMessageEntry> messageQueue;
    private readonly Thread outputThread;
    private readonly ITextBlockProvider textBlockProvider;
    private TextBlockLoggerQueueFullMode fullMode = TextBlockLoggerQueueFullMode.Wait;
    private bool isAddingCompleted;
    private int maxQueuedMessages = TextBlockLoggerOptions.DefaultMaxQueueLengthValue;
    private volatile int messagesDropped;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextBlockLoggerProcessor"/> class.
    /// </summary>
    /// <param name="textBlockProvider">A <see cref="ITextBlockProvider"/> implementation.</param>
    /// <param name="fullMode">The full mode to use.</param>
    /// <param name="maxQueueLength">The max messages to queue.</param>
    public TextBlockLoggerProcessor(ITextBlockProvider textBlockProvider, TextBlockLoggerQueueFullMode fullMode, int maxQueueLength)
    {
        messageQueue = new Queue<LogMessageEntry>();
        FullMode = fullMode;
        MaxQueueLength = maxQueueLength;
        this.textBlockProvider = textBlockProvider;

        // Start Console message queue processor
        outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "Console logger queue processing thread",
        };
        outputThread.Start();
    }

    /// <summary>
    /// Gets or sets the full mode to use.
    /// </summary>
    public TextBlockLoggerQueueFullMode FullMode
    {
        get => fullMode;
        set
        {
            if (value is not TextBlockLoggerQueueFullMode.Wait and not TextBlockLoggerQueueFullMode.DropWrite)
            {
                throw new ArgumentOutOfRangeException(nameof(FullMode), $"{value} is not a supported queue mode value.");
            }

            lock (messageQueue)
            {
                // _fullMode is used inside the lock and is safer to guard setter with lock as well
                // this set is not expected to happen frequently
                fullMode = value;
                Monitor.PulseAll(messageQueue);
            }
        }
    }

    /// <summary>
    /// Gets or sets the max messages to queue.
    /// </summary>
    public int MaxQueueLength
    {
        get => maxQueuedMessages;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxQueueLength), $"{nameof(MaxQueueLength)} must be larger than zero.");
            }

            lock (messageQueue)
            {
                maxQueuedMessages = value;
                Monitor.PulseAll(messageQueue);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        CompleteAdding();

        try
        {
            _ = outputThread.Join(1500); // with timeout in-case Console is locked by user input
        }
        catch (ThreadStateException)
        {
        }
    }

    /// <summary>
    /// Enqueue a new log message.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    public virtual void EnqueueMessage(LogMessageEntry message)
    {
        // cannot enqueue when adding is completed
        if (!Enqueue(message))
        {
            WriteMessage(message);
        }
    }

    private void CompleteAdding()
    {
        lock (messageQueue)
        {
            isAddingCompleted = true;
            Monitor.PulseAll(messageQueue);
        }
    }

    private bool Enqueue(LogMessageEntry item)
    {
        lock (messageQueue)
        {
            while (messageQueue.Count >= MaxQueueLength && !isAddingCompleted)
            {
                if (FullMode == TextBlockLoggerQueueFullMode.DropWrite)
                {
                    messagesDropped++;
                    return true;
                }

                Debug.Assert(FullMode == TextBlockLoggerQueueFullMode.Wait, "Invalid full mode.");
                _ = Monitor.Wait(messageQueue);
            }

            if (!isAddingCompleted)
            {
                Debug.Assert(messageQueue.Count < MaxQueueLength, "Message queue is full.");
                var startedEmpty = messageQueue.Count == 0;
                if (messagesDropped > 0)
                {
                    messageQueue.Enqueue(new LogMessageEntry(message: $"{messagesDropped} message(s) dropped because of queue size limit. Increase the queue size or decrease logging verbosity to avoid this. You may change `TextBlockLoggerQueueFullMode` to stop dropping messages."));
                    messagesDropped = 0;
                }

                // if we just logged the dropped message warning this could push the queue size to
                // MaxLength + 1, that shouldn't be a problem. It won't grow any further until it is less than
                // MaxLength once again.
                messageQueue.Enqueue(item);

                // if the queue started empty it could be at 1 or 2 now
                if (startedEmpty)
                {
                    // pulse for wait in Dequeue
                    Monitor.PulseAll(messageQueue);
                }

                return true;
            }
        }

        return false;
    }

    private void ProcessLogQueue()
    {
        while (TryDequeue(out var message))
        {
            WriteMessage(message);
        }
    }

    private bool TryDequeue(out LogMessageEntry item)
    {
        lock (messageQueue)
        {
            while (messageQueue.Count == 0 && !isAddingCompleted)
            {
                _ = Monitor.Wait(messageQueue);
            }

            if (messageQueue.Count > 0 && !isAddingCompleted)
            {
                item = messageQueue.Dequeue();
                if (messageQueue.Count == MaxQueueLength - 1)
                {
                    // pulse for wait in Enqueue
                    Monitor.PulseAll(messageQueue);
                }

                return true;
            }

            item = default;
            return false;
        }
    }

    private void WriteMessage(LogMessageEntry entry)
    {
        try
        {
            foreach (var textBlock in textBlockProvider.Sinks)
            {
                textBlock.Write(entry.Message);
            }
        }
        catch
        {
            CompleteAdding();
        }
    }
}