using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TrayManager.Features.ListProcesses;

public class ProcessProvider(ILogger<ProcessProvider> logger) : IProcessProvider
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    public IReadOnlyList<AppProcessInfo> GetUserProcesses()
    {
        var grouped = new Dictionary<string, List<int>>(PathComparer);
        var names = new Dictionary<string, string>(PathComparer);
        int totalScanned = 0;
        int skippedAccess = 0;

        foreach (var proc in Process.GetProcesses())
        {
            totalScanned++;
            try
            {
                if (proc.MainWindowHandle != IntPtr.Zero) continue;

                string? exePath = null;
                try { exePath = proc.MainModule?.FileName; }
                catch (Exception ex)
                {
                    skippedAccess++;
                    logger.LogDebug(ex, "Cannot access module for PID {Pid} ({Name})", proc.Id, proc.ProcessName);
                }
                if (string.IsNullOrEmpty(exePath)) continue;

                if (exePath.StartsWith(@"C:\WINDOWS\", StringComparison.OrdinalIgnoreCase)) continue;

                if (!grouped.TryGetValue(exePath, out var pids))
                {
                    pids = [];
                    grouped[exePath] = pids;
                    names[exePath] = proc.ProcessName;
                }

                pids.Add(proc.Id);
            }
            catch (Exception ex)
            {
                skippedAccess++;
                logger.LogWarning(ex, "Error processing PID {Pid}", proc.Id);
            }
            finally
            {
                proc.Dispose();
            }
        }

        var result = grouped
            .Select(kvp => new AppProcessInfo(
                names[kvp.Key],
                kvp.Key,
                kvp.Value.ToArray(),
                kvp.Value.Count))
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        logger.LogDebug("Scanned {Total} processes, {Skipped} inaccessible, {Result} user apps returned",
            totalScanned, skippedAccess, result.Count);

        return result;
    }
}
