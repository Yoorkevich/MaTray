using System.Threading;
using TrayManager.Features.KillProcess;
using TrayManager.Features.ListProcesses;
using TrayManager.Features.Overlay;

namespace TrayManager;

public partial class App : System.Windows.Application
{
    private static Mutex? _mutex;

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        const string mutexName = "TrayManager_SingleInstance";
        _mutex = new Mutex(true, mutexName, out bool createdNew);
        if (!createdNew)
        {
            Current.Shutdown();
            return;
        }

        base.OnStartup(e);

        var listHandler = new ListProcessesHandler(new ProcessProvider());
        var killHandler = new KillProcessHandler(new ProcessKiller());
        var vm = new OverlayViewModel(listHandler, killHandler);
        vm.Refresh();

        var window = new OverlayWindow { DataContext = vm };
        window.Show();
    }
}

