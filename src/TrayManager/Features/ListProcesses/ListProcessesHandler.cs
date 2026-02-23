namespace TrayManager.Features.ListProcesses;

public class ListProcessesHandler(IProcessProvider provider)
{
    public IReadOnlyList<AppProcessInfo> Handle() => provider.GetUserProcesses();
}
