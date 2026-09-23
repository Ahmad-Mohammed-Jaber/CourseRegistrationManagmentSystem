using Serilog.Core;
using Serilog.Events;

namespace Shared.Logging;

public sealed class FailoverSink : ILogEventSink, IDisposable
{
    private readonly ILogEventSink _primary;
    private readonly ILogEventSink _fallback;
    private bool _disposed;

    public FailoverSink(ILogEventSink primary, ILogEventSink fallback)
    {
        _primary = primary ?? throw new ArgumentNullException(nameof(primary));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    public void Emit(LogEvent logEvent)
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _primary.Emit(logEvent);
        }
        catch (Exception primaryEx)
        {
            System.Diagnostics.Debug.WriteLine($"FailoverSink primary failed, using fallback: {primaryEx.Message}");
            try
            {
                _fallback.Emit(logEvent);
            }
            catch (Exception fallbackEx)
            {
                System.Diagnostics.Debug.WriteLine($"FailoverSink fallback failed: {fallbackEx.Message}");
            }
        }
    }

    public void Dispose()
    {
        _disposed = true;
        (_primary as IDisposable)?.Dispose();
        (_fallback as IDisposable)?.Dispose();
    }
}
