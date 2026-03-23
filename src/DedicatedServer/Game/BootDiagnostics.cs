using System.Threading;

namespace DedicatedServer.Game;

/// <summary>
/// Counts non-fatal exceptions captured during the server boot sequence.
///
/// Exposed via /api/state as <c>bootErrorCount</c> so the web visualiser
/// shows a red alert when the server booted with errors (e.g. missing YAML
/// element files, failed asset extraction, etc.).
///
/// Thread-safe: <see cref="Record"/> uses <see cref="Interlocked.Increment"/>.
/// </summary>
public static class BootDiagnostics {

    private static int _errorCount;

    /// <summary>Total non-fatal errors captured since process start.</summary>
    public static int ErrorCount => _errorCount;

    /// <summary>Increment the error counter by one.</summary>
    public static void Record() => Interlocked.Increment(ref _errorCount);

    /// <summary>
    /// Reset the counter to zero. Intended for use in tests only —
    /// not called anywhere in production code.
    /// </summary>
    internal static void Reset() => Interlocked.Exchange(ref _errorCount, 0);
}
