namespace TrayManager.Features.ListProcesses;

public record AppProcessInfo(
    string Name,
    string ExePath,
    int[] Pids,
    int InstanceCount);
