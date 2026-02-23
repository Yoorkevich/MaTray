using System.Diagnostics;

namespace TrayManager.Features.KillProcess;

public class ProcessKiller : IProcessKiller
{
    public KillProcessResult Kill(KillProcessRequest request)
    {
        int killed = 0;
        var errors = new List<string>();

        foreach (var pid in request.Pids)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                proc.Kill();
                proc.WaitForExit(3000);
                killed++;
            }
            catch (ArgumentException)
            {
                // Already exited — counts as success
                killed++;
            }
            catch (Exception ex)
            {
                errors.Add($"PID {pid}: {ex.Message}");
            }
        }

        if (errors.Count == 0)
            return new KillProcessResult(true, $"Killed {request.ProcessName} ({killed} instance(s))");

        return new KillProcessResult(false,
            $"Killed {killed}/{request.Pids.Length}: {string.Join("; ", errors)}");
    }
}
