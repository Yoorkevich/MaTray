namespace TrayManager.Features.KillProcess;

public class KillProcessHandler(IProcessKiller killer)
{
    public KillProcessResult Handle(KillProcessRequest request) => killer.Kill(request);
}
