namespace TrayManager.Features.KillProcess;

public record KillProcessRequest(int[] Pids, string ProcessName);

public record KillProcessResult(bool Success, string Message);
