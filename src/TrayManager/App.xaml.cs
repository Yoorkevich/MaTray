using System.Threading;
using Microsoft.Extensions.Logging;
using TrayManager.Features.KillProcess;
using TrayManager.Features.ListProcesses;
using TrayManager.Features.Overlay;
using TrayManager.Infrastructure.Logging;

namespace TrayManager;

public partial class App : System.Windows.Application
{
    private static Mutex? _mutex;
    private ILoggerFactory? _loggerFactory;

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        _loggerFactory = LoggingSetup.Create();
        var appLogger = _loggerFactory.CreateLogger<App>();

        const string mutexName = "TrayManager_SingleInstance";
        _mutex = new Mutex(true, mutexName, out bool createdNew);
        if (!createdNew)
        {
            appLogger.LogInformation("Another instance already running, shutting down");
            Current.Shutdown();
            return;
        }

        // Global exception handlers
        DispatcherUnhandledException += (_, args) =>
        {
            appLogger.LogError(args.Exception, "Unhandled dispatcher exception");
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            appLogger.LogError(args.ExceptionObject as Exception, "Unhandled domain exception");
        };

        appLogger.LogInformation("TrayManager started");
        base.OnStartup(e);

        var listHandler = new ListProcessesHandler(
            new ProcessProvider(_loggerFactory.CreateLogger<ProcessProvider>()));
        var killHandler = new KillProcessHandler(
            new ProcessKiller(_loggerFactory.CreateLogger<ProcessKiller>()));
        var vm = new OverlayViewModel(listHandler, killHandler,
            _loggerFactory.CreateLogger<OverlayViewModel>());
        vm.Refresh();

        var window = new OverlayWindow { DataContext = vm };
        window.Show();
    }

    protected override void OnExit(System.Windows.ExitEventArgs e)
    {
        _loggerFactory?.Dispose();
        base.OnExit(e);
    }
}

