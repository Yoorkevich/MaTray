namespace TrayManager.Features.ListProcesses;

public interface IProcessProvider
{
    IReadOnlyList<AppProcessInfo> GetUserProcesses();
}
