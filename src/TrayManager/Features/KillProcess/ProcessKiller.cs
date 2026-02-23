using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TrayManager.Features.KillProcess;

public class ProcessKiller(ILogger<ProcessKiller> logger) : IProcessKiller
{
    public KillProcessResult Kill(KillProcessRequest request)
    {
        logger.LogInformation("Kill requested: {Name} PIDs=[{Pids}]",
            request.ProcessName, string.Join(", ", request.Pids));

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
                logger.LogInformation("Killed PID {Pid}", pid);
            }
            catch (ArgumentException)
            {
                killed++;
                logger.LogInformation("PID {Pid} already exited", pid);
            }
            catch (Exception ex)
            {
                errors.Add($"PID {pid}: {ex.Message}");
                logger.LogError(ex, "Failed to kill PID {Pid}", pid);
            }
        }

        if (errors.Count == 0)
            return new KillProcessResult(true, $"Killed {request.ProcessName} ({killed} instance(s))");

        return new KillProcessResult(false,
            $"Killed {killed}/{request.Pids.Length}: {string.Join("; ", errors)}");
    }
}
