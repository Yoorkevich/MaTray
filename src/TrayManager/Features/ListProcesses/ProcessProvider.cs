using System.Diagnostics;

namespace TrayManager.Features.ListProcesses;

public class ProcessProvider : IProcessProvider
{
    private static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;

    public IReadOnlyList<AppProcessInfo> GetUserProcesses()
    {
        var grouped = new Dictionary<string, List<int>>(PathComparer);
        var names = new Dictionary<string, string>(PathComparer);

        foreach (var proc in Process.GetProcesses())
        {
            try
            {
                if (proc.MainWindowHandle != IntPtr.Zero) continue;

                string? exePath = null;
                try { exePath = proc.MainModule?.FileName; } catch { }
                if (string.IsNullOrEmpty(exePath)) continue;

                // Skip system processes
                if (exePath.StartsWith(@"C:\WINDOWS\", StringComparison.OrdinalIgnoreCase)) continue;

                if (!grouped.TryGetValue(exePath, out var pids))
                {
                    pids = [];
                    grouped[exePath] = pids;
                    names[exePath] = proc.ProcessName;
                }

                pids.Add(proc.Id);
            }
            catch
            {
                // Access denied or process exited — skip
            }
            finally
            {
                proc.Dispose();
            }
        }

        return grouped
            .Select(kvp => new AppProcessInfo(
                names[kvp.Key],
                kvp.Key,
                kvp.Value.ToArray(),
                kvp.Value.Count))
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
