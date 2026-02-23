using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using TrayManager.Features.KillProcess;
using TrayManager.Features.ListProcesses;
using TrayManager.Infrastructure.Wpf;

namespace TrayManager.Features.Overlay;

public class OverlayViewModel : INotifyPropertyChanged
{
    private readonly ListProcessesHandler _listHandler;
    private readonly KillProcessHandler _killHandler;
    private readonly ILogger<OverlayViewModel> _logger;

    public ObservableCollection<AppProcessInfo> Processes { get; } = [];

    public RelayCommand<AppProcessInfo> KillCommand { get; }

    public OverlayViewModel(
        ListProcessesHandler listHandler,
        KillProcessHandler killHandler,
        ILogger<OverlayViewModel> logger)
    {
        _listHandler = listHandler;
        _killHandler = killHandler;
        _logger = logger;

        KillCommand = new RelayCommand<AppProcessInfo>(OnKill);
    }

    public void Refresh()
    {
        Processes.Clear();
        foreach (var p in _listHandler.Handle())
            Processes.Add(p);

        _logger.LogInformation("Refresh: {Count} processes loaded", Processes.Count);
    }

    private void OnKill(AppProcessInfo? info)
    {
        if (info is null) return;

        _logger.LogInformation("Kill command: {Name}", info.Name);
        var result = _killHandler.Handle(new KillProcessRequest(info.Pids, info.Name));

        if (result.Success)
        {
            Processes.Remove(info);
        }
        else
        {
            _logger.LogWarning("Kill failed for {Name}: {Message}", info.Name, result.Message);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
