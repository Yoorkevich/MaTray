namespace TrayManager.Features.KillProcess;

public interface IProcessKiller
{
    KillProcessResult Kill(KillProcessRequest request);
}
